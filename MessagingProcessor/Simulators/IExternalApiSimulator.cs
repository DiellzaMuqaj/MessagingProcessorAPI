using MessagingProcessor.Models;

namespace MessagingProcessor.Simulators
{
    public interface IExternalApiSimulator
    {
        Task<bool> SendAsync(Message message);
    }
}
