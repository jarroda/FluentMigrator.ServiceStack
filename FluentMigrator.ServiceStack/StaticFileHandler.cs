using ServiceStack.IO;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints.Extensions;
using ServiceStack.WebHost.Endpoints.Support;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using HttpRequestWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpRequestWrapper;
using HttpResponseWrapper = ServiceStack.WebHost.Endpoints.Extensions.HttpResponseWrapper;
//using System.Web;

namespace FluentMigrator.ServiceStack
{
    public sealed class VirtualFileHandler : IHttpHandler, IServiceStackHttpHandler
    {
        private IVirtualFile _file;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File to serve up</param>
        public VirtualFileHandler(IVirtualFile file)
        {
            _file = file;
        }


        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            ProcessRequest(new HttpRequestWrapper(context.Request),
                           new HttpResponseWrapper(context.Response),
                           null);
        }   // eo ProcessRequest

        public void ProcessRequest(IHttpRequest request, IHttpResponse response, string operationName)
        {
            try
            {
                //response.ContentType = ALHEnvironment.GetMimeType(_file.Extension);
                
                using (Stream reader = _file.OpenRead())
                {
                    byte[] data = reader.ReadFully();
                    response.SetContentLength(data.Length);
                    response.OutputStream.Write(data, 0, data.Length);
                    response.OutputStream.Flush();
                }
            }
            catch (System.Net.HttpListenerException ex)
            {
                //Error: 1229 is "An operation was attempted on a nonexistent network connection"
                //This exception occures when http stream is terminated by the web browser.
                if (ex.ErrorCode == 1229)
                    return;
                throw;
            }
        }   // eo ProcessRequest
    }   // eo class VirtualFileHandler

    internal static class Extensions
    {
        public static byte[] ReadFully(this Stream stream)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] buffer = new byte[8192];
                int bytesRead;
                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                }
                return memoryStream.ToArray();
            }
        }
    }
}

