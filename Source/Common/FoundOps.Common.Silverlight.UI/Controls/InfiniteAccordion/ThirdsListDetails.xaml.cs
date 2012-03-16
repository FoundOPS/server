using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    public enum ThirdsListDetailsMode
    {
        DetailsOnly,
        ListDetailsSummary
    }

    public partial class ThirdsListDetails : IObjectTypeDisplay, INotifyPropertyChanged
    {
        private readonly RadFluidContentControl _leftRadFluidContentControl = new RadFluidContentControl { ContentChangeMode = ContentChangeMode.Manual };
        private readonly RadFluidContentControl _rightRadFluidContentControl = new RadFluidContentControl { ContentChangeMode = ContentChangeMode.Manual, TransitionDuration = new TimeSpan(0, 0, 0, 0, 300) };

        public ThirdsListDetails()
        {
            InitializeComponent();
            
            //Commented out because small state is disable for now, TODO: Uncomment when small state is ready to be used again
            //_rightRadFluidContentControl.SizeChanged += (sender, e) =>
            //{
            //    _rightRadFluidContentControl.State = e.NewSize.Width < 500 ? FluidContentControlState.Small : FluidContentControlState.Normal;
            //};

            thirdsAccordion.LeftContent = _leftRadFluidContentControl;
            thirdsAccordion.RightContent = _rightRadFluidContentControl;

            SetupRadFluidContentControls(Mode);
        }

        #region Mode Dependency Property

        /// <summary>
        /// Mode
        /// </summary>
        public ThirdsListDetailsMode Mode
        {
            get { return (ThirdsListDetailsMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /// <summary>
        /// Mode Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                "Mode",
                typeof(ThirdsListDetailsMode),
                typeof(ThirdsListDetails),
                new PropertyMetadata(ThirdsListDetailsMode.ListDetailsSummary, new PropertyChangedCallback(ModeChanged)));

        private static void ModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as ThirdsListDetails;
            if (c != null)
            {
                if (e.NewValue == null)
                    return;

                c.SetupRadFluidContentControls((ThirdsListDetailsMode)e.NewValue);
            }
        }

        #endregion

        private object _list;
        public object List
        {
            get { return _list; }
            set
            {
                _list = value;
                SetupRadFluidContentControls(Mode);
                this.RaisePropertyChanged("List");
            }
        }

        private object _details;
        public object Details
        {
            get { return _details; }
            set
            {
                _details = value;
                //Display is the Details
                this.Display = (FrameworkElement)Details;
                SetupRadFluidContentControls(Mode);
                this.RaisePropertyChanged("Details");
            }
        }

        private object _summary;
        public object Summary
        {
            get { return _summary; }
            set
            {
                _summary = value;
                SetupRadFluidContentControls(Mode);
                this.RaisePropertyChanged("Summary");
            }
        }

        private void SetupRadFluidContentControls(ThirdsListDetailsMode mode)
        {
            if (_leftRadFluidContentControl == null || _rightRadFluidContentControl == null) return;

            if (mode == ThirdsListDetailsMode.DetailsOnly)
            {
                thirdsAccordion.Visibility = Visibility.Collapsed;
                DetailsContentPresenter.Visibility = Visibility.Visible;
            }
            else if (mode == ThirdsListDetailsMode.ListDetailsSummary)
            {
                if (Summary != null)
                    _rightRadFluidContentControl.SmallContent = Summary;

                if (Details != null)
                    _rightRadFluidContentControl.Content = Details;

                if (List != null)
                {
                    _leftRadFluidContentControl.SmallContent = List;
                    _leftRadFluidContentControl.Content = List;
                }

                _leftRadFluidContentControl.State = FluidContentControlState.Small;
                _rightRadFluidContentControl.State = FluidContentControlState.Normal;

                thirdsAccordion.Visibility = Visibility.Visible;
                DetailsContentPresenter.Visibility = Visibility.Collapsed;
            }

            InvalidateArrange();
        }

        #region IObjectTypeDisplay

                /// <value>
        /// The related context provider.
        /// </value>
        public IProvideContext ContextProvider { get; set; }

        /// <value>
        /// The object type to display.
        /// </value>
        public string ObjectTypeToDisplay { get; set; }

        private FrameworkElement _display;
        /// <summary>
        /// Gets or sets the Display FrameworkElement. The ContextProvider is the DataContext of this FrameworkElement
        /// </summary>
        public FrameworkElement Display
        {
            get { return _display; }
            set
            {
                _display = value;
                this.RaisePropertyChanged("Display");
                this.ContextProvider = value.DataContext as IProvideContext;
            }
        }

        #endregion

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