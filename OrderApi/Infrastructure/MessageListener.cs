using System;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using OrderApi.Data;
using SharedModels;

namespace OrderApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;
        IBus bus;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the order repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderAcceptedMessage>("orderApiHkAccepted",
                    HandleOrderAccepted);
                bus.PubSub.Subscribe<OrderCreatedMessage>("orderApiHkCreated",
                    HandleOrderCreated);
                bus.PubSub.Subscribe<OrderRejectedMessage>("orderApiHkRejected",
                    HandleOrderRejected);

                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderAccepted(OrderAcceptedMessage message)
        {
            // TODO has to be modified so doesnt automatically changes a created order to completed order

            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<Order>>();

                // Mark order as completed
                var order = orderRepos.Get(message.OrderId);
                order.Status = Order.OrderStatus.completed;
                orderRepos.Edit(order);
            }
        }

        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            // TODO has to be modified so doesnt automatically changes a created order to completed order

            Console.WriteLine("order id from handle order created : " +message.OrderId);
        }

        private void HandleOrderRejected(OrderRejectedMessage message)
        {
            Console.WriteLine("Listener ran");
            Console.WriteLine("order id: " + message.OrderId);

            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var orderRepos = services.GetService<IRepository<Order>>();

                // Delete tentative order.
                orderRepos.Remove(message.OrderId);
            }
        }
    }
}
