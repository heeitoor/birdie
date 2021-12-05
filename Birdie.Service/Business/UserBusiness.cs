using System;
using System.Threading.Tasks;
using Birdie.Data.Entities;
using Birdie.Data.Repositories;
using Birdie.Service.Helpers;
using Birdie.Service.Models;
using Microsoft.Extensions.Configuration;

namespace Birdie.Service.Business
{
    public interface IUserBusiness
    {
        Task<int> Authenticate(UserLoginModel model);

        Task<bool> Create(UserSignUpModel model);
    }

    public class UserBusiness : IUserBusiness
    {
        private readonly IUserRepository userRepository;
        private readonly IConfiguration configuration;

        public UserBusiness(IUserRepository userRepository, IConfiguration configuration)
        {
            this.userRepository = userRepository;
            this.configuration = configuration;
        }

        public Task<int> Authenticate(UserLoginModel model)
        {
            string password = SecurityHelper.Encrypt(configuration["PrivateKey"], model.Password);

            User user = userRepository.GetForLogin(model.UserName, password);

            if (user == null)
            {
                return Task.FromResult(-1);
            }
            else
            {
                return Task.FromResult(user.Id);
            }
        }

        public async Task<bool> Create(UserSignUpModel model)
        {
            bool exists = userRepository.Exists(model.UserName);

            if (exists)
            {
                return false;
            }

            string password = SecurityHelper.Encrypt(configuration["PrivateKey"], model.Password);

            await userRepository.CreateUser(new User
            {
                UserName = model.UserName,
                Password = password,
                UpdatedAt = DateTimeOffset.Now
            });

            return true;
        }
    }
}
