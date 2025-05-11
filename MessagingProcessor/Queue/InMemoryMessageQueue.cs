using MessagingProcessor.Models;
using System.Collections.Generic;

namespace MessagingProcessor.Queue
{
    public class InMemoryMessageQueue : IMessageQueue
    {
        private readonly List<Message> _queue = new();
        private readonly object _lock = new();

        public void Enqueue(Message message)
        {
            lock (_lock)
            {
                _queue.Add(message);
            }
        }

        public Message? Dequeue()
        {
            lock (_lock)
            {
                if (_queue.Count == 0)
                    return null;

                // Lower enum value = higher priority (High=1, Medium=2, Low=3)
                var highestPriorityMessage = _queue
                    .OrderBy(m => m.Priority)
                    .ThenBy(m => m.CreatedAt)
                    .First();

                _queue.Remove(highestPriorityMessage);
                return highestPriorityMessage;
            }
        }

        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _queue.Count;
                }
            }
        }
    }
}
