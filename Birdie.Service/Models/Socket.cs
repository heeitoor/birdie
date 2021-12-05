using Newtonsoft.Json;

namespace Birdie.Service.Models
{
    public class SimpleSocketMessageModel
    {
        public string Content { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public SocketMessageType Type { get; set; }
    }

    public class SocketMessageModel : SimpleSocketMessageModel
    {
        [JsonIgnore]
        public string ClientId { get; set; }
    }

    public enum SocketMessageType
    {
        Join,
        Leave,
        Content,
        Bot
    }
}
