using MessagingProcessor.Models;

namespace MessagingProcessor.DTO
{
    public class MessageRequest
    {
        public MessageType Type { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessagePriority Priority { get; set; } = MessagePriority.Medium;

    }
}
