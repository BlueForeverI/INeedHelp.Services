using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INeedHelp.Models
{
    public class HelpRequest
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string PictureUrl { get; set; }
        public bool Solved { get; set; }

        public virtual User User { get; set; }
        public virtual Coordinates Coordinates { get; set; }
        public virtual ICollection<User> Helpers { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
 
        public HelpRequest()
        {
            this.Helpers = new HashSet<User>();
            this.Comments = new HashSet<Comment>();
        }
    }
}
