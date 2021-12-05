using System;

namespace Birdie.Service.Models
{
    public class RoomItemModel
    {
        public virtual Guid Identifier { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTimeOffset UpdatedAt { get; set; }
    }
}
