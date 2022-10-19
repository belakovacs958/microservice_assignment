using System.Collections.Generic;
using System.Linq;
using UserApi.Models;
using System;

namespace UserApi.Data
{
    public class DbInitializer : IDbInitializer
    {
        // This method will create and seed the database.
        public void Initialize(UserApiContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

         
            if (context.Users.Any())
            {
                return;   
            }

            List<User> users = new List<User>
            {
                new User { Id = 1, Name = "First Customer", Email = "test@mail.dk", Phone = 12345678, BillingAddress = "20 Test Street", ShippingAddress = "21 Test Street", CreditStanding = "test" }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
