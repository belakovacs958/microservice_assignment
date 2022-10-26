using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Xml.Linq;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using SharedModels;

namespace ProductApi.Infrastructure
{
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;
        IBus bus;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderCreatedMessage>("orderApiHkCreated", 
                    HandleOrderCreated);
                bus.PubSub.Subscribe<OrderRejectedMessage>("orderApiHkRejected", 
                    HandleOrderRejected);
                bus.PubSub.Subscribe<OrderPaidMessage>("orderApiHkPaid",
                    HandleOrderPaid);
                


                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderPaid(OrderPaidMessage message)
        {
            throw new NotImplementedException();
        }

        private void HandleOrderRejected(OrderRejectedMessage message)
        {
            Console.WriteLine("Your order has been rejected! The order number is: " + message.OrderId.ToString());

        }


        private void HandleOrderCreated(OrderCreatedMessage message)
        {
           
            Console.WriteLine("This is a confirmation of your order! The order number is: " + message.OrderId.ToString());

        }

    }
}
