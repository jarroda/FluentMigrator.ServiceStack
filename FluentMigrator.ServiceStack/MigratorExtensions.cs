using ServiceStack.ServiceHost;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace FluentMigrator.ServiceStack
{
    public static class MigratorExtensions
    {
        public static TextWriter GetChunkedWriter(this IHttpResponse response)
        {
            if (response.OriginalResponse is HttpResponse)
            {
                return ((HttpResponse)response.OriginalResponse).GetChunkedWriter();
            }
            if(response.OriginalResponse is HttpResponseBase)
            {
                return ((HttpResponseBase)response.OriginalResponse).GetChunkedWriter();
            }
            if(response.OriginalResponse is HttpListenerResponse)
            {
                return ((HttpListenerResponse)response.OriginalResponse).GetChunkedWriter();
            }

            throw new InvalidOperationException("Unknown response type.");
        }

        public static TextWriter GetChunkedWriter(this HttpListenerResponse response)
        {
            response.SendChunked = true;
            return new StreamWriter(response.OutputStream) { AutoFlush = true };
        }

        public static TextWriter GetChunkedWriter(this HttpResponseBase response)
        {
            response.Clear();
            response.BufferOutput = false;
            return response.Output;
        }

        public static TextWriter GetChunkedWriter(this HttpResponse response)
        {
            response.Clear();
            response.BufferOutput = false;
            return response.Output;
        }
    }
}