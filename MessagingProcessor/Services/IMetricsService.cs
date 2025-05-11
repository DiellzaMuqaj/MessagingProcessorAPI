namespace MessagingProcessor.Services
{
    public interface IMetricsService
    {
        void IncrementProcessed();
        void IncrementFailed();
        void IncrementQueued();
        void DecrementQueued();

        int GetThroughput();
        int GetQueueDepth();
        double GetErrorRate();
    }

}
