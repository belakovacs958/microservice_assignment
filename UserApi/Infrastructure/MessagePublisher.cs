using System;
using System.Collections.Generic;
using EasyNetQ;
using SharedModels;

namespace UserApi.Infrastructure
{
    public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishOrderRequestMessage(int? customerId, int orderId, IList<OrderLine> orderLines)
        {
            var message = new OrderRequestMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines
            };

            bus.PubSub.Publish(message);
        }
    }
}

