using MessagingProcessor.DTO;
using MessagingProcessor.Models;
using MessagingProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingProcessor.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitMessage([FromBody] MessageRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Recipient) || string.IsNullOrWhiteSpace(request.Content))
                return BadRequest("Recipient and Content are required");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Type = request.Type,
                Recipient = request.Recipient,
                Content = request.Content,
                Status = MessageStatus.Pending,
                CreatedAt = DateTime.Now,
                RetryCount = 0,
                Priority = request.Priority
            };

            await _messageService.SubmitMessageAsync(message);
            return Accepted(new { message.Id });
        }


        [HttpGet("all")]
        public async Task<IActionResult> GetAllMessages([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _messageService.GetMessagesAsync(pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var stats = await _messageService.GetGroupedStatisticsAsync();
            return Ok(stats);
        }

    }
}
