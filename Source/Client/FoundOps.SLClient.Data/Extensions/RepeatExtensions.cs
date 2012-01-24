using System;
using System.Reactive.Linq;
using FoundOps.Common.Tools;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper disable CheckNamespace
{
    public partial class Repeat
    {
        /// <summary>
        /// Exposes the RejectChanges method.
        /// </summary>
        public void Reject() { this.RejectChanges(); }

        partial void OnCreation()
        {
            SetDefaultWeeklyFrequencyDetailHelper();
        }

        protected override void OnLoaded(bool isInitialLoad)
        {
            if (isInitialLoad)
                SetDefaultWeeklyFrequencyDetailHelper();
        }

        private void SetDefaultWeeklyFrequencyDetailHelper()
        {
            //Whenever the Frequency is changed or the StartDate changes
            Observable2.FromPropertyChangedPattern(this, x => x.Frequency).DistinctUntilChanged().AsGeneric()
                .Merge(Observable2.FromPropertyChangedPattern(this, x => x.StartDate).DistinctUntilChanged().AsGeneric())
                //and when this Frequency is Weekly
                .Where(_ => this.Frequency == Frequency.Weekly).ObserveOnDispatcher()
                //Set the WeeklyFrequencyDetail to by default repeat on the StartDate's DayOfWeek
                .Subscribe(_ => FrequencyDetailAsWeeklyFrequencyDetail = new[] { this.StartDate.DayOfWeek });
        }
    }
}