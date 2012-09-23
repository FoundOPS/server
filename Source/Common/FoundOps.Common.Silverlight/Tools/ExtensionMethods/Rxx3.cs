using FoundOps.Common.Tools;
using System;
using System.IO;
using System.Net;
using System.Net.Browser;
using System.Reactive;
using System.Windows;
using System.Reactive.Linq;

namespace FoundOps.Common.Silverlight.Tools.ExtensionMethods
{
    /// <summary>
    /// Extensions for Reactive Extensions that are Silverlight only
    /// </summary>
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

        /// <summary>
        /// Runs an action on the dispatcher after the delay.
        /// </summary>
        /// <param name="delay">The delayed timespan</param>
        /// <param name="action">The action to run after the delay.</param>
        public static IDisposable RunDelayed(TimeSpan delay, Action action)
        {
            return Observable.Interval(delay).Take(1).ObserveOnDispatcher().Subscribe(_ => action());
        }

        /// <summary>
        /// An observable that pushes whenever the FrameworkElement is loaded to the page.
        /// </summary>
        /// <param name="element">The framework element.</param>
        public static IObservable<EventPattern<RoutedEventArgs>> LoadedObservable(this FrameworkElement element)
        {
            return Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(h => element.Loaded += h, h => element.Loaded -= h);
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
            return fetchRequestStream().SelectLatest(stream =>
            {
                using (var binWriter = new BinaryWriter(stream))
                    binWriter.Write(postData);

                return fetchResponse();
            }).Select(result => result);
        }
    }
}
