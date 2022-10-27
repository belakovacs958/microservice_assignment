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
            Console.WriteLine("PublishOrderCreatedMessage called from MessagePublisher order api");
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

        public void PublishOrderShippedMessage(int customerId, int orderId,
            IList<OrderLine> orderLines)
        {
            var message = new OrderShippedMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines
            };

            bus.PubSub.Publish(message);

        }

        public void PublishOrderCancelledMessage(int customerId, int orderId, IList<OrderLine> orderLines)
        {
            var message = new OrderCancelledMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines
            };

            bus.PubSub.Publish(message);
        }

        public void PublishOrderPaidMessage(int customerId, int orderId, IList<OrderLine> orderLines)
        {
            var message = new OrderPaidMessage
            {
                CustomerId = customerId,
                OrderId = orderId,
                OrderLines = orderLines

            };

            bus.PubSub.Publish(message);

        }
    }
}
