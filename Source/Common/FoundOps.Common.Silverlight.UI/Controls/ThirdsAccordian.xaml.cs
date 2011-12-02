using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Browser;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.Controls
{
    public partial class ThirdsAccordian : INotifyPropertyChanged
    {
        /// <summary>
        /// Flag indicating Navigator/Firefox/Safari or Internet Explorer
        /// </summary>
        private static bool _isNavigator;

        /// <summary>
        /// Gets the window object's client width
        /// </summary>
        public double ClientWidth
        {
            get
            {
                //HtmlPage is not available in design mode
                if (DesignerProperties.IsInDesignTool)
                    return 1024;

                return _isNavigator ? (double)HtmlPage.Window.GetProperty("innerWidth")
                    : (double)HtmlPage.Document.Body.GetProperty("clientWidth");
            }

        }

        public ThirdsAccordian()
        {
            InitializeComponent();
            this.InvalidateArrange();
            ContentResized(null, null);
            Application.Current.Host.Content.Resized += ContentResized;
        }

        //When the gridsplitter is adjusted, keep the columns the correct size
        public void ContentResized(object sender, EventArgs e)
        {
            if (ClientWidth < 330) return;
            LeftColumn.MaxWidth = ClientWidth - 330;
            RightColumn.MaxWidth = ClientWidth - 330;
        }

        //NOTE: LeftContent (RadFluidContentControl)'s ContentChangeMode must be set to"Manual"

        private RadFluidContentControl _leftContent;
        public RadFluidContentControl LeftContent
        {
            get { return _leftContent; }
            set
            {
                _leftContent = value;

                RaisePropertyChanged("LeftContent");
                if (LeftContent == null) return;
                LeftContent.State = FluidContentControlState.Small;
            }
        }

        //NOTE: RightContent (RadFluidContentControl)'s ContentChangeMode must be set to"Manual"

        private RadFluidContentControl _rightContent;
        public RadFluidContentControl RightContent
        {
            get { return _rightContent; }
            set
            {
                _rightContent = value;

                RaisePropertyChanged("RightContent");
                if (RightContent == null) return;
                RightContent.State = FluidContentControlState.Normal;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected void RaisePropertyChanged(params string[] propertyNames)
        {
            foreach (var name in propertyNames)
            {
                RaisePropertyChanged(name);
            }
        }
        #endregion

    }
}
