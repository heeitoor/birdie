using System;
using System.Linq;
using Birdie.Data.Entities;
using Birdie.Data.Repositories;
using Birdie.Service.Models;

namespace Birdie.Service.Business
{
    public interface IRoomBusiness
    {
        IQueryable<RoomItemModel> Get();

        int GetRommIdByIdentifier(string identifier);
    }

    public class RoomBusiness : IRoomBusiness
    {
        private readonly IRoomRepository roomRepository;

        public RoomBusiness(IRoomRepository roomRepository)
        {
            this.roomRepository = roomRepository;
        }

        public IQueryable<RoomItemModel> Get()
        {
            return roomRepository.GetAll().Select(x => new RoomItemModel
            {
                Identifier = x.Identifier,
                Name = x.Name,
                UpdatedAt = x.UpdatedAt,
            });
        }

        public int GetRommIdByIdentifier(string identifier)
        {
            Room room = roomRepository.GetByIdentifier(new Guid(identifier));

            if (room == null) return -1;

            return room.Id;
        }
    }
}
