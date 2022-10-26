using System;
using System.Collections.Generic;
using EasyNetQ;
using SharedModels;

namespace OrderApi.Infrastructure
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

        public void PublishOrderCreatedMessage(int? customerId, int orderId, IList<OrderLine> orderLines)
        {
            var message = new OrderCreatedMessage
            { 
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines 
            };

            bus.PubSub.Publish(message);
        }

        public void PublishOrderRejectedMessage(int customerId, int orderId)
        {
            var message = new OrderRejectedMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
            };

            bus.PubSub.Publish(message);
        }

    }
}
