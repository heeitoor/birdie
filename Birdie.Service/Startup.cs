using Birdie.Service;
using Birdie.Service.Business;
using Birdie.Service.Managers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Birdie
{
    public static class Startup
    {
        public static IServiceCollection AddBirdieServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IStockBusiness, StockBusiness>();
            services.AddTransient<IUserBusiness, UserBusiness>();
            services.AddTransient<IRoomBusiness, RoomBusiness>();
            services.AddTransient<IMessageBusiness, MessageBusiness>();
            services.AddSingleton<IConnectionFactory>(new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:HostName"],
                UserName = configuration["RabbitMQ:UserName"],
                Password = configuration["RabbitMQ:Password"],
                VirtualHost = configuration["RabbitMQ:VirtualHost"],
                Port = int.Parse(configuration["RabbitMQ:Port"])
            });
            services.AddTransient<IStockHttpService, StockHttpService>();
            services.AddSingleton<IRabbitMQService, RabbitMQService>();
            services.AddSingleton<ISocketManager, SocketManager>();
            services.AddBirdieData(configuration);
            return services;
        }
    }
}
