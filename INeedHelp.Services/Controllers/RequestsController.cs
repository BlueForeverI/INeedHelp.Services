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
    public class RequestsController : ApiController
    {
        private HelpRequestsPersister requestsPersister;
        private UsersPersister usersPersister;

        public RequestsController()
        {
            DatabaseContext context = new DatabaseContext();
            requestsPersister = new HelpRequestsPersister(context);
            usersPersister = new UsersPersister(context);
        }

        [HttpGet]
        [ActionName("all")]
        public HttpResponseMessage GetAllRequests(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            if(usersPersister.GetBySessionKey(sessionKey) == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            IEnumerable<HelpRequestModel> helpRequests = requestsPersister.GetAll()
                .Select(r => HelpRequestModel.FromHelpRequest(r, false)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, helpRequests);
        }

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage AddRequest([FromBody]HelpRequest request,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if(sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            requestsPersister.Add(sender, request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("comment")]
        public HttpResponseMessage AddComment(int id, [FromBody]Comment comment,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            requestsPersister.AddComment(id, comment, sender);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpGet]
        [ActionName("addhelper")]
        public HttpResponseMessage AddHelper(int requestId, int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            try
            {
                requestsPersister.AddHelper(requestId, id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Cannot add helper");
            }
        }

        [HttpGet]
        [ActionName("byuser")]
        public HttpResponseMessage GetRequestsByUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var requests = requestsPersister.GetByUser(sender)
                .Select(r => HelpRequestModel.FromHelpRequest(r, false));
            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpGet]
        [ActionName("byid")]
        public HttpResponseMessage GetRequestById(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            HelpRequestModel request = 
                HelpRequestModel.FromHelpRequest(requestsPersister.GetById(id));
            return Request.CreateResponse(HttpStatusCode.OK, request);
        }

        [HttpGet]
        [ActionName("solve")]
        public HttpResponseMessage MarkRequestSolved(int id,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            try
            {
                requestsPersister.MarkSolved(id, sender);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpPost]
        [ActionName("edit")]
        public HttpResponseMessage EditRequest([FromBody]HelpRequest request,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            requestsPersister.EditRequest(request);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [HttpPost]
        [ActionName("near")]
        public HttpResponseMessage GetRequestNearPoint(int id, [FromBody]Coordinates point,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            double maxDistance = id;
            var requests = requestsPersister.GetRequestsNearPoint(point, maxDistance)
                .Select(r => HelpRequestModel.FromHelpRequest(r, false));

            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }

        [HttpPost]
        [ActionName("search")]
        public HttpResponseMessage SearchRequests([FromBody]QueryModel query,
            [ValueProvider(typeof(HeaderValueProviderFactory<String>))] String sessionKey)
        {
            var sender = usersPersister.GetBySessionKey(sessionKey);
            if (sender == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid session key");
            }

            var requests = requestsPersister.Search(query.Text)
                .Select(r => HelpRequestModel.FromHelpRequest(r, false)).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, requests);
        }
    }
}
