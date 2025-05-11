using MessagingProcessor.Models;

namespace MessagingProcessor.Queue
{
    public interface IMessageQueue
    {
        void Enqueue(Message message);
        Message? Dequeue();
        int Count { get; }
    }
}
