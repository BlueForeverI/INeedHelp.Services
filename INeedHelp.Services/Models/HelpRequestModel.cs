using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using INeedHelp.Models;

namespace INeedHelp.Services.Models
{
    public class HelpRequestModel
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string PictureUrl { get; set; }
        public bool Solved { get; set; }
        public Coordinates Coordinates { get; set; }

        public UserModel User { get; set; }
        public IEnumerable<UserModel> Helpers { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }

        public static HelpRequestModel FromHelpRequest(HelpRequest r)
        {
            return new HelpRequestModel()
                       {
                           Id = r.Id,
                           Text = r.Text,
                           User = UserModel.FromUser(r.User),
                           PictureUrl = r.PictureUrl,
                           Coordinates = r.Coordinates,
                           Solved = r.Solved,
                           Comments = r.Comments.Select(c => new CommentModel()
                                                                {
                                                                    Content = c.Content,
                                                                    User = UserModel.FromUser(c.User)
                                                                }),
                           Helpers = r.Helpers.Select(u => UserModel.FromUser(u))
                       };
        }
    }
}