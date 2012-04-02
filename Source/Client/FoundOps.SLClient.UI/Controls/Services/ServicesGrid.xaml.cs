using System.ComponentModel;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Shapes;
using FoundOps.Common.Tools;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using Telerik.Windows.Data;
using ItemsControlExtensions = FoundOps.Common.Silverlight.Tools.ItemsControlExtensions;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace FoundOps.SLClient.UI.Controls.Services
{
    /// <summary>
    /// The UI for displaying a list of Services.
    /// </summary>
    public partial class ServicesGrid
    {
        #region Public Properties

        private bool _isMainGrid;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is main grid.
        /// </summary>
        public bool IsMainGrid
        {
            get { return _isMainGrid; }
            set
            {
                _isMainGrid = value;

                if (!IsMainGrid) //Set max height to 125 pixels
                    this.Height = 125;
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesGrid"/> class.
        /// </summary>
        public ServicesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Services);

            //Initialize SortDescriptors
            SortGrid();

            //On double click move the infinite accordion to the Services details view
            ServicesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent,
                new EventHandler<RadRoutedEventArgs>((s, args) =>
                {
                    if (!IsMainGrid)
                        VM.Services.MoveToDetailsView.Execute(null);
                }), true);

            //Scroll the SelectedEntity to the middle
            //a) after the ServicesRadGridView's data is loaded
            //b) when the selected ServiceHolder changes
            Observable.FromEventPattern<EventHandler<EventArgs>, EventArgs>(h =>
            ServicesRadGridView.DataLoaded += h, h => ServicesRadGridView.DataLoaded -= h).AsGeneric()
            .Merge(VM.Services.SelectedEntityObservable.AsGeneric())
                //Throttle so it is not called to often
            .Throttle(TimeSpan.FromMilliseconds(250))
            .SubscribeOnDispatcher().Select(_ => VM.Services.SelectedEntity)
            .ObserveOnDispatcher().Subscribe(ScrollToMiddle);

            //Resort the grid and put the service in the middle whenever
            //a) the SelectedEntity's OccurDate is changed
            VM.Services.SelectedEntityObservable.WhereNotNull().SelectLatest(service => Observable2.FromPropertyChangedPattern(service, x => x.OccurDate))
                //Throttle to prevent refreshing to often
                .Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher()
                .Subscribe(e =>
                {
                    SortGrid();
                    ScrollToMiddle(VM.Services.SelectedEntity);
                });
        }

        /// <summary>
        /// Refreshes the Grid's SortDescriptors
        /// </summary>
        private void SortGrid()
        {
            ServicesRadGridView.SortDescriptors.Clear();
            ServicesRadGridView.SortDescriptors.Add(new SortDescriptor { Member = "OccurDate", SortDirection = ListSortDirection.Ascending });
            ServicesRadGridView.SortDescriptors.Add(new SortDescriptor { Member = "ServiceName", SortDirection = ListSortDirection.Ascending });
            ServicesRadGridView.SortDescriptors.Add(new SortDescriptor { Member = "ServiceId", SortDirection = ListSortDirection.Ascending });
            ServicesRadGridView.SortDescriptors.Add(new SortDescriptor { Member = "RecurringServiceId", SortDirection = ListSortDirection.Ascending });
        }

        #region Scrolling Logic

        private GridViewScrollViewer _servicesRadGridViewScrollViewer;
        private GridViewScrollViewer ServicesRadGridViewScrollViewer
        {
            get
            {
                return _servicesRadGridViewScrollViewer ??
                       (_servicesRadGridViewScrollViewer = ServicesRadGridView.ChildrenOfType<GridViewScrollViewer>().First());
            }
        }

        /// <summary>
        /// Fixes scroll stuck bug.
        /// </summary>
        private void ClearScrollBarFocus()
        {
            var thumbs = ServicesRadGridViewScrollViewer.ChildrenOfType<Thumb>();
            foreach (var thumb in thumbs)
                thumb.CancelDrag();
        }

        /// <summary>
        /// The scroll bar position of the ServicesGrid.
        /// </summary>
        private ScrollBarPosition ScrollBarPosition
        {
            get
            {
                //If it is within the top 5% of the scrollbar consider it to be at the top
                if (Math.Abs(ServicesRadGridViewScrollViewer.VerticalOffset - 0)
                    < (ServicesRadGridViewScrollViewer.ScrollableHeight * .05))
                    return ScrollBarPosition.Top;

                //If it is within the bottom 5% of the scrollbar consider it to be at the bottom
                if (Math.Abs(ServicesRadGridViewScrollViewer.VerticalOffset - ServicesRadGridViewScrollViewer.ScrollableHeight)
                    < (ServicesRadGridViewScrollViewer.ScrollableHeight * .05))
                    return ScrollBarPosition.Bottom;

                return ScrollBarPosition.Middle;
            }
        }

        //Prevent hooking up to ScrollChanged more than once
        private bool _setupScrollChanged;
        /// <summary>
        /// Setup the ServicesRadGridViewScrollViewer listener when nearing the beginning or end 
        /// of the scroll viewer to push the ServicesVM back or forward to show more items
        /// </summary>
        private void ServicesGridLoaded(object sender, RoutedEventArgs e)
        {
            if (_setupScrollChanged)
                return;

            _setupScrollChanged = true;
            ServicesRadGridViewScrollViewer.ScrollChanged += (s, args) =>
            {
                //Only allow push forward or back if all of the conditions are satisfied
                //a) this control is not automatically scrolling an item to the middle
                //b) the ServicesVM is not loading
                //c) the last load was 1 second ago (allow time for the RadGridView to update)
                if (_forceScrolling || VM.Services.IsLoading || DateTime.Now - VM.Services.LastLoad < TimeSpan.FromSeconds(1))
                    return;

                //Scrolled to the top
                if (args.VerticalChange < 0 && ScrollBarPosition == ScrollBarPosition.Top)
                {
                    //If the scroll viewer is still at the top in .5 second PushBackGeneratedServices
                    Observable.Interval(TimeSpan.FromSeconds(.5)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
                    {
                        if (ScrollBarPosition != ScrollBarPosition.Top) return;

                        //Clear the scroll focus (to prevent getting stuck) and push back services
                        ClearScrollBarFocus();

                        VM.Services.PushBackServices();
                        //Debug.WriteLine("Scroll to Top");
                    });
                }

                //Scrolled to the bottom
                if (args.VerticalChange > 0 && ScrollBarPosition == ScrollBarPosition.Bottom)
                {
                    //If the scroll viewer is still at the bottom in .5 seconds PushForwardGeneratedServices
                    Observable.Interval(TimeSpan.FromMilliseconds(.5)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
                    {
                        if (ScrollBarPosition != ScrollBarPosition.Bottom) return;

                        //Clear the scroll focus (to prevent getting stuck) and push forward services
                        ClearScrollBarFocus();

                        VM.Services.PushForwardServices();
                        //Debug.WriteLine("Scroll to Bottom");
                    });
                }
            };
        }

        //Keep track of when this control is force scrolling an item into the middle
        //to prevent triggering PushBackServices & PushForwardServices
        private bool _forceScrolling;
        private void ScrollToMiddle(object item)
        {
            if (item == null) return;
            _forceScrolling = true;

            //ScrollIntoViewAsync to scroll the selected item to the top
            ServicesRadGridView.ScrollIntoViewAsync(item, r =>
            {
                //Find the container
                var container = ServicesRadGridView.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                if (container == null)
                {
                    _forceScrolling = false;
                    return;
                }
                //Now find the middle, and scroll to it
                //Compute the center point of the container relative to the scrollInfo 
                Size size = container.RenderSize;
                if (Math.Abs(size.Width - 0) < 1 && Math.Abs(size.Height - 0) < 1)
                {
                    _forceScrolling = false;
                    return;
                }
                Point center = container.TransformToVisual(ServicesRadGridViewScrollViewer).Transform(new Point(size.Width / 2, size.Height / 2));
                center.Y += ServicesRadGridViewScrollViewer.VerticalOffset;
                center.X += ServicesRadGridViewScrollViewer.HorizontalOffset;

                ServicesRadGridViewScrollViewer.ScrollToVerticalOffset(ItemsControlExtensions.CenteringOffset(center.Y, ServicesRadGridViewScrollViewer.ViewportHeight, ServicesRadGridViewScrollViewer.ExtentHeight));

                //Focus on the row
                var row = r as GridViewRow;
                if (row != null)
                {
                    row.IsCurrent = true;
                    row.Focus();
                }

                _forceScrolling = false;
            });
        }

        #endregion
    }

    /// <summary>
    /// The position of the scroll bar
    /// </summary>
    public enum ScrollBarPosition
    {
        /// <summary>
        /// The top of the scroll bar.
        /// </summary>
        Top,
        /// <summary>
        /// Somewhere in the middle of the scroll bar.
        /// </summary>
        Middle,
        /// <summary>
        /// The bottom of the scroll bar.
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Changes the Foreground depending on the date of the service.
    /// </summary>
    public class ServiceStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var style = new Style(typeof(GridViewRow));
            if (item == null)
                return style;
            var serviceDate = ((ServiceHolder)item).OccurDate.Date;

            //If the service is in the past it should be grey.
            if (serviceDate < DateTime.Now.Date)
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Gray)));
            //If the service is today it should be green.
            else if (serviceDate == DateTime.Now.Date)
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Green)));
            //If the service is in the future it should be blue.
            else
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Blue)));
            return style;
        }
    }
}
