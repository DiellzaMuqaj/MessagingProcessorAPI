using Dapper;
using MessagingProcessor.DTO;
using MessagingProcessor.Models;
using MessagingProcessor.Persistence;
using Microsoft.Data.Sqlite;
using System.Data;

public class MessageRepository : IMessageRepository
{
    private readonly string _connectionString;

    public MessageRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");
    }

    private IDbConnection CreateConnection()
    {
        var connection = new SqliteConnection(_connectionString);
        connection.Open(); 
        return connection;
    }

    public async Task AddMessageAsync(Message message)
    {
        const string sql = @"
    INSERT INTO Messages (Id, Type, Recipient, Content, Status, CreatedAt, RetryCount, Priority)
    VALUES (@Id, @Type, @Recipient, @Content, @Status, @CreatedAt, @RetryCount, @Priority)";

        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, message);
    }

    public async Task UpdateMessageAsync(Message message)
    {
        const string sql = @"UPDATE Messages SET 
                             Type = @Type,
                             Recipient = @Recipient,
                             Content = @Content,
                             Status = @Status,
                             RetryCount = @RetryCount,
                             Priority=@Priority
                             WHERE Id = @Id";
        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, message);
    }

    public async Task UpdateMessageStatusAsync(Guid id, MessageStatus status)
    {
        const string sql = @"UPDATE Messages SET Status = @Status WHERE Id = @Id";
        using var conn = CreateConnection();
        await conn.ExecuteAsync(sql, new { Id = id, Status = status });
    }

    public async Task<PaginatedResult<Message>> GetMessagesPagedAsync(int pageNumber, int pageSize)
    {
        using var conn = CreateConnection();

        var query = @"SELECT * FROM Messages ORDER BY CreatedAt DESC LIMIT @PageSize OFFSET @Offset;";
        var countQuery = @"SELECT COUNT(*) FROM Messages;";

        var messages = await conn.QueryAsync<Message>(query, new
        {
            PageSize = pageSize,
            Offset = (pageNumber - 1) * pageSize
        });

        var totalCount = await conn.ExecuteScalarAsync<int>(countQuery);

        return new PaginatedResult<Message>
        {
            Items = messages,
            TotalCount = totalCount
        };
    }

    public async Task<IEnumerable<MessageStatistics>> GetGroupedStatsAsync()
    {
        const string sql = @"
        SELECT Type, Status, COUNT(*) as Count
        FROM Messages
        GROUP BY Type, Status";
        using var conn = CreateConnection();
        return await conn.QueryAsync<MessageStatistics>(sql);
    }

    public async Task<IEnumerable<Message>> GetUnprocessedMessagesAsync()
    {
        const string sql = @"
        SELECT * FROM Messages 
        WHERE Status = @Pending OR Status = @Processing OR Status = @Retried OR Status = @DeadLetter";
        using var conn = CreateConnection();
        return await conn.QueryAsync<Message>(sql, new
        {
            MessageStatus.Pending,
            MessageStatus.Processing,
            MessageStatus.Retried,
            MessageStatus.DeadLetter
        });
    }
}
