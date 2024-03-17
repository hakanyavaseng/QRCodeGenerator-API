using RabbitMQ.Client;

namespace QRCodeGenerator.API.Interfaces
{
    public interface IMessageService
    {
        Task<IConnection> CreateConnection();
        Task<byte[]> SendMessage(string message);

    }
}
