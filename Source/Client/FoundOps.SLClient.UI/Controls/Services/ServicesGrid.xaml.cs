using System;
using System.Linq;
using System.Windows;
using FoundOps.Common.Tools;
using Telerik.Windows;
using System.Windows.Media;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using FoundOps.SLClient.UI.Tools;
using FoundOps.SLClient.Data.Tools;
using FoundOps.Core.Models.CoreEntities;
using Telerik.Windows.Controls.GridView;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using ItemsControlExtensions = FoundOps.Common.Silverlight.Tools.ItemsControlExtensions;

namespace FoundOps.SLClient.UI.Controls.Services
{
    /// <summary>
    /// The UI for displaying a list of Services.
    /// </summary>
    public partial class ServicesGrid
    {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ServicesGrid"/> class.
        /// </summary>
        public ServicesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(VM.Services);
            this.DependentWhenVisible(VM.ServiceTemplates);

            //Double click moves infinite accordion through to context
            ServicesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent,
                                           new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);

            //Order the grid by the date.
            ServicesRadGridView.SortDescriptors.Add(new ColumnSortDescriptor { Column = ServicesRadGridView.Columns["DateColumn"], SortDirection = ListSortDirection.Ascending });

            ServicesRadGridView.RowLoaded += (s, e) =>
            {
                //Make sure the SelectedEntity is in the middle
                if (_lastItemScrolledTo != VM.Services.SelectedEntity) ScrollToMiddle(VM.Services.SelectedEntity);
            };

            //Whenever the selected service changes, scroll it to the middle.
            VM.Services.SelectedEntityObservable.Throttle(TimeSpan.FromSeconds(1)).ObserveOnDispatcher()
                .Subscribe(ScrollToMiddle);

            //Listen to the latest selected service's occur date changes
            VM.Services.SelectedEntityObservable.WhereNotNull().SelectLatest(service => Observable2.FromPropertyChangedPattern(service, x => x.OccurDate))
                //Wait until the RadGridView SelectedItem updates
                .Throttle(TimeSpan.FromMilliseconds(500)).ObserveOnDispatcher()
                .Subscribe(e =>
                {
                    //resort the grid and put the service in the middle
                    ServicesRadGridView.SortDescriptors.Clear();
                    ServicesRadGridView.SortDescriptors.Add(new ColumnSortDescriptor { Column = ServicesRadGridView.Columns["DateColumn"], SortDirection = ListSortDirection.Ascending });
                    ScrollToMiddle(VM.Services.SelectedEntity);
                });
        }

        private GridViewScrollViewer _servicesRadGridViewScrollViewer;
        private GridViewScrollViewer ServicesRadGridViewScrollViewer
        {
            get
            {
                return _servicesRadGridViewScrollViewer ??
                       (_servicesRadGridViewScrollViewer =
                       ServicesRadGridView.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault());
            }
        }

        void ServicesGridLoaded(object sender, RoutedEventArgs e)
        {
            //If nearing the beginning or end of the scroll viewer push the GeneratedServices back or forward to show more items
            ServicesRadGridViewScrollViewer.ScrollChanged += (s, args) =>
            {
                //Only allow push forward or back after the current service has been scrolled to
                //Also do not push backward when the VerticalOffset==0 (to prevent looping backward when the data reloads)
                if (_lastItemScrolledTo != VM.Services.SelectedEntity)
                    return;

                if (args.VerticalChange < 0 && ServicesRadGridViewScrollViewer.VerticalOffset == 0)
                //If the scroll viewer is still at the top in 1 second, PushBackGeneratedServices
                {
                    Observable.Interval(TimeSpan.FromSeconds(1)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
                    {
                        if (ServicesRadGridViewScrollViewer.VerticalOffset == 0)
                            VM.Services.PushBackGeneratedServices();
                    });
                }

                if (args.VerticalChange > 0 && ServicesRadGridViewScrollViewer.VerticalOffset == ServicesRadGridViewScrollViewer.ScrollableHeight)
                //If the scroll viewer is still at the bottom in 1 second, PushForwardGeneratedServices
                {
                    Observable.Interval(TimeSpan.FromSeconds(1)).Take(1).ObserveOnDispatcher().Subscribe(_ =>
                    {
                        if (ServicesRadGridViewScrollViewer.VerticalOffset == ServicesRadGridViewScrollViewer.ScrollableHeight)
                            VM.Services.PushForwardGeneratedServices();
                    });
                }
            };
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }

        private object _lastItemScrolledTo;
        private void ScrollToMiddle(object item)
        {
            if (item == null) return;
            //ScrollIntoViewAsync to allow the ItemContainerGenerator to generate the container, and scroll it to the top
            ServicesRadGridView.ScrollIntoViewAsync(item, (callback) =>
            {
                //Now find and scroll to the middle

                // Find the container 
                var container = ServicesRadGridView.ItemContainerGenerator.ContainerFromItem(item) as UIElement;
                if (container == null) return;

                // Compute the center point of the container relative to the scrollInfo 
                Size size = container.RenderSize;
                if (size.Width == 0 && size.Height == 0)
                    return;
                Point center = container.TransformToVisual(ServicesRadGridViewScrollViewer).Transform(new Point(size.Width / 2, size.Height / 2));
                center.Y += ServicesRadGridViewScrollViewer.VerticalOffset;
                center.X += ServicesRadGridViewScrollViewer.HorizontalOffset;

                ServicesRadGridViewScrollViewer.ScrollToVerticalOffset(ItemsControlExtensions.CenteringOffset(center.Y, ServicesRadGridViewScrollViewer.ViewportHeight, ServicesRadGridViewScrollViewer.ExtentHeight));

                _lastItemScrolledTo = item;
            });
        }
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
            var serviceDate = ((ServiceHolder)item).OccurDate;

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
