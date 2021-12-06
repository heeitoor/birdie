using Birdie.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Birdie.Data.Repositories;

namespace Birdie
{
    public static class Startup
    {
        public static IServiceCollection AddBirdieData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<BirdieDbContext>(options =>
            {
                options.UseNpgsql(configuration["ConnectionString"]);
            });

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IRoomRepository, RoomRepository>();
            services.AddTransient<IMessageRepository, MessageRepository>();
            services.AddTransient<IStockRepository, StockRepository>();

            return services;
        }
    }
}
