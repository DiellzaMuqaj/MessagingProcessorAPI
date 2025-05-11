using System.Data;
using Dapper;

namespace MessagingProcessor.Persistence
{
    public class DatabaseInitializer
    {
        public static void Initialize(IDbConnection dbConnection)
        {
             const string sql = @"
               CREATE TABLE IF NOT EXISTS Messages (
               Id TEXT PRIMARY KEY,
               Type INTEGER NOT NULL,
               Recipient TEXT NOT NULL,
               Content TEXT NOT NULL,
               Status INTEGER NOT NULL,
               CreatedAt TEXT NOT NULL,
               RetryCount INTEGER NOT NULL,
               Priority INTEGER NOT NULL DEFAULT 2
             )";

            dbConnection.Execute(sql);
        }
    }
}
