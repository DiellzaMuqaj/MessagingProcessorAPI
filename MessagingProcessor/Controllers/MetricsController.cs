using MessagingProcessor.Services;
using Microsoft.AspNetCore.Mvc;

namespace MessagingProcessor.Controllers
{
 
        [ApiController]
        [Route("api/[controller]")]
        public class MetricsController : ControllerBase
        {
            private readonly IMetricsService _metrics;

            public MetricsController(IMetricsService metrics)
            {
                _metrics = metrics;
            }

            [HttpGet("summary")]
            public IActionResult GetMetricsSummary()
            {
                return Ok(new
                {
                    ThroughputLast10s = _metrics.GetThroughput(),
                    QueueDepth = _metrics.GetQueueDepth(),
                    ErrorRate = _metrics.GetErrorRate(),
                });
            }
        }

    
}
