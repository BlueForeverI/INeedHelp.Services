using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INeedHelp.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string Content { get; set; }

        public virtual User User { get; set; }
        public virtual HelpRequest HelpRequest { get; set; }
    }
}
