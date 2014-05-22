using ServiceStack.Common.Web;
using ServiceStack.IO;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost;
using ServiceStack.WebHost.Endpoints.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web;
using HttpRequestWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpResponseWrapper;

namespace FluentMigrator.ServiceStack
{
    public sealed class EmbeddedFileHandler : IHttpHandler, IServiceStackHttpHandler
    {
        private string _filePath;
        private Assembly _assembly = Assembly.GetExecutingAssembly();

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File to serve up</param>
        public EmbeddedFileHandler(string file)
        {
            _filePath = file;
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(
                new HttpRequestWrapper(context.Request),
                new HttpResponseWrapper(context.Response),
                null);
        }

        public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
        {
            var resourceName = string.Concat(_assembly.GetName().Name, ".", _filePath.Replace('/', '.'));

            response.ContentType = MimeTypes.GetMimeType(_filePath);

            using(var stream = _assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
                else
                {
                    response.SetContentLength(stream.Length);
                    stream.CopyTo(response.OutputStream);
                    response.OutputStream.Flush();
                }
            }
        }
    }
}