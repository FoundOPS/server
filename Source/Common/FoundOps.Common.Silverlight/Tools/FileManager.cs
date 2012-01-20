using System;
using System.Net;
using System.Reactive.Linq;
using FoundOps.Common.Tools;
using FoundOps.Common.Tools.ExtensionMethods;

namespace FoundOps.Common.Silverlight.Tools
{
    /// <summary>
    /// Works with the FileController to Get, Insert, Update and Delete protected files.
    /// </summary>
    public static class FileManager
    {
        //Ex. www.foundops.com/File
        private static readonly string FileControllerUrl = String.Format(@"{0}/File", UriExtensions.ThisRootUrl);

        /// <summary>
        /// Gets the protected file from the FileController's storage url.
        /// </summary>
        /// <param name="ownerPartyId">The file owner's Id.</param>
        /// <param name="fileId">The file id.</param>
        public static IObservable<byte[]> GetFile(Guid ownerPartyId, Guid fileId)
        {
            //Get the get access key
            var getAccessKey = Rxx2.HttpGetAsString(String.Format(@"{0}/GetBlobUrl?ownerPartyId={1}&fileGuid={2}", FileControllerUrl, ownerPartyId, fileId));

            //When the access key returns, get the file at the url + key (as a byte array)
            //It is a SelectLatest because both GetResponse methods return an Observables
            return getAccessKey.SelectLatest(accessKey => Rxx2.HttpGetAsByteArray(AzureTools.BuildFileUrl(ownerPartyId, fileId, accessKey)));
        }

        /// <summary>
        /// Inserts the protected file to the FileController's storage url.
        /// You must subscribe for it to work.
        /// </summary>
        /// <param name="ownerPartyId">The file owner's Id.</param>
        /// <param name="fileId">The file id.</param>
        /// <param name="data">The file data.</param>
        public static IObservable<WebResponse> InsertFile(Guid ownerPartyId, Guid fileId, byte[] data)
        {
            //Get the insert access Url
            var insertAccessKey = Rxx2.HttpGetAsString(String.Format(@"{0}/InsertBlobUrl?ownerPartyId={1}&fileGuid={2}", FileControllerUrl, ownerPartyId, fileId));

            //When the access key returns, post the file at the insert access Url

            //It is a SelectLatest because both GetResponse methods return an Observables
            return insertAccessKey.SelectLatest(accessKey => Rxx2.HttpPut(AzureTools.BuildFileUrl(ownerPartyId, fileId, accessKey), data));
        }

        /// <summary>
        /// Deletes the protected file.
        /// You must subscribe for it to work.
        /// </summary>
        /// <param name="ownerPartyId">The file owner's Id.</param>
        /// <param name="fileId">The file id.</param>
        public static IObservable<WebResponse> DeleteFile(Guid ownerPartyId, Guid fileId)
        {
            //Get the delete Url
            return Rxx2.HttpPut(String.Format(@"{0}/DeleteBlob?ownerPartyId={1}&fileGuid={2}", FileControllerUrl,
                ownerPartyId, fileId), null, verb: "POST");
        }
    }
}
