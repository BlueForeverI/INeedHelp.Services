using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INeedHelp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string SessionKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ProfilePictureUrl { get; set; }
        public int Reputation { get; set; }

        public virtual ICollection<User> Friends { get; set; }
        public virtual ICollection<FriendRequest> FriendRequests { get; set; }
        public virtual ICollection<HelpRequest> HelpRequests { get; set; } 

        public User()
        {
            this.Friends = new HashSet<User>();
            this.FriendRequests = new HashSet<FriendRequest>();
            this.HelpRequests = new HashSet<HelpRequest>();
        }
    }
}
