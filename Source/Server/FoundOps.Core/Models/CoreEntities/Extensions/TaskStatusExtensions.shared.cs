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
                return IntegerToStatusDetail(StatusDetailInt);
            }
            set
            {
                var detailsForStatusDetails = value;

                if (detailsForStatusDetails == null || detailsForStatusDetails.Any())
                {
                    StatusDetailInt = null;
                    return;
                }

                var statusDetailString = "";

                if (detailsForStatusDetails.Contains(StatusDetail.OutOfRoute))
                    statusDetailString += "1";

                if (detailsForStatusDetails.Contains(StatusDetail.InRoute))
                    statusDetailString += "2";

                if (detailsForStatusDetails.Contains(StatusDetail.CreatedDefault))
                    statusDetailString += "3";

                if (detailsForStatusDetails.Contains(StatusDetail.RoutedDefault))
                    statusDetailString += "4";

                if (detailsForStatusDetails.Contains(StatusDetail.CompletedDefault))
                    statusDetailString += "5";

                StatusDetailInt = Convert.ToInt32(statusDetailString);
            }
        }

        private StatusDetail[] IntegerToStatusDetail(int? statusDetailInt)
        {
            var detailsForStatusDetails = new ObservableCollection<StatusDetail>();

            if (statusDetailInt == null)
                return detailsForStatusDetails.ToArray();

            var statusDetailString = statusDetailInt.ToString();

            if (statusDetailString.Contains("1"))
                detailsForStatusDetails.Add(StatusDetail.OutOfRoute);

            if (statusDetailString.Contains("2"))
                detailsForStatusDetails.Add(StatusDetail.InRoute);

            if (statusDetailString.Contains("3"))
                detailsForStatusDetails.Add(StatusDetail.CreatedDefault);

            if (statusDetailString.Contains("4"))
                detailsForStatusDetails.Add(StatusDetail.RoutedDefault);

            if (statusDetailString.Contains("5"))
                detailsForStatusDetails.Add(StatusDetail.CompletedDefault);

            return detailsForStatusDetails.ToArray();
        }

        public void AddStatusToTaskStatus(StatusDetail statusDetail)
        {
            var detailString = StatusDetailInt.ToString();

            if(statusDetail == StatusDetail.OutOfRoute)
                detailString += "1";
            if(statusDetail == StatusDetail.InRoute)
                detailString += "2";
            if(statusDetail == StatusDetail.CreatedDefault)
                detailString += "3";
            if(statusDetail == StatusDetail.RoutedDefault)
                detailString += "4";
            if(statusDetail == StatusDetail.CompletedDefault)
                detailString += "5";

            StatusDetailInt = Convert.ToInt32(detailString);
        }
    }

    /// <summary>
    /// InRoute means that the TaskStatus requires that task to be in a route
    /// </summary>
    public enum StatusDetail
    {
        OutOfRoute = 1,
        InRoute = 2,
        CreatedDefault = 3,
        RoutedDefault = 4,
        CompletedDefault = 5
    }
}
