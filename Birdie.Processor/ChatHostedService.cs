using Birdie.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Birdie.Service.Business;
using Birdie.Service.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Caching.Memory;

namespace Birdie.Processor
{
    public class ChatHostedService : IHostedService
    {
        private readonly ILogger<ChatHostedService> logger;
        private readonly IMessageBusiness messageBusiness;
        private readonly IRoomBusiness roomBusiness;
        private readonly IRabbitMQService rabbitMQService;
        private readonly IConfiguration configuration;
        private readonly IMemoryCache memoryCache;

        private string ChatQueue
        {
            get => configuration["Queue:chat"];
        }

        public ChatHostedService(
            IMessageBusiness messageBusiness,
            IRoomBusiness roomBusiness,
            IRabbitMQService rabbitMQService,
            IConfiguration configuration,
            IMemoryCache memoryCache,
            ILogger<ChatHostedService> logger)
        {
            this.logger = logger;
            this.messageBusiness = messageBusiness;
            this.roomBusiness = roomBusiness;
            this.rabbitMQService = rabbitMQService;
            this.configuration = configuration;
            this.memoryCache = memoryCache;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(rabbitMQService.ChatChannel);

            consumer.Received += Received;

            rabbitMQService.StockChannel.BasicConsume(ChatQueue, true, "chatConsumer", false, false, null, consumer);

            return Task.CompletedTask;
        }

        private void Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);

            logger.LogInformation($"Message received: {message}");

            ChatQueueModel model = JsonConvert.DeserializeObject<ChatQueueModel>(message);

            if (!memoryCache.TryGetValue(model.RoomIdentifier, out int roomId))
            {
                roomId = roomBusiness.GetRommIdByIdentifier(model.RoomIdentifier);

                if (roomId == -1)
                {
                    logger.LogError($"Room {model.RoomIdentifier} wasn't found.");
                    return;
                }

                logger.LogInformation($"Adding room {roomId} to cache.");

                memoryCache.Set(model.RoomIdentifier, roomId);
            }

            messageBusiness.Add(new MessageCreateModel
            {
                Content = model.Content,
                RoomId = roomId,
                UserId = model.UserId
            }).Wait();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
