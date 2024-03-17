using RabbitMQ.Client;

namespace QRCodeGenerator.Creator.Interfaces
{
    public interface IMessageService
    {
        //RabbitMQ Operations
        IConnection CreateConnection();
        void ListenQueue();
    }
}
