using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INeedHelp.Services.Models
{
    public class UsedEditModel
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string OldPasswordHash { get; set; }

        public string ProfilePictureUrl { get; set; }

        public string NewPasswordHash { get; set; }
    }
}