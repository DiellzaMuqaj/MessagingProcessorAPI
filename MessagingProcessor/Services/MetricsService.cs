namespace MessagingProcessor.Services
{
    public class MetricsService : IMetricsService
    {
        private int _processed;
        private int _failed;
        private int _queued;
        private readonly Queue<DateTime> _timestamps = new();

        public void IncrementProcessed()
        {
            Interlocked.Increment(ref _processed);

            lock (_timestamps)
            {
                _timestamps.Enqueue(DateTime.Now);
                while (_timestamps.Count > 0 && (DateTime.Now - _timestamps.Peek()).TotalSeconds > 10)
                    _timestamps.Dequeue();
            }
        }

        public void IncrementFailed() => Interlocked.Increment(ref _failed);
        public void IncrementQueued() => Interlocked.Increment(ref _queued);
        public void DecrementQueued() => Interlocked.Decrement(ref _queued);

        public int GetThroughput()
        {
            lock (_timestamps)
            {
                return _timestamps.Count;
            }
        }

        public int GetQueueDepth() => _queued;

        public double GetErrorRate()
        {
            int total = _processed;
            if (total == 0) return 0;
            return (double)_failed / total;
        }
    }
}