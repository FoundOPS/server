using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Net.Browser;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Collections.Generic;
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
        public static IObservable<NotifyCollectionChangedEventArgs> FromCollectionChangedEvent(this INotifyCollectionChanged collection)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventArgs>(collection, "CollectionChanged").Select(ep => ep.EventArgs);
        }

        /// <summary>
        /// Creates an Observable of bool whenever a collection changes.
        /// </summary>
        /// <param name="collection">The collection.</param>
        public static IObservable<bool> FromCollectionChangedEventGeneric(this INotifyCollectionChanged collection)
        {
            return collection.FromCollectionChangedEvent().Select(_ => true);
        }

        #endregion

        #region FromCollectionChangedOrSet

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
                observableCollectionObservable.SelectMany(collection => collection.FromCollectionChangedEvent().Select(cc => collection));

            //Merge the two
            return initiallySet.Merge(collectionChanged);
        }

        /// <summary>
        /// Returns true whenever the ObservableCollection is changed or set.
        /// </summary>
        /// <param name="observableCollectionObservable">The IObservable'ObservableCollection'.</param>
        /// <returns>True whenever the ObservableCollection changed or set</returns>
        public static IObservable<bool> FromCollectionChangedOrSetGeneric<T>(this IObservable<ObservableCollection<T>> observableCollectionObservable)
        {
            return observableCollectionObservable.FromCollectionChangedOrSet().Select(_ => true);
        }

        #endregion

        /// <summary>
        /// Creates an Observable of PropertyChangedEventArgs from a class which implements INotifyPropertyChanged.
        /// </summary>
        /// <typeparam name="T">Must implement INotifyPropertyChanged.</typeparam>
        /// <param name="obj">The obj to create the PropertyChangedEventArgs observable.</param>
        public static IObservable<PropertyChangedEventArgs> FromAnyPropertyChanged<T>(this T obj) where T : INotifyPropertyChanged
        {
            return Observable.FromEventPattern<PropertyChangedEventArgs>(obj, "PropertyChanged").Select(ep => ep.EventArgs);
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

        /// <summary>
        /// An N-ary CombineLatest.
        /// It's designed with considerations for performance, memory consumption (written in terms of IEnumerable, without requiring an initial buffer for a total count)
        /// and to avoid stack overflows. From http://social.msdn.microsoft.com/Forums/en-ca/rx/thread/daaa84db-b560-4eda-871e-e523098db20c
        /// </summary>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="sources">The sources.</param>
        /// <returns>An IObservable of a List of the combined items</returns>
        public static IObservable<IList<TSource>> CombineLatest<TSource>(this IEnumerable<IObservable<TSource>> sources)
        {
            return Observable.Create<IList<TSource>>(
                observer =>
                {
                    object gate = new object();
                    var disposables = new CompositeDisposable();
                    var list = new List<TSource>();
                    var hasValueFlags = new List<bool>();
                    var actionSubscriptions = 0;
                    bool hasSources, hasValueFromEach = false;

                    using (var e = sources.GetEnumerator())
                    {
                        bool subscribing = hasSources = e.MoveNext();

                        while (subscribing)
                        {
                            var source = e.Current;
                            int index;

                            lock (gate)
                            {
                                actionSubscriptions++;

                                list.Add(default(TSource));
                                hasValueFlags.Add(false);

                                index = list.Count - 1;

                                subscribing = e.MoveNext();
                            }

                            disposables.Add(
                                source.Subscribe(
                                    value =>
                                    {
                                        IList<TSource> snapshot;

                                        lock (gate)
                                        {
                                            list[index] = value;

                                            if (!hasValueFromEach)
                                            {
                                                hasValueFlags[index] = true;

                                                if (!subscribing)
                                                {
                                                    hasValueFromEach = hasValueFlags.All(b => b);
                                                }
                                            }

                                            if (subscribing || !hasValueFromEach)
                                            {
                                                snapshot = null;
                                            }
                                            else
                                            {
                                                snapshot = list.ToList().AsReadOnly();
                                            }
                                        }

                                        if (snapshot != null)
                                        {
                                            observer.OnNext(snapshot);
                                        }
                                    },
                                    observer.OnError,
                                    () =>
                                    {
                                        bool completeNow;

                                        lock (gate)
                                        {
                                            actionSubscriptions--;

                                            completeNow = actionSubscriptions == 0 && !subscribing;
                                        }

                                        if (completeNow)
                                        {
                                            observer.OnCompleted();
                                        }
                                    }));
                        }
                    }

                    if (!hasSources)
                    {
                        observer.OnCompleted();
                    }

                    return disposables;
                });
        }
    }
}
