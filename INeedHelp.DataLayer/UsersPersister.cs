using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INeedHelp.Models;

namespace INeedHelp.DataLayer
{
    public class UsersPersister
    {
        private DatabaseContext dbContext;

        public UsersPersister(DatabaseContext context)
        {
            this.dbContext = context;
        }

        public void SetSessionKey(User user, string sessionKey)
        {
            dbContext.Users.Attach(user);
            user.SessionKey = sessionKey;
            dbContext.SaveChanges();
        }

        public User GetByUsername(string username)
        {
            return dbContext.Users.FirstOrDefault(u => u.Username == username);
        }

        public void Add(User user)
        {
            dbContext.Users.Add(user);
            dbContext.SaveChanges();
        }

        public User CheckLogin(string username, string passwordHash)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user;
        }

        public User GetBySessionKey(string sessionKey)
        {
            return dbContext.Users.FirstOrDefault(u => u.SessionKey == sessionKey);
        }

        public void Logout(User user)
        {
            dbContext.Users.Attach(user);
            user.SessionKey = null;
            dbContext.SaveChanges();
        }
    }
}
