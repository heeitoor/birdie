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
    public class UserBusinessTestFixture : IDisposable
    {
        public UserBusinessTestFixture() { }

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

    [Trait("Business", "User")]
    public class UserBusinessTest : IClassFixture<UserBusinessTestFixture>
    {
        private readonly UserBusinessTestFixture fixture;

        public UserBusinessTest(UserBusinessTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public void AutenticateSuccess()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            User resultOfGetForLogin = new User { Id = 99 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IUserRepository> userRepository = new Mock<IUserRepository>();

                userRepository
                    .Setup(x => x.GetForLogin(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(resultOfGetForLogin);

                return userRepository.Object;
            });

            servicesCollection.AddTransient<IUserBusiness, UserBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IUserBusiness userBusiness = serviceProvider.GetService<IUserBusiness>();

            int result = userBusiness.Authenticate(new UserLoginModel
            {
                UserName = "dummy",
                Password = "dummy"
            }).Result;

            Assert.Equal(resultOfGetForLogin.Id, result);
        }

        [Fact]
        public void AutenticateFail()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            User resultOfGetForLogin = new User { Id = -1 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IUserRepository> userRepository = new Mock<IUserRepository>();

                userRepository
                    .Setup(x => x.GetForLogin(It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(resultOfGetForLogin);

                return userRepository.Object;
            });

            servicesCollection.AddTransient<IUserBusiness, UserBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IUserBusiness userBusiness = serviceProvider.GetService<IUserBusiness>();

            int result = userBusiness.Authenticate(new UserLoginModel
            {
                UserName = "dummy",
                Password = "dummy"
            }).Result;

            Assert.Equal(resultOfGetForLogin.Id, result);
        }

        [Fact]
        public void CreateSuccess()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            User resultOfGetForLogin = new User { Id = -1 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IUserRepository> userRepository = new Mock<IUserRepository>();

                userRepository
                    .Setup(x => x.CreateUser(It.IsAny<User>()))
                    .ReturnsAsync(new User { Id = 99 });

                userRepository
                    .Setup(x => x.Exists(It.IsAny<string>()))
                    .Returns(false);

                return userRepository.Object;
            });

            servicesCollection.AddTransient<IUserBusiness, UserBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IUserBusiness userBusiness = serviceProvider.GetService<IUserBusiness>();

            bool result = userBusiness.Create(new UserSignUpModel
            {
                UserName = "dummy",
                Password = "dummy",
                PasswordConfirm = "dummy"
            }).Result;

            Assert.True(result);
        }

        [Fact]
        public void CreateFail()
        {
            ServiceCollection servicesCollection = new ServiceCollection();

            User resultOfGetForLogin = new User { Id = -1 };

            servicesCollection.AddTransient((services) =>
            {
                Mock<IUserRepository> userRepository = new Mock<IUserRepository>();

                userRepository
                    .Setup(x => x.CreateUser(It.IsAny<User>()))
                    .ReturnsAsync(new User { Id = 99 });

                userRepository
                    .Setup(x => x.Exists(It.IsAny<string>()))
                    .Returns(true);

                return userRepository.Object;
            });

            servicesCollection.AddTransient<IUserBusiness, UserBusiness>();

            IServiceProvider serviceProvider = fixture.GetServices(servicesCollection);

            IUserBusiness userBusiness = serviceProvider.GetService<IUserBusiness>();

            bool result = userBusiness.Create(new UserSignUpModel
            {
                UserName = "dummy",
                Password = "dummy",
                PasswordConfirm = "dummy"
            }).Result;

            Assert.False(result);
        }
    }
}
