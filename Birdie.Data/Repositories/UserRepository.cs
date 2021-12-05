using Birdie.Data.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Birdie.Data.Repositories
{
    public interface IUserRepository
    {
        User GetForLogin(string userName, string password);

        bool Exists(string userName);

        Task<User> CreateUser(User entity);
    }

    internal class UserRepository : RepositoryBase<User>, IUserRepository
    {
        public UserRepository(BirdieDbContext birdieDbContext) : base(birdieDbContext) { }

        public User GetForLogin(string userName, string password)
        {
            return base.Get(x => x.UserName == userName && x.Password == password).FirstOrDefault();
        }

        public Task<User> CreateUser(User entity)
        {
            return base.Create(entity);
        }

        public bool Exists(string userName)
        {
            return base.Get(x => x.UserName == userName).Any();
        }
    }
}
