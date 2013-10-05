using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INeedHelp.Models;

namespace INeedHelp.Services.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string SessionKey { get; set; }
        
        public int Reputation { get; set; }

        public static UserModel FromUser(User user)
        {
            return new UserModel()
                       {
                           Id = user.Id,
                           FirstName = user.FirstName,
                           LastName = user.LastName,
                           Username = user.Username,
                           ProfilePictureUrl = user.ProfilePictureUrl
                       };
        }
    }
}
