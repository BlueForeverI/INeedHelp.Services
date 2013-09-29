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

        public bool SendContactRequest(User sender, User receiver)
        {
            dbContext.Users.Attach(sender);
            dbContext.Users.Attach(receiver);

            if (receiver.FriendRequests.Any(c => c.Sender.Id == sender.Id))
            {
                return false;
            }

            receiver.FriendRequests.Add(new FriendRequest() { Sender = sender });
            dbContext.SaveChanges();
            return true;
        }

        public User Get(int id)
        {
            return dbContext.Users.Find(id);
        }

        public bool AcceptContactRequest(int requestId, User user)
        {
            var request = dbContext.FriendRequests.FirstOrDefault(c => c.Id == requestId);
            if (request == null)
            {
                return false;
            }

            dbContext.Users.Attach(user);
            if (!user.FriendRequests.Any(c => c.Id == requestId))
            {
                return false;
            }

            user.Friends.Add(request.Sender);
            request.Sender.Friends.Add(user);
            dbContext.SaveChanges();

            dbContext.FriendRequests.Remove(request);
            dbContext.SaveChanges();

            return true;
        }

        public bool DenyContactRequest(int requestId, User user)
        {
            var request = dbContext.FriendRequests.FirstOrDefault(c => c.Id == requestId);
            if (request == null)
            {
                return false;
            }

            dbContext.Users.Attach(user);
            if (!user.FriendRequests.Any(c => c.Id == requestId))
            {
                return false;
            }

            dbContext.FriendRequests.Remove(request);
            dbContext.SaveChanges();

            return true;
        }

        public bool EditUser(User userToEdit, string newPasswordHash)
        {
            var user = dbContext.Users.Find(userToEdit.Id);
            if (userToEdit.PasswordHash != null)
            {
                if (user.Username != userToEdit.Username || user.PasswordHash != userToEdit.PasswordHash)
                {
                    return false;
                }
            }

            user.FirstName = userToEdit.FirstName ?? user.FirstName;
            user.LastName = userToEdit.LastName ?? user.LastName;
            user.PasswordHash = (userToEdit.PasswordHash != null) ? newPasswordHash : user.PasswordHash;
            user.ProfilePictureUrl = userToEdit.ProfilePictureUrl ?? user.ProfilePictureUrl;
            dbContext.SaveChanges();
            return true;
        }
    }
}
