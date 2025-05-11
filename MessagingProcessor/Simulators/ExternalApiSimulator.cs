using MessagingProcessor.Models;

namespace MessagingProcessor.Simulators
{
    public class ExternalApiSimulator : IExternalApiSimulator
    {
        private static readonly Random _random = new();

        public async Task<bool> SendAsync(Message message)
        {
            await Task.Delay(_random.Next(500, 2000)); // simulate delay

            return _random.NextDouble() >= 0.5; // 50% chance of success

        }
    }
}
