using System.Collections.Generic;
using System.Linq;
using CustomerApi.Models;
using System;

namespace CustomerApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(CustomerApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Look for any Products
            if (context.Customers.Any())
            {
                return;   // DB has been seeded
            }

            List<Customer> customers = new List<Customer>
            {
                new Customer { Id = 1, Name = "First Customer", Email = "test@mail.dk", Phone = 12345678, BillingAddress = "20 Test Street", ShippingAddress = "21 Test Street", CreditStanding = "test" }
            };

            context.Customers.AddRange(customers);
            context.SaveChanges();
        }
    }
}
