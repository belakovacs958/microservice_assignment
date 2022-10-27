using System;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UserApi.Data;
using SharedModels;
using UserApi.Models;

namespace UserApi.Infrastructure
{
    
    public class MessageListener
    {
        IServiceProvider provider;
        string connectionString;
        IBus bus;

        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }
        //TODO implement listentning to user credit change
        public void Start()
        {
            using (bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderCreatedMessage>("orderApiHkAccepted",
                    HandleOrderCreated);

                bus.PubSub.Subscribe<OrderPaidMessage>("orderApiHkPaid",
                    HandleOrderPaid);
                
                //write subscribing functions 
                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            
            Console.WriteLine("handle order create called from user api");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<User>>();

                var user = productRepos.Get((int)message.CustomerId);

                user.CreditStanding = "BAD";
                productRepos.Edit(user);
       
            }
        }
        private void HandleOrderPaid(OrderPaidMessage message)
        {

            Console.WriteLine("handle order paid called from user api");
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<User>>();

                var user = productRepos.Get((int)message.CustomerId);

                user.CreditStanding = "GOOD";
                productRepos.Edit(user);
            }
        }
    }
}

