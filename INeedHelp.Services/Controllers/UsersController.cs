using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using INeedHelp.DataLayer;
using INeedHelp.Models;
using INeedHelp.Services.Attributes;
using INeedHelp.Services.Models;

namespace INeedHelp.Services.Controllers
{
    public class UsersController : ApiController
    {
        private UsersPersister usersPersister;
        private const int SessionKeyLength = 50;
        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private static readonly Random rand = new Random();

        public UsersController()
        {
            DatabaseContext dbContext = new DatabaseContext();
            this.usersPersister = new UsersPersister(dbContext);
        }

        [ActionName("register")]
        public HttpResponseMessage Register([FromBody]User value)
        {
            if (string.IsNullOrEmpty(value.Username) || string.IsNullOrWhiteSpace(value.Username)
                || value.Username.Length < 5 || value.Username.Length > 30)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              "Invalid username. Should be between 5 and 30 characters");
            }

            if (usersPersister.GetByUsername(value.Username) != null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,
                                              "Username already exists");
            }

            usersPersister.Add(value);
            var sessionKey = GenerateSessionKey(value.Id);
            usersPersister.SetSessionKey(value, sessionKey);
            var userModel = new UserModel()
            {
                Id = value.Id,
                Username = value.Username,
                SessionKey = sessionKey,
                FirstName = value.Username,
                LastName = value.LastName,
                ProfilePictureUrl = value.ProfilePictureUrl
            };

            return Request.CreateResponse(HttpStatusCode.Created, userModel);   
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage Login([FromBody]User value)
        {
            User user = usersPersister.CheckLogin(value.Username, value.PasswordHash);
            if (user != null)
            {
                var sessionKey = GenerateSessionKey(user.Id);
                usersPersister.SetSessionKey(user, sessionKey);

                var userModel = new UserModel()
                {
                    Id = user.Id,
                    SessionKey = sessionKey,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    ProfilePictureUrl = user.ProfilePictureUrl
                };

                return Request.CreateResponse(HttpStatusCode.OK, userModel);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.Unauthorized, "Invalid username or password");
            }
        }

        [HttpGet]
        [ActionName("logout")]
        public HttpResponseMessage Logout(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            User user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            usersPersister.Logout(user);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }
    }
}
