using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace INeedHelp.Models
{
    public class FriendRequest
    {
        public int Id { get; set; }
        public virtual User User { get; set; }
        public virtual User Sender { get; set; }
    }
}
