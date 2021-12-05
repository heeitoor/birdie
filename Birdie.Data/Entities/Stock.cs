using System;

namespace Birdie.Data.Entities
{
    public class Stock
    {
        public virtual int Id { get; set; }

        public virtual string Symbol { get; set; }

        public virtual DateTimeOffset Date { get; set; }

        public virtual decimal Open { get; set; }

        public virtual decimal High { get; set; }

        public virtual decimal Low { get; set; }

        public virtual decimal Close { get; set; }

        public virtual DateTimeOffset UpdatedAt { get; set; }

        public virtual int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
