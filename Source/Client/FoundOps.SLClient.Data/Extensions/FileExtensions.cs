using FoundOps.Common.Silverlight.Tools;
using System;
using System.Reactive.Linq;

//This is a partial class and must be in the same namespace as the generated CoreDomainService
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class File
    {
        #region Public Properties

        private byte[] _byteData;
        /// <summary>
        /// Gets the byte data of the file.
        /// </summary>
        public byte[] ByteData
        {
            get
            {
                //Get the data if it's not already loaded
                if (_byteData == null)
                {
                    FileManager.GetFile(this._partyId, this.Id).ObserveOnDispatcher()
                        .Subscribe(data =>
                        {
                            //When the data loads, call PropertyChanged
                            _byteData = data;
                            this.CompositeRaiseEntityPropertyChanged("ByteData");
                        });
                }

                return _byteData;
            }
            set
            {
                if (value == null)
                    return;

                _byteData = value;
                FileManager.InsertFile(this._partyId, this.Id, ByteData).ObserveOnDispatcher()
                    //Must subscribe to initiaate insert
                        .Subscribe();

                this.CompositeRaiseEntityPropertyChanged("ByteData");
            }
        }

        #endregion
    }
}
