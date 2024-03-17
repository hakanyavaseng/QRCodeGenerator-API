using QRCodeGenerator.Creator.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QRCodeGenerator.Creator.Concretes
{
    public class MessageService : IMessageService
    {
        QrCodeService service = new();

        public IConnection CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                Uri = new Uri("amqp://localhost")
            };

            return factory.CreateConnection();
        }

        public void ListenQueue()
        {
            IConnection connection = CreateConnection();
            IModel channel = connection.CreateModel();

            string requestQueueName = "qr-code-queue-request";

            channel.QueueDeclare(
                queue: requestQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            EventingBasicConsumer consumer = new(channel);

            channel.BasicConsume(
                queue: requestQueueName,
                autoAck: true,
                consumer: consumer);

            consumer.Received += (sender, e) =>
            {
                string message = Encoding.UTF8.GetString(e.Body.ToArray());
                Console.WriteLine($"QRCode generating for: {message}");

                byte[] qrCode = service.CreateQrCode(message);


                #region Response to Publisher
             

                IBasicProperties properties = channel.CreateBasicProperties();
                properties.CorrelationId = e.BasicProperties.CorrelationId;

                channel.BasicPublish(
                    exchange: string.Empty,
                    routingKey: e.BasicProperties.ReplyTo,
                    basicProperties: properties,
                    body: qrCode);
                #endregion
            };
        }
    }
}
