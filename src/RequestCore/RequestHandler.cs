using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace RequestCore
{
    public class RequestHandler : IHttpService, IRequestHandler
    {
        private readonly Dictionary<string, HttpHandlerCollection> _methodHandlers;

        public RequestHandler()
        {
            _methodHandlers = new Dictionary<string, HttpHandlerCollection>
                              {
                                  { "GET", new HttpHandlerCollection() },
                                  { "POST", new HttpHandlerCollection()},
                                  { "DELETE", new HttpHandlerCollection()},
                                  { "PUT", new HttpHandlerCollection()},
                              };
        }

        public HttpHandlerCollection Delete => _methodHandlers["DELETE"];
        public HttpHandlerCollection Get => _methodHandlers["GET"];
        public HttpHandlerCollection Post => _methodHandlers["POST"];
        public HttpHandlerCollection Put => _methodHandlers["PUT"];

        public IHttpService Handles => this;

        public Func<object, string> ObjectCoverter { get; set; }

        public async Task<bool> HandleAsync(HttpContext context)
        {
            if (!_methodHandlers.ContainsKey(context.Request.Method))
            {
                return false;
            }

            var handlers = _methodHandlers[context.Request.Method];
            if (!handlers.ContainsRoute(context.Request.Path.ToString()))
            {
                return false;
            }

            var requestContent = await new StreamReader(context.Request.Body).ReadToEndAsync();
            var resposne = await handlers.InvokeHandler(context.Request.Path.ToString(), requestContent);
            if (resposne != null)
            {
                if (resposne is string) await context.Response.WriteAsync(resposne.ToString());
                else if (ObjectCoverter != null) await context.Response.WriteAsync(ObjectCoverter(resposne));
                else await context.Response.WriteAsync(resposne.ToString());
            }

            return true;
        }
    }
}
