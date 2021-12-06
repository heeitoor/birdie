using Birdie.Service;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System;
using Birdie.Service.Business;
using Birdie.Service.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.WebSockets;

namespace Birdie.Processor
{
    public class RabbitHostedService : IHostedService
    {
        private readonly ILogger<RabbitHostedService> logger;
        private readonly IRabbitMQService rabbitMQService;
        private readonly IStockHttpService stockHttpService;
        private readonly IStockBusiness stockBusiness;
        private readonly IConfiguration configuration;

        private string StocksQueue
        {
            get => configuration["Queue:stocks"];
        }

        private string SocketAddress
        {
            get => configuration["BirdieSocket"];
        }

        public RabbitHostedService(
            IRabbitMQService rabbitMQService,
            IStockHttpService stockHttpService,
            IStockBusiness stockBusiness,
            IConfiguration configuration,
            ILogger<RabbitHostedService> logger)
        {
            this.logger = logger;
            this.rabbitMQService = rabbitMQService;
            this.stockHttpService = stockHttpService;
            this.stockBusiness = stockBusiness;
            this.configuration = configuration;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(rabbitMQService.StockChannel);

            consumer.Received += Received;

            rabbitMQService.StockChannel.BasicConsume(StocksQueue, true, "stockConsumer", false, false, null, consumer);

            return Task.CompletedTask;
        }

        private void Received(object sender, BasicDeliverEventArgs e)
        {
            string message = Encoding.UTF8.GetString(e.Body);

            logger.LogInformation($"Message received: {message}");

            StockQueueModel model = JsonConvert.DeserializeObject<StockQueueModel>(message);

            Match match = Regex.Match(model.Search, "^\\/stock=.*");

            if (match.Success)
            {
                string symbol = GetStockSymbol(match.Value);

                if (!string.IsNullOrEmpty(symbol))
                {
                    StockAddOrUpdateModel stock = TryGetStock(model.CorrelationId, symbol);

                    if (stock != null)
                    {
                        stock.UserId = model.UserId;

                        stockBusiness.CreateOrUpdate(stock).Wait();

                        string responseMessage = $"{symbol.ToUpper()} quote is ${stock.Open} per share";

                        logger.LogInformation($"Notifying with {responseMessage}");

                        NotifyRoom(model.CorrelationId, responseMessage).Wait();
                    }
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private async Task NotifyRoom(string correlationId, string message)
        {
            using (ClientWebSocket clientWebSocket = new ClientWebSocket())
            {
                byte[] buffer = new byte[1024 * 2];

                await clientWebSocket.ConnectAsync(new Uri(SocketAddress), CancellationToken.None);

                while (clientWebSocket.State == WebSocketState.Open)
                {
                    var joinRoomMessage = Util.GetJsonAsArraySegment(new SocketMessageModel
                    {
                        Content = correlationId,
                        Type = SocketMessageType.Join,
                        UserId = (int)SpecialUser.BotUser
                    });

                    await clientWebSocket.SendAsync(joinRoomMessage, WebSocketMessageType.Text, true, CancellationToken.None);

                    var resultMessage = Util.GetJsonAsArraySegment(new SocketMessageModel
                    {
                        Content = message,
                        Type = SocketMessageType.Content,
                        UserId = (int)SpecialUser.BotUser
                    });

                    await clientWebSocket.SendAsync(resultMessage, WebSocketMessageType.Text, true, CancellationToken.None);

                    break;
                }

                await clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
            }
        }

        private string GetStockSymbol(string search)
        {
            return search.Split('=').LastOrDefault()?.Split(' ').FirstOrDefault();
        }

        private StockAddOrUpdateModel TryGetStock(string correlationId, string symbol)
        {
            try
            {
                return stockHttpService.RetrieveStock(symbol).Result;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error while getting {symbol} information. More detail: {ex.Message}");
                NotifyRoom(correlationId, $"Unfortunately information for {symbol} wasn't found").Wait();
            }

            return null;
        }
    }
}
