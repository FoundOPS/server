using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Net.Browser;
using System.Reactive.Linq;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    public static class Rxx3
    {
        /// <summary>
        /// Binds a DependencyObject to an observable.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <param name="property">The property.</param>
        /// <param name="source">The source.</param>
        public static IDisposable SetOneWayBinding<T>(this DependencyObject obj, DependencyProperty property, IObservable<T> source)
        {
            return source.SubscribeOnDispatcher().Subscribe(value => obj.SetValue(property, value));
        }


        #region Web Requests

        /// <summary>
        /// Performs an HTTP Get. Publishes the response as a byte[] when it is recieved.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns></returns>
        public static IObservable<Stream> HttpGetAsStream(string uri)
        {
            var request = (HttpWebRequest)WebRequest.Create(new Uri(uri));
            request.Method = "GET";
            var getUrl = Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse)()
                //In case nothing could be downloaded, return an empty stream
                .Catch(Observable.Empty<WebResponse>());

            return getUrl.Select(webResponse => webResponse.GetResponseStream());
        }

        /// <summary>
        /// Performs an HTTP Get. Publishes the response as a string when it is recieved.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns></returns>
        public static IObservable<string> HttpGetAsString(string uri)
        {
            return HttpGetAsStream(uri).Select(responseStream =>
                                                   {
                                                       using (var reader = new StreamReader(responseStream))
                                                           return reader.ReadToEnd();
                                                   });
        }

        /// <summary>
        /// Performs an HTTP Get. Publishes the response as a byte[] when it is recieved.
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <returns></returns>
        public static IObservable<byte[]> HttpGetAsByteArray(string uri)
        {
            return HttpGetAsStream(uri).Select(responseStream =>
                                                   {
                                                       using (var ms = new MemoryStream())
                                                       {
                                                           responseStream.CopyTo(ms);
                                                           return ms.ToArray();
                                                       }
                                                   });
        }

        /// <summary>
        /// Performs an HTTP Post. Publishes the HttpStatusCode.
        /// See http://stackoverflow.com/questions/7057805/how-to-make-http-post-using-reactive-extension-on-windows-phone-7
        /// </summary>
        /// <param name="uri">The Uri.</param>
        /// <param name="postData">The post data.</param>
        /// <param name="contentType">The post content type. (Defaults to application/octet-stream, general file or application)</param>
        /// <param name="verb">The HTTP verb. Defaults to PUT.</param>
        public static IObservable<WebResponse> HttpPut(string uri, byte[] postData, string contentType = "application/octet-stream", string verb = "PUT")
        {
            var request = (HttpWebRequest)WebRequestCreator.ClientHttp.Create(new Uri(uri));
            request.Method = "PUT";
            request.ContentType = contentType;

            var fetchRequestStream = Observable.FromAsyncPattern<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream);
            var fetchResponse = Observable.FromAsyncPattern<WebResponse>(request.BeginGetResponse, request.EndGetResponse);
            return fetchRequestStream().SelectMany(stream =>
                                                       {
                                                           using (var binWriter = new BinaryWriter(stream))
                                                               binWriter.Write(postData);

                                                           return fetchResponse();
                                                       }).Select(result => result);
        }

        #endregion
    }
}
