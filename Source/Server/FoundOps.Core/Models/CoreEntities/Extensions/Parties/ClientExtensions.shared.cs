using System;
using FoundOps.Common.Composite.Entities;
using FoundOps.Core.Models.CoreEntities.DesignData;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Client : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
            Initialize();
        }
#else
        public Client()
        {
            ((IEntityDefaultCreation)this).OnCreate();
            Initialize();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        private void Initialize()
        {
            //Whenever OwnedParty's DisplayName changes, update this DisplayName
            this.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "OwnedParty")
                {
                    if (this.OwnedParty != null)
                    {
                        OwnedParty.PropertyChanged += (s, args) =>
                        {
                            if (e.PropertyName == "DisplayName")
                                this.CompositeRaiseEntityPropertyChanged("DisplayName");
                        };
                    }

                    this.CompositeRaiseEntityPropertyChanged("DisplayName");
                }
            };
        }

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            DateAdded = DateTime.Now;
            OnCreation();
        }

        #endregion

        #region Implementation of ICompositeRaiseEntityPropertyChanged

#if SILVERLIGHT
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
#else
        public void CompositeRaiseEntityPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }
#endif
        #endregion

        public string DisplayName
        {
            get { return OwnedParty == null ? "" : OwnedParty.DisplayName; }
        }
    }
}