using System;

namespace Birdie.Data.Entities
{
    public class Message
    {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual int RoomId { get; set; }

        public virtual string Content { get; set; }

        public virtual DateTimeOffset UpdatedAt { get; set; }

        public virtual User User { get; set; }

        public virtual Room Room { get; set; }
    }
}
