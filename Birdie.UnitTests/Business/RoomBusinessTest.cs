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
    public class RoomBusinessTestFixture : IDisposable
    {
        public RoomBusinessTestFixture() { }

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

    [Trait("Business", "Room")]
    public class RoomBusinessTest : IClassFixture<RoomBusinessTestFixture>
    {
        private readonly RoomBusinessTestFixture fixture;

        public RoomBusinessTest(RoomBusinessTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void Create()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            IQueryable<Room> resultOfGetAll = new[]
            {
                new Room { Id = 1, Name = "New Room", Identifier = Guid.NewGuid() }
            }.AsQueryable();

            servicesCollection.AddTransient((services) =>
            {
                Mock<IRoomRepository> roomRepository = new Mock<IRoomRepository>();

                roomRepository
                    .Setup(x => x.GetAll())
                    .Returns(resultOfGetAll);

                return roomRepository.Object;
            });

            servicesCollection.AddTransient<IRoomBusiness, RoomBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IRoomBusiness roomBusiness = serviceProvider.GetService<IRoomBusiness>();

            IQueryable<RoomItemModel> result = roomBusiness.Get();

            Assert.Equal(result.Count(), resultOfGetAll.Count());
        }
    }
}
