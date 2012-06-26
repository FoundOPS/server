using System.ComponentModel;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Dispatcher
{
    /// <summary>
    /// Displays the algorithm's current status.
    /// </summary>
    public partial class AlgorithmStatus : INotifyPropertyChanged
    {
        private AlgorithmVM _algorithmVM;
        public AlgorithmVM AlgorithmVM
        {
            get { return _algorithmVM; }
            set
            {
                _algorithmVM = value;
                this.RaisePropertyChanged("AlgorithmVM");
            }
        }

        public AlgorithmStatus()
        {
            InitializeComponent();
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void RaisePropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}

