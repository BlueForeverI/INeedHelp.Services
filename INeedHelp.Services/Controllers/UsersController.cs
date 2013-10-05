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
                ProfilePictureUrl = value.ProfilePictureUrl,
                Reputation = value.Reputation
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

        [HttpPost]
        [ActionName("edit")]
        public HttpResponseMessage EditUser([FromBody]UsedEditModel value,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var userToEdit = new User()
            {
                Id = user.Id,
                Username = user.Username,
                FirstName = value.FirstName,
                LastName = value.LastName,
                PasswordHash = value.OldPasswordHash,
                ProfilePictureUrl = value.ProfilePictureUrl
            };

            if (usersPersister.EditUser(userToEdit, value.NewPasswordHash))
            {
                var updatedUser = usersPersister.Get(userToEdit.Id);
                var userModel = new UserModel()
                {
                    Id = updatedUser.Id,
                    Username = updatedUser.Username,
                    SessionKey = sessionKey,
                    FirstName = updatedUser.FirstName,
                    LastName = updatedUser.LastName,
                    ProfilePictureUrl = updatedUser.ProfilePictureUrl
                };

                return Request.CreateResponse(HttpStatusCode.OK, userModel);
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest, "Could not edit user");
        }

        [HttpGet]
        [ActionName("session")]
        public HttpResponseMessage GetUserBySessionKey(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var user = usersPersister.GetBySessionKey(sessionKey);
            if (user == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var userModel = new UserModel()
                                {
                                    Id = user.Id,
                                    FirstName = user.FirstName,
                                    LastName = user.LastName,
                                    ProfilePictureUrl = user.ProfilePictureUrl,
                                    SessionKey = user.SessionKey,
                                    Username = user.Username
                                };

            return Request.CreateResponse(HttpStatusCode.OK, userModel);
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
