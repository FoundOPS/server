using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Invoice : ICompositeRaiseEntityPropertyChanged, IEntityDefaultCreation
    {
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

        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Invoice()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        #endregion

        public ScheduleMode ScheduleMode
        {
            get { return (ScheduleMode)ScheduleModeInt; }

            set
            {
                ScheduleModeInt = (int)value;
                CompositeRaiseEntityPropertyChanged("ScheduleMode");
            }
        }

        public FixedScheduleOption FixedScheduleOption
        {
            get { return (FixedScheduleOption)FixedScheduleOptionInt; }

            set
            {
                FixedScheduleOptionInt = (int)value;
                CompositeRaiseEntityPropertyChanged("FixedScheduleOption");
            }
        }

        partial void OnCreation(); //For Extensions on Silverlight Side

        public void OnCreate()
        {
            DueDate = this.DueDate;
            FixedScheduleOptionInt = this.FixedScheduleOptionInt;
            Memo = this.Memo;
            RelativeScheduleDays = this.RelativeScheduleDays;
            ScheduleModeInt = this.ScheduleModeInt;
            BillToLocation = this.BillToLocation;
            SalesTerm = this.SalesTerm;
            IsBillToLocationChanged = false;
            IsDueDateChanged = false;
            IsMemoChanged = false;
            OnCreation();
        }
    }
    public enum ScheduleMode
    {
        FixedDate = 0,
        Relative = 1
    }

    public enum FixedScheduleOption
    {
        FirstOfMonth = 0,
        MiddleOfMonth = 1,
        LastOfMonth = 2
    }
}