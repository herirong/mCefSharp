using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace mCefSharp.WinForms.tools
{
    public class file
    {
        public long UploadFile(string path, string url, string contentType= "form-data")
        {
            // Build request
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Post;
            request.AllowWriteStreamBuffering = false;
            request.ContentType = contentType;
            string fileName = Path.GetFileName(path);
            request.Headers["Content-Disposition"] = string.Format("attachment; filename=\"{0}\"", fileName);

            try
            {
                // Open source file
                using (var fileStream = File.OpenRead(path))
                {
                    // Set content length based on source file length
                    request.ContentLength = fileStream.Length;

                    // Get the request stream with the default timeout
                    using (var requestStream = WebRequestExtensions.GetRequestStreamWithTimeout(request, 30000))
                    {
                        // Upload the file with no timeout
                        fileStream.CopyTo(requestStream);
                    }
                }

                // Get response with the default timeout, and parse the response body
                using (var response = WebRequestExtensions.GetResponseWithTimeout(request, 30000))
                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream))
                {
                    string json = reader.ReadToEnd();
                    var j = JObject.Parse(json);
                    return j.Value<long>("Id");
                }
            }
            catch (WebException ex)
            {
                if (ex.Status == WebExceptionStatus.Timeout)
                {
                   //LogError(ex, "Timeout while uploading '{0}'", fileName);
                }
                else
                {
                    //LogError(ex, "Error while uploading '{0}'", fileName);
                }
                throw;
            }
        }

    }
}
public static class WebRequestExtensions
{
    public static Stream GetRequestStreamWithTimeout(
        this WebRequest request,
        int? millisecondsTimeout = null)
    {
        return AsyncToSyncWithTimeout(
            request.BeginGetRequestStream,
            request.EndGetRequestStream,
            millisecondsTimeout ?? request.Timeout);
    }

    public static WebResponse GetResponseWithTimeout(
        this HttpWebRequest request,
        int? millisecondsTimeout = null)
    {
        return AsyncToSyncWithTimeout(
            request.BeginGetResponse,
            request.EndGetResponse,
            millisecondsTimeout ?? request.Timeout);
    }

    private static T AsyncToSyncWithTimeout<T>(
        Func<AsyncCallback, object, IAsyncResult> begin,
        Func<IAsyncResult, T> end,
        int millisecondsTimeout)
    {
        var iar = begin(null, null);
        if (!iar.AsyncWaitHandle.WaitOne(millisecondsTimeout))
        {
            var ex = new TimeoutException();
            throw new WebException(ex.Message, ex, WebExceptionStatus.Timeout, null);
        }
        return end(iar);
    }
}
