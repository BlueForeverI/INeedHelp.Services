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
        public string Title { get; set; }
        public string Text { get; set; }
        public string PictureUrl { get; set; }
        public bool Solved { get; set; }
        public Coordinates Coordinates { get; set; }

        public UserModel User { get; set; }
        public IEnumerable<UserModel> Helpers { get; set; }
        public IEnumerable<CommentModel> Comments { get; set; }

        public static HelpRequestModel FromHelpRequest(HelpRequest r, bool withComments = true)
        {
            return new HelpRequestModel()
                       {
                           Id = r.Id,
                           Title = r.Title,
                           Text = r.Text,
                           User = UserModel.FromUser(r.User),
                           PictureUrl = r.PictureUrl,
                           Coordinates = r.Coordinates,
                           Solved = r.Solved,
                           Comments = (withComments) 
                           ? r.Comments.Select(c => new CommentModel()
                            {
                                Content = c.Content,
                                User = UserModel.FromUser(c.User)
                            }) 
                           : new List<CommentModel>(),
                           Helpers = r.Helpers.Select(u => UserModel.FromUser(u))
                       };
        }
    }
}