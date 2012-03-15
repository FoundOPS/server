using FoundOps.Common.Silverlight.UI.Interfaces;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class Employee : ILoadDetails
    {
        #region Public Properties

        #region Implementation of ILoadDetails

        private bool _detailsLoading;
        /// <summary>
        /// Gets or sets a value indicating whether [details loading].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [details loading]; otherwise, <c>false</c>.
        /// </value>
        public bool DetailsLoading
        {
            get { return _detailsLoading; }
            set
            {
                _detailsLoading = value;
                this.RaisePropertyChanged("DetailsLoading");
            }
        }

        #endregion

        #endregion
    }
}
