using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INeedHelp.Services.Models
{
    public class CommentModel
    {
        public UserModel User { get; set; }
        public string Content { get; set; }
    }
}
