using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using UserApi.Models;
using System;
using UserApi.Data;


namespace UserApi.Data
{
    public class UserRepository : IRepository<User>
    {
        private readonly UserApiContext db;

        public UserRepository(UserApiContext context)
        {
            db = context;
        }

        User IRepository<User>.Add(User entity)
        {
            var newUser = db.Users.Add(entity).Entity;
            db.SaveChanges();
            return newUser;
        }

        void IRepository<User>.Edit(User entity)
        {
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }

        User IRepository<User>.Get(int id)
        {
            return db.Users.FirstOrDefault(o => o.Id == id);
        }

        void IRepository<User>.Remove(int id)
        {
            var user = db.Users.FirstOrDefault(p => p.Id == id);
            db.Users.Remove(user);
            db.SaveChanges();
        }
    }
}


