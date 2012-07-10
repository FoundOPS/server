using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class TaskStatus
    {
        public StatusDetail[] StatusDetails
        {
            get
            {
                return IntegerToStatusDetail(DefaultTypeInt);
            }
            set
            {
                var detailsForStatusDetails = value;

                if (detailsForStatusDetails == null || detailsForStatusDetails.Any())
                {
                    DefaultTypeInt = null;
                    return;
                }

                //Join the integers of the distinct status details
                //Ex { OutOfRoute, CreatedDefault } = 13
                var statusDetailString = String.Join("", detailsForStatusDetails.Distinct().Select(d => ((int) d).ToString()));
                
                if(statusDetailString != "")
                    DefaultTypeInt = Convert.ToInt32(statusDetailString);
            }
        }

        private StatusDetail[] IntegerToStatusDetail(int? defaultTypeInt)
        {
            var detailsForStatusDetails = new ObservableCollection<StatusDetail>();

            if (defaultTypeInt == null)
                return detailsForStatusDetails.ToArray();

            //Split the integers and convert them to the StatusDetail enum
            var statusDetailString = defaultTypeInt.ToString().ToCharArray()
                                    .Select(sd => ((StatusDetail)Convert.ToInt32(sd))).ToArray();

            foreach (var statusDetail in statusDetailString)
            {
                if (statusDetail == StatusDetail.CreatedDefault)
                    detailsForStatusDetails.Add(StatusDetail.CreatedDefault);

                if (statusDetail == StatusDetail.RoutedDefault)
                    detailsForStatusDetails.Add(StatusDetail.RoutedDefault);

                if (statusDetail == StatusDetail.CompletedDefault)
                    detailsForStatusDetails.Add(StatusDetail.CompletedDefault);
            }

            return detailsForStatusDetails.ToArray();
        }

        public TaskStatus GetDefaultTaskStatus(BusinessAccount businessAccount, StatusDetail detail)
        {
            return businessAccount.TaskStatuses.FirstOrDefault(ts => ts.StatusDetails.Contains(detail));
        }
    }

    /// <summary>
    /// InRoute means that the TaskStatus requires that task to be in a route
    /// </summary>
    public enum StatusDetail
    {
        CreatedDefault = 1, //If you change this, also change it in the SQL function that fills the TaskBoard
        RoutedDefault = 2,
        CompletedDefault = 3
    }
}
