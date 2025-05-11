namespace MessagingProcessor.Models
{
    public class Message
    {
        public Guid Id { get; set; }
        public MessageType Type { get; set; }
        public string Recipient { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public MessageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public int RetryCount { get; set; }
        public MessagePriority Priority { get; set; } = MessagePriority.Medium;

    }

}
