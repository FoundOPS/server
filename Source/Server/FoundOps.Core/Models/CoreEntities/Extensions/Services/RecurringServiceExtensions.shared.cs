using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Entities;

//This is a partial class, must be in the same namespace so disable warning
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    //Not sure what this is used for [Field("PartyToService", "Client")]
    public partial class RecurringService : IEntityDefaultCreation, ICompositeRaiseEntityPropertyChanged
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT

        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else

        public RecurringService()
        {
            OnCreation();
        }

#endif
        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            AddRepeat();
        }

        #endregion

        public void AddRepeat()
        {
            var repeat = new Repeat { Id = this.Id };
            this.Repeat = repeat;
        }

        /// <summary>
        /// Gets or sets the excluded dates.
        /// </summary>
        /// <value>
        /// The excluded dates.
        /// </value>
        public IEnumerable<DateTime> ExcludedDates
        {
            get
            {
                if (ExcludedDatesString == null)
                    return new List<DateTime>();

                //Split the delimited string and convert it to a list of DateTimes
                return ExcludedDatesString.Split(',').Select(Convert.ToDateTime);
            }
            set
            {
                //Create a delimited string of DateTimes
                ExcludedDatesString = String.Join(",", value);
                CompositeRaiseEntityPropertyChanged("ExcludedDates");
            }
        }

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
    }
}