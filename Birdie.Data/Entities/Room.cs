using System;
using System.Collections.Generic;

namespace Birdie.Data.Entities
{
    public class Room
    {
        public virtual int Id { get; set; }

        public virtual int UserId { get; set; }

        public virtual Guid Identifier { get; set; }

        public virtual string Name { get; set; }

        public virtual DateTimeOffset UpdatedAt { get; set; }

        public virtual User User { get; set; }

        public virtual ICollection<Message> Message { get; set; }
    }
}
