using System;
using System.Collections.Generic;

namespace Birdie.Data.Entities
{
    public class User
    {
        public virtual int Id { get; set; }

        public virtual string UserName { get; set; }

        public virtual string Password { get; set; }

        public virtual DateTimeOffset UpdatedAt { get; set; }

        public virtual ICollection<Message> Message { get; set; }

        public virtual ICollection<Room> Room { get; set; }

        public virtual ICollection<Stock> Stock { get; set; }
    }
}
