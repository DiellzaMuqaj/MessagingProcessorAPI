using MessagingProcessor.DTO;
using MessagingProcessor.Models;

namespace MessagingProcessor.Persistence
{
    public interface IMessageRepository
    {
        Task AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task UpdateMessageStatusAsync(Guid id, MessageStatus status);
        Task<PaginatedResult<Message>> GetMessagesPagedAsync(int pageNumber, int pageSize);
        Task<IEnumerable<MessageStatistics>> GetGroupedStatsAsync();
        Task<IEnumerable<Message>> GetUnprocessedMessagesAsync();

    }
}
