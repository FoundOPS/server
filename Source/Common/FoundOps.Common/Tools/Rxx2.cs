using System;
using System.IO;
using System.Net;
using System.Net.Browser;
using System.Reactive;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Reactive.Disposables;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace FoundOps.Common.Tools
{
    /// <summary>
    /// Our extensions to Reactive Extensions Extensions (Rxx)
    /// </summary>
    public static class Rxx2
    {
        #region FromCollectionChanged

        /// <summary>
        /// Creates an Observable of NotifyCollectionChangedEventArgs from a collection.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> FromCollectionChanged(this INotifyCollectionChanged collection)
        {
            //Need to specifically use the addHandler and removeHandler parameters on the FromEventPattern 
            //because EntityCollections explicitly implements INotifyCollectionChanged
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => collection.CollectionChanged += h, h => collection.CollectionChanged -= h);
        }

        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> FromCollectionChangedAndNow(this INotifyCollectionChanged collection)
        {
            return collection.FromCollectionChanged().AndNow(new EventPattern<NotifyCollectionChangedEventArgs>(collection, null));
        }

        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static IObservable<bool> FromCollectionChangedGeneric(this INotifyCollectionChanged collection)
        {
            return collection.FromCollectionChanged().AsGeneric();
        }


        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static IObservable<bool> FromCollectionChangedGenericAndNow(this INotifyCollectionChanged collection)
        {
            return collection.FromCollectionChanged().AsGeneric().AndNow();
        }

        #endregion

        /// <summary>
        /// Returns the ObservableCollection whenever it is changed or set.
        /// </summary>
        /// <param name="observableCollectionObservable">The IObservable'ObservableCollection'.</param>
        public static IObservable<ObservableCollection<T>> FromCollectionChangedOrSet<T>(this IObservable<ObservableCollection<T>> observableCollectionObservable)
        {
            //An observable of when the collection is set
            var initiallySet = observableCollectionObservable;

            //An observable of when the collection is changed
            var collectionChanged =
                observableCollectionObservable.SelectLatest(collection => collection.FromCollectionChanged().Select(cc => collection));

            //Merge the two
            return initiallySet.Merge(collectionChanged);
        }

        /// <summary>
        /// Creates an Observable of PropertyChangedEventArgs from a class which implements INotifyPropertyChanged.
        /// </summary>
        /// <typeparam name="T">Must implement INotifyPropertyChanged.</typeparam>
        /// <param name="obj">The obj to create the PropertyChangedEventArgs observable.</param>
        public static IObservable<PropertyChangedEventArgs> FromAnyPropertyChanged<T>(this T obj) where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventArgs>(obj, "PropertyChanged").Select(ep => ep.EventArgs);
        }

        /// <summary>
        /// Creates an Observable of PropertyChangedEventArgs from a class which implements INotifyPropertyChanged.
        /// </summary>
        /// <typeparam name="T">Must implement INotifyPropertyChanged.</typeparam>
        /// <param name="obj">The obj to create the PropertyChangedEventArgs observable.</param>
        /// <param name="propertyName">The name of the property changes to observe.</param>
        public static IObservable<PropertyChangedEventArgs> FromPropertyChanged<T>(this T obj, string propertyName) where T : INotifyPropertyChanged
        {
            return obj.FromAnyPropertyChanged().Where(p => p.PropertyName == propertyName);
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
            return fetchRequestStream().SelectLatest(stream =>
                                        {
                                            using (var binWriter = new BinaryWriter(stream))
                                                binWriter.Write(postData);

                                            return fetchResponse();
                                        }).Select(result => result);
        }

        #endregion

        /// <summary>
        /// Merges an observable with an immediate true.
        /// </summary>
        /// <param name="observable">The observable to trigger now as well.</param>
        public static IObservable<bool> AndNow(this IObservable<bool> observable)
        {
            return observable.AndNow(true);
        }

        /// <summary>
        /// Merges an observable with an immediate <paramref name="now"/>
        /// </summary>
        /// <param name="observable">The observable to trigger now as well.</param>
        /// <param name="now">The object to send now.</param>
        public static IObservable<T> AndNow<T>(this IObservable<T> observable, T now)
        {
            return observable.Merge(new[] { now }.ToObservable());
        }

        /// <summary>
        /// Makes the observable return true whenever it fires.
        /// </summary>
        /// <param name="observable">The observable.</param>
        /// <returns></returns>
        public static IObservable<bool> AsGeneric<T>(this IObservable<T> observable)
        {
            return observable.Select(_ => true);
        }

        /// <summary>
        /// Similar to SelectMany except it only subscribes to the last TSource from source.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="selector">The selector.</param>
        /// <returns></returns>
        /// 
        public static System.IObservable<TResult> SelectLatest<TSource, TResult>(this System.IObservable<TSource> source, System.Func<TSource, IObservable<TResult>> selector)
        {
            var selectLatestSubject = new Subject<TResult>();

            var serialDisposable = new SerialDisposable();

            //Subscribe latestSelect to latest (the latest pushed TSource object)
            source.Subscribe(latest =>
            {
                //Call selectLatestSubject.OnNext whenever the returned IObservable<TResult> from selector(latest) pushes a new TResult
                var latestSubscription = selector(latest).Subscribe(selectLatestSubject.OnNext);

                try //Prevents random error when Locations is loading. TODO: Figure out why it does not dispose
                {
                    //Dispose the subscription whenever latestSubscription changes
                    serialDisposable.Disposable = latestSubscription;
                } catch{}
            });

            return selectLatestSubject;
        }

        /// <summary>
        /// Returns the Observable where the pushed object is not null.
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        public static IObservable<TSource> WhereNotNull<TSource>(this IObservable<TSource> source) where TSource : class
        {
            return source.Where(obj => obj != null);
        }
    }
}