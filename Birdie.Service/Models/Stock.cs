using System;

namespace Birdie.Service.Models
{
    public class StockAddOrUpdateModel
    {
        public virtual int UserId { get; set; }

        public virtual string Symbol { get; set; }

        public virtual DateTimeOffset Date { get; set; }

        public virtual decimal Open { get; set; }

        public virtual decimal High { get; set; }

        public virtual decimal Low { get; set; }

        public virtual decimal Close { get; set; }
    }
}
