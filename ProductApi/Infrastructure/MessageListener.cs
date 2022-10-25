﻿using System;
using System.Collections.Generic;
using System.Threading;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using ProductApi.Data;
using ProductApi.Models;
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
                bus.PubSub.Subscribe<OrderCreatedMessage>("productApiHkCreated", 
                    HandleOrderCreated);

               // bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkCompleted",
                //    HandleOrderCompleted, x => x.WithTopic("completed"));

              //  bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkCancelled",
                  //  HandleOrderCancelled, x => x.WithTopic("cacelled"));

              //  bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkShipped",
                  //  HandleOrderCancelled, x => x.WithTopic("shipped"));

              //  bus.PubSub.Subscribe<OrderStatusChangedMessage>("productApiHkPaid",
                  //  HandleOrderCancelled, x => x.WithTopic("paid"));


                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

      /*  private void HandleOrderCompleted(OrderStatusChangedMessage message)
        {
            // A service scope is created to get an instance of the product repository.
            // When the service scope is disposed, the product repository instance will
            // also be disposed.
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<Product>>();

                // Reserve items of ordered product (should be a single transaction).
                // Beware that this operation is not idempotent.
                foreach (var orderLine in message.OrderLines)
                {
                    var product = productRepos.Get(orderLine.ProductId);
                    product.ItemsReserved += orderLine.Quantity;
                    productRepos.Edit(product);
                }
            }
        }
      */
     
      
        private void HandleOrderCreated(OrderCreatedMessage message)
        {
            // A service scope is created to get an instance of the product repository.
            // When the service scope is disposed, the product repository instance will
            // also be disposed.
            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var productRepos = services.GetService<IRepository<Product>>();

                if (ProductItemsAvailable(message.OrderLines, productRepos))
                {
                    // Reserve items and publish an OrderAcceptedMessage
                    foreach (var orderLine in message.OrderLines)
                    {
                        var product = productRepos.Get(orderLine.ProductId);
                        product.ItemsReserved += orderLine.Quantity;
                        productRepos.Edit(product);
                    }

                    var replyMessage = new OrderAcceptedMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
                else
                {
                    // Publish an OrderRejectedMessage
                    var replyMessage = new OrderRejectedMessage
                    {
                        OrderId = message.OrderId
                    };

                    bus.PubSub.Publish(replyMessage);
                }
            }
        }

        private bool ProductItemsAvailable(IList<OrderLine> orderLines, IRepository<Product> productRepos)
        {
            foreach (var orderLine in orderLines)
            {
                var product = productRepos.Get(orderLine.ProductId);
                if (orderLine.Quantity > product.ItemsInStock - product.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }


    }
}
