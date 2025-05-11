
using System.Collections.Concurrent;
using MessagingProcessor.DTO;
using MessagingProcessor.Models;
using MessagingProcessor.Persistence;
using MessagingProcessor.Queue;
using MessagingProcessor.Simulators;

namespace MessagingProcessor.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageQueue _queue;
        private readonly IMessageRepository _repository;
        private readonly ILogger<MessageService> _logger;
        private readonly SemaphoreSlim _throttler;
        private readonly IExternalApiSimulator _apiSimulator;
        private readonly IMetricsService _metrics;
        private const int MaxConcurrency = 5;
        private const int MaxRetries = 3;

      

        public MessageService(
            IMessageQueue queue,
            IMessageRepository repository,
            IExternalApiSimulator apiSimulator,
            ILogger<MessageService> logger,
            IMetricsService metrics)
        {
            _queue = queue;
            _repository = repository;
            _apiSimulator = apiSimulator;
            _logger = logger;
            _throttler = new SemaphoreSlim(MaxConcurrency); 
            _metrics = metrics;
            RecoverUnprocessedMessagesAsync().Wait();
            StartProcessing();
       
        }

        public async Task SubmitMessageAsync(Message message)
        {
            message.Id = Guid.NewGuid();
            message.Status = MessageStatus.Pending;
            message.CreatedAt = DateTime.Now;

            await _repository.AddMessageAsync(message);

            _queue.Enqueue(message);
            _metrics.IncrementQueued(); 
        }

        internal async Task RecoverUnprocessedMessagesAsync()

        {
            var messages = await _repository.GetUnprocessedMessagesAsync();
            foreach (var message in messages)
            {
                _logger.LogWarning("Recovering message {Id} with status {Status}", message.Id, message.Status);
                _queue.Enqueue(message);
            }

            _logger.LogInformation("Recovered {Count} pending/processing/retried/deadLetter messages", messages.Count());
        }

        private void StartProcessing()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    var message = _queue.Dequeue();
                    if (message != null)
                    {
                        _ = ProcessMessageAsync(message);
                    }
                    await Task.Delay(100);
                }
            });
        }

        internal async Task ProcessMessageAsync(Message message)
        {
            await _throttler.WaitAsync();
            _metrics.DecrementQueued(); 

            try
            {
                message.Status = MessageStatus.Processing;
                await _repository.UpdateMessageStatusAsync(message.Id, MessageStatus.Processing);

                var success = await _apiSimulator.SendAsync(message);

                if (success)
                {
                    message.Status = MessageStatus.Sent;
                    _metrics.IncrementProcessed();
                    _logger.LogInformation("Message {Id} sent successfully", message.Id);
                }
                else
                {
                    message.RetryCount++;
                    _metrics.IncrementFailed();

                    if (message.RetryCount <= MaxRetries)
                    {
                        message.Status = MessageStatus.Retried;

                        var delay = TimeSpan.FromSeconds(Math.Pow(2, message.RetryCount));
                        _logger.LogWarning("Retry {Count} for message {Id} with {Delay}s delay", message.RetryCount, message.Id, delay.TotalSeconds);
                        await Task.Delay(delay);

                        _queue.Enqueue(message);
                        _metrics.IncrementQueued();
                    }
                    else
                    {
                        message.Status = MessageStatus.DeadLetter;
                        _logger.LogError("Message {Id} failed permanently after {RetryCount} retries", message.Id, message.RetryCount);
                    }
                }

                await _repository.UpdateMessageAsync(message);
            }
            catch (Exception ex)
            {
                _metrics.IncrementFailed();
                _logger.LogError(ex, "Error processing message {Id}", message.Id);
            }
            finally
            {
                _throttler.Release();
            }
        }




        public async Task<PaginatedResult<Message>> GetMessagesAsync(int pageNumber, int pageSize)
        {
            return await _repository.GetMessagesPagedAsync(pageNumber, pageSize);
        }


        public async Task<IEnumerable<MessageStatistics>> GetGroupedStatisticsAsync() =>       
             await _repository.GetGroupedStatsAsync();
        

    }

}