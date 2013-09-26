using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using INeedHelp.DataLayer;
using INeedHelp.Models;
using INeedHelp.Services.Attributes;
using INeedHelp.Services.Models;

namespace INeedHelp.Services.Controllers
{
    public class FriendsController : ApiController
    {
        private UsersPersister usersPersister;

        public FriendsController()
        {
            var context = new DatabaseContext();
            this.usersPersister = new UsersPersister(context);
        }

        [HttpGet]
        [ActionName("all")]
        public HttpResponseMessage GetContacts(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var contacts = user.Friends.Select(u => new UserModel()
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                ProfilePictureUrl = u.ProfilePictureUrl
            });

            return Request.CreateResponse(HttpStatusCode.OK, contacts);
        }

        [HttpGet]
        [ActionName("add")]
        public HttpResponseMessage SendContactRequest(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            User receiver = usersPersister.Get(id);

            if (usersPersister.SendContactRequest(sender, receiver))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "You have already sent request to this person");
        }

        [HttpGet]
        [ActionName("accept")]
        public HttpResponseMessage AcceptContactRequest(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            if (usersPersister.AcceptContactRequest(id, user))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "Wrong contact request");
        }

        [HttpGet]
        [ActionName("deny")]
        public HttpResponseMessage DenyContactRequest(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            if (usersPersister.DenyContactRequest(id, user))
            {
                return Request.CreateResponse(HttpStatusCode.OK);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest,
                                          "Wrong contact request");
        }

        [HttpGet]
        [ActionName("requests")]
        public HttpResponseMessage GetAllContactRequests(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var requests = user.FriendRequests.Select(c => new FriendRequest()
            {
                Id = c.Id,
                Sender = new User()
                {
                    Id = c.Sender.Id,
                    Username = c.Sender.Username,
                    FirstName = c.Sender.FirstName,
                    LastName = c.Sender.LastName,
                    ProfilePictureUrl =
                    c.Sender.ProfilePictureUrl
                }
            });

            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }
    }
}
