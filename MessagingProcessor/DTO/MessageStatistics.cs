using MessagingProcessor.Models;

namespace MessagingProcessor.DTO
{
    public class MessageStatistics
    {
        public MessageType Type { get; set; }
        public MessageStatus Status { get; set; }
        public int Count { get; set; }
    }
}
