using RabbitMQ.Client;
using Microsoft.Extensions.Configuration;
using Birdie.Service.Models;

namespace Birdie.Service
{
    public interface IRabbitMQService
    {
        IModel StockChannel { get; }

        IModel ChatChannel { get; }

        void PushToStockQueue(StockQueueModel model);

        void PushToChatQueue(ChatQueueModel model);
    }

    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IConfiguration configuration;
        private IModel _stockChannel;
        private IModel _chatChannel;

        private string StocksQueue
        {
            get => configuration["Queue:stocks"];
        }
        private string ChatQueue
        {
            get => configuration["Queue:chat"];
        }

        public IModel StockChannel
        {
            get
            {
                if (_stockChannel == null)
                {

                    IConnection connection = connectionFactory.CreateConnection();
                    IModel channel = connection.CreateModel();
                    channel.QueueDeclare(queue: StocksQueue, durable: true, exclusive: false, autoDelete: false);
                    _stockChannel = channel;
                }
                return _stockChannel;
            }
        }

        public IModel ChatChannel
        {
            get
            {
                if (_chatChannel == null)
                {

                    IConnection connection = connectionFactory.CreateConnection();
                    IModel channel = connection.CreateModel();
                    channel.QueueDeclare(queue: ChatQueue, durable: true, exclusive: false, autoDelete: false);
                    _chatChannel = channel;
                }
                return _chatChannel;
            }
        }

        public RabbitMQService(IConnectionFactory connectionFactory, IConfiguration configuration)
        {
            this.connectionFactory = connectionFactory;
            this.configuration = configuration;
        }

        public void PushToStockQueue(StockQueueModel model)
        {
            StockChannel
                .BasicPublish(string.Empty, StocksQueue, null, Util.GetJsonBytes(model));
        }

        public void PushToChatQueue(ChatQueueModel model)
        {
            StockChannel
                .BasicPublish(string.Empty, ChatQueue, null, Util.GetJsonBytes(model));
        }
    }
}

