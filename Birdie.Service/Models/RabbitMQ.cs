namespace Birdie.Service.Models
{
    public class StockQueueModel
    {
        public virtual string CorrelationId { get; set; }

        public virtual string Search { get; set; }

        public virtual int UserId { get; set; }
    }

    public class ChatQueueModel
    {
        public virtual string Content { get; set; }

        public virtual string RoomIdentifier { get; set; }

        public virtual int UserId { get; set; }
    }
}
