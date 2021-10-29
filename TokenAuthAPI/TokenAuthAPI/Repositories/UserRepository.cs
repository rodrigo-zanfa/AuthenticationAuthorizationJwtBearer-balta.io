using TokenAuthAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAuthAPI.Repositories
{
    public static class UserRepository
    {
        public static User Get(string username, string password)
        {
            //var users = new List<User>();
            //users.Add(new User { Id = 1, Username = "rodrigo", Password = "rodrigo123", Email = "rodrigo@email.com", Role = "owner" });
            //users.Add(new User { Id = 3, Username = "raphaela", Password = "raphaela123", Email = "raphaela@email.com", Role = "manager" });
            //users.Add(new User { Id = 3, Username = "guilherme", Password = "guilherme123", Email = "guilherme@email.com", Role = "employee" });

            var users = new List<User>
            {
                new User { Id = 1, Username = "rodrigo", Password = "rodrigo123", Email = "rodrigo@email.com", Role = "owner" },
                new User { Id = 3, Username = "raphaela", Password = "raphaela123", Email = "raphaela@email.com", Role = "manager" },
                new User { Id = 3, Username = "guilherme", Password = "guilherme123", Email = "guilherme@email.com", Role = "employee" }
            };

            return users.Where(x => x.Username.ToLower() == username.ToLower() && x.Password == password).FirstOrDefault();
        }
    }
}
