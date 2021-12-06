using Birdie.Data.Entities;
using System;
using System.Linq;

namespace Birdie.Data.Repositories
{
    public interface IRoomRepository
    {
        IQueryable<Room> GetAll();

        Room GetByIdentifier(Guid identifier);
    }

    internal class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(BirdieDbContext birdieDbContext) : base(birdieDbContext) { }

        public IQueryable<Room> GetAll()
        {
            return base.Get();
        }

        public Room GetByIdentifier(Guid identifier)
        {
            return base.Get(x => x.Identifier == identifier).FirstOrDefault();
        }
    }
}
