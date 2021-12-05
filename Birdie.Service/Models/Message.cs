namespace Birdie.Service.Models
{
    public class MessageCreateModel
    {
        public virtual int UserId { get; set; }

        public virtual int RoomId { get; set; }

        public virtual string Content { get; set; }
    }
}
