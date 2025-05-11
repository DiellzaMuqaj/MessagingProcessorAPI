using MessagingProcessor.DTO;
using MessagingProcessor.Models;

namespace MessagingProcessor.Services
{
    public interface IMessageService
    {
        Task SubmitMessageAsync(Message message);
        Task<PaginatedResult<Message>> GetMessagesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<MessageStatistics>> GetGroupedStatisticsAsync();
    }

}
