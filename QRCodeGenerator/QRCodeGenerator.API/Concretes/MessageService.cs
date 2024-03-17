using QRCodeGenerator.API.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;

namespace QRCodeGenerator.API.Concretes
{
    public class MessageService : IMessageService
    {
        public async Task<IConnection> CreateConnection()
        {
            ConnectionFactory factory = new ConnectionFactory();
            factory.Uri = new Uri("amqp://localhost");
            return await Task.Run(() => factory.CreateConnection());
        }

        public async Task<byte[]> SendMessage(string message)
        {
            IConnection connection = await CreateConnection();
            IModel channel = connection.CreateModel();
            byte[] responseQrCodeBytes = null;

            string requestQueueName = "qr-code-queue-request";

            channel.QueueDeclare(
                queue: requestQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            string responseQueueName = channel.QueueDeclare().QueueName;

            string correlationId = Guid.NewGuid().ToString();

            IBasicProperties properties = channel.CreateBasicProperties();
            properties.CorrelationId = correlationId;
            properties.ReplyTo = responseQueueName;

            byte[] requestMessage = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(
                exchange: "",
                routingKey: requestQueueName,
                basicProperties: properties,
                body: requestMessage);

            // Setup a task completion source to await the response
            TaskCompletionSource<byte[]> tcs = new TaskCompletionSource<byte[]>();

            // Listen for the response
            EventingBasicConsumer consumer = new(channel);
            consumer.Received += (sender, e) =>
            {
                if (e.BasicProperties.CorrelationId == correlationId)
                {
                    responseQrCodeBytes = e.Body.ToArray();
                    tcs.SetResult(responseQrCodeBytes); // Set the result when the response is received
                }
            };

            channel.BasicConsume(
                queue: responseQueueName,
                autoAck: true,
                consumer: consumer);

            // Wait for the response
            await Task.Run(() => tcs.Task);

            return responseQrCodeBytes;
        }
    }
}
