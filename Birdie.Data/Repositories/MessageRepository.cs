using Birdie.Data.Entities;
using System.Threading.Tasks;

namespace Birdie.Data.Repositories
{
    public interface IMessageRepository
    {
        Task<Message> Add(Message message);
    }

    internal class MessageRepository : RepositoryBase<Message>, IMessageRepository
    {
        public MessageRepository(BirdieDbContext birdieDbContext) : base(birdieDbContext) { }

        public Task<Message> Add(Message message)
        {
             return base.Create(message);
        }
    }
}
