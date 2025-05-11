namespace MessagingProcessor.Models
{
    public enum MessageStatus
    {
        Pending,
        Processing,
        Sent,
        Failed,
        Retried,
        DeadLetter
    }

}
