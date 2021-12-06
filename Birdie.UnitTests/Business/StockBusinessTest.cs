using Birdie.Data.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Birdie.Data.Entities;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;
using Birdie.Service.Business;
using Birdie.Service.Models;

namespace Birdie.UnitTests.Business
{
    public class StockBusinessTestFixture : IDisposable
    {
        public StockBusinessTestFixture() { }

        public IServiceProvider GetServices(ServiceCollection servicesCollection)
        {
            servicesCollection.AddTransient(services =>
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.json");
                return (IConfiguration)configurationBuilder.Build();
            });

            return servicesCollection.BuildServiceProvider();
        }

        public void Dispose() { }
    }

    [Trait("Business", "Stock")]
    public class StockBusinessTest : IClassFixture<StockBusinessTestFixture>
    {
        private readonly StockBusinessTestFixture fixture;

        public StockBusinessTest(StockBusinessTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Create()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            Stock resultOfGetBySymbol = null;
            Stock resultCreateStock = new Stock { Id = 88 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IStockRepository> stockRepository = new Mock<IStockRepository>();

                stockRepository
                    .Setup(x => x.GetBySymbol(It.IsAny<string>()))
                    .Returns(resultOfGetBySymbol);

                stockRepository
                    .Setup(x => x.CreateStock(It.IsAny<Stock>()))
                    .ReturnsAsync(resultCreateStock);

                return stockRepository.Object;
            });

            servicesCollection.AddTransient<IStockBusiness, StockBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IStockBusiness stockBusiness = serviceProvider.GetService<IStockBusiness>();

            int result = stockBusiness.CreateOrUpdate(new StockAddOrUpdateModel
            {
                Symbol = "DMY",
                Low = 9999,
                Open = 9999,
                High = 9999,
                Close = 9999,
                Date = DateTimeOffset.Now,
                UserId = 99
            }).Result;

            Assert.Equal(resultCreateStock.Id, result);
        }


        [Fact]
        public void Update()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            Stock resultOfGetBySymbol = new Stock { Id = 88 };
            Stock resultUpdateStock = new Stock { Id = 88 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IStockRepository> stockRepository = new Mock<IStockRepository>();

                stockRepository
                    .Setup(x => x.GetBySymbol(It.IsAny<string>()))
                    .Returns(resultOfGetBySymbol);

                stockRepository
                    .Setup(x => x.UpdateStock(It.IsAny<Stock>()))
                    .ReturnsAsync(resultUpdateStock);

                return stockRepository.Object;
            });

            servicesCollection.AddTransient<IStockBusiness, StockBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IStockBusiness stockBusiness = serviceProvider.GetService<IStockBusiness>();

            int result = stockBusiness.CreateOrUpdate(new StockAddOrUpdateModel
            {
                Symbol = "DMY",
                Low = 9999,
                Open = 9999,
                High = 9999,
                Close = 9999,
                Date = DateTimeOffset.Now,
                UserId = 99
            }).Result;

            Assert.Equal(resultUpdateStock.Id, result);
        }
    }
}
