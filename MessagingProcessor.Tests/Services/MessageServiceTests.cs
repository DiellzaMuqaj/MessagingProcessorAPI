using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MessagingProcessor.DTO;
using MessagingProcessor.Models;
using MessagingProcessor.Persistence;
using MessagingProcessor.Queue;
using MessagingProcessor.Services;
using MessagingProcessor.Simulators;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace MessagingProcessor.Tests.Services
{
    public class MessageServiceTests
    {
        private readonly Mock<IMessageQueue> _mockQueue = new();
        private readonly Mock<IMessageRepository> _mockRepo = new();
        private readonly Mock<IExternalApiSimulator> _mockApi = new();
        private readonly Mock<ILogger<MessageService>> _mockLogger = new();
        private readonly Mock<IMetricsService> _mockMetrics = new();
        private MessageService CreateService()
        {
            return new MessageService(
                _mockQueue.Object,
                _mockRepo.Object,
                _mockApi.Object,
                _mockLogger.Object,
                _mockMetrics.Object
            );
        }

        [Fact]
        public async Task SubmitMessageAsync_ShouldSaveAndEnqueueMessage()
        {
            var message = new Message
            {
                Type = MessageType.Email,
                Recipient = "test@example.com",
                Content = "Hello"
            };

            var service = CreateService();

            await service.SubmitMessageAsync(message);

            _mockRepo.Verify(r => r.AddMessageAsync(It.Is<Message>(m =>
                m.Recipient == "test@example.com" &&
                m.Content == "Hello" &&
                m.Type == MessageType.Email &&
                m.Status == MessageStatus.Pending
            )), Times.Once);

            _mockQueue.Verify(q => q.Enqueue(It.IsAny<Message>()), Times.Once);
        }



        [Fact]
        public async Task RecoverUnprocessedMessages_ShouldEnqueueEachPendingMessage()
        {
            var recoveredMessages = new List<Message>
    {
        new Message
        {
            Id = Guid.NewGuid(),
            Status = MessageStatus.Pending,
            Recipient = "a",
            Content = "x",
            Type = MessageType.SMS,
            CreatedAt = DateTime.Now
        },
        new Message
        {
            Id = Guid.NewGuid(),
            Status = MessageStatus.Processing,
            Recipient = "b",
            Content = "y",
            Type = MessageType.Email,
            CreatedAt = DateTime.Now
        }
    };

            _mockRepo.Setup(r => r.GetUnprocessedMessagesAsync())
                     .ReturnsAsync(recoveredMessages);

            var service = CreateService();
            _mockQueue.Invocations.Clear(); 

            await service.RecoverUnprocessedMessagesAsync();

            _mockQueue.Verify(q => q.Enqueue(It.IsAny<Message>()), Times.Exactly(recoveredMessages.Count));

        }


        [Fact]
        public async Task GetGroupedStatisticsAsync_ShouldReturnStatsFromRepository()
        {
            var stats = new List<MessageStatistics>
            {
                new MessageStatistics { Status = MessageStatus.Sent, Count = 5 },
                new MessageStatistics { Status = MessageStatus.Failed, Count = 2 }
            };

            _mockRepo.Setup(r => r.GetGroupedStatsAsync())
                     .ReturnsAsync(stats);

            var service = CreateService();

            var result = await service.GetGroupedStatisticsAsync();

            Assert.Collection(result,
                item => Assert.Equal(5, item.Count),
                item => Assert.Equal(2, item.Count));
        }

        [Fact]
        public async Task GetMessagesAsync_ShouldReturnPagedMessagesFromRepository()
        {
            // Arrange
            var expectedMessages = new List<Message>
    {
        new Message { Id = Guid.NewGuid(), Recipient = "a", Content = "x", Type = MessageType.SMS, CreatedAt = DateTime.Now },
        new Message { Id = Guid.NewGuid(), Recipient = "b", Content = "y", Type = MessageType.Email, CreatedAt = DateTime.Now }
    };

            var paginatedResult = new PaginatedResult<Message>
            {
                Items = expectedMessages,
                TotalCount = 2
            };

            _mockRepo.Setup(r => r.GetMessagesPagedAsync(1, 10))
                     .ReturnsAsync(paginatedResult);

            var service = CreateService();

            // Act
            var result = await service.GetMessagesAsync(1, 10);

            // Assert
            Assert.Equal(2, result.Items.Count());
            Assert.Equal(2, result.TotalCount);
        }


    }
}
