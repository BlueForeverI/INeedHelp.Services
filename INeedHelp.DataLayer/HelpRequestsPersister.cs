using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using INeedHelp.Models;

namespace INeedHelp.DataLayer
{
    public class HelpRequestsPersister
    {
        private DatabaseContext dbContext;
        private UsersPersister usersPersister;

        public HelpRequestsPersister(DatabaseContext context)
        {
            this.dbContext = context;
            this.usersPersister = new UsersPersister(context);
        }

        public IEnumerable<HelpRequest> GetAll()
        {
            return dbContext.HelpRequests.ToList();
        }

        public void Add(User sender, HelpRequest request)
        {
            var user = dbContext.Users.Attach(sender);
            user.HelpRequests.Add(request);
            dbContext.SaveChanges();
        }

        public void AddComment(int id, Comment comment, User sender)
        {
            var request = dbContext.HelpRequests.Find(id);
            var user = dbContext.Users.Attach(sender);
            comment.User = user;
            request.Comments.Add(comment);
            dbContext.SaveChanges();
        }

        public void AddHelper(int requestId, int userId)
        {
            var request = dbContext.HelpRequests.Find(requestId);
            var user = dbContext.Users.Find(userId);
            request.Helpers.Add(user);
            dbContext.SaveChanges();
        }

        public IEnumerable<HelpRequest> GetByUser(User sender)
        {
            return dbContext.HelpRequests.Where(r => r.User.Id == sender.Id).ToList();
        }

        public void MarkSolved(int requestId, User user)
        {
            var request = dbContext.HelpRequests.Find(requestId);
            if(request.User.Id != user.Id)
            {
                throw new Exception("Invalid user");
            }

            request.Solved = true;
            dbContext.SaveChanges();
        }

        public HelpRequest GetById(int id)
        {
            return dbContext.HelpRequests.Find(id);
        }
    }
}
