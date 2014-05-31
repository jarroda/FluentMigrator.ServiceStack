#if V3
using ServiceStack.Common.Web;
using ServiceStack.WebHost.Endpoints.Extensions;
using IServiceStackHandler = ServiceStack.WebHost.Endpoints.Support.IServiceStackHttpHandler;
using HttpRequestWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpResponseWrapper;
using IRequest = ServiceStack.ServiceHost.IHttpRequest;
using IResponse = ServiceStack.ServiceHost.IHttpResponse;
#else
using ServiceStack.Host.AspNet;
using ServiceStack.Host.Handlers;
using ServiceStack.Web;
using ServiceStack;
#endif
using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace FluentMigrator.ServiceStack
{
    public sealed class EmbeddedFileHandler : IHttpAsyncHandler, IServiceStackHandler
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
            var task = ProcessRequestAsync(context.Request.RequestContext.HttpContext);

            if (task.Status == TaskStatus.Created)
            {
                task.RunSynchronously();
            }
            else
            {
                task.Wait();
            }
        }

        public void ProcessRequest(IRequest request, IResponse response, string operationName)
        {
            var resourceName = _filePath.Replace('/', '.');
            response.ContentType = MimeTypes.GetMimeType(_filePath);

            using (var stream = _assembly.GetManifestResourceStream(typeof(MigrationFeature), resourceName))
            {
                if (stream == null)
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    response.Close();
                }
                else
                {
                    // TODO: Figure out the best way to enable browser caching of this static content.
                    response.SetContentLength(stream.Length);
                    stream.CopyTo(response.OutputStream);
                    response.OutputStream.Flush();
                }
            }
        }


        public Task ProcessRequestAsync(HttpContextBase context)
        {
#if V3
            var httpContext = context.ApplicationInstance.Context;
            return CreateProcessRequestTask(new HttpRequestWrapper(httpContext.Request), new HttpResponseWrapper(httpContext.Response), httpContext.Request.GetOperationName());
#else
            var operationName = context.Request.GetOperationName();
            var httpReq = new AspNetRequest(context, operationName);             
            return CreateProcessRequestTask(httpReq, httpReq.Response, operationName);
#endif
        }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            if (cb == null)
            {
                throw new ArgumentNullException("cb");
            }

            var task = ProcessRequestAsync(context.Request.RequestContext.HttpContext);

            task.ContinueWith(ar => cb(ar));

            if (task.Status == TaskStatus.Created)
            {
                task.Start();
            }

            return task;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task)result;
            task.Wait();
        }

        public Task ProcessRequestAsync(IRequest httpReq, IResponse httpRes, string operationName)
        {
            var task = CreateProcessRequestTask(httpReq, httpRes, operationName);
            task.Start();
            return task;
        }

        private Task CreateProcessRequestTask(IRequest httpReq, IResponse httpRes, string operationName)
        {
            return new Task(() => ProcessRequest(httpReq, httpRes, operationName));
        }
    }
}