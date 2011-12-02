using System;
using System.Linq;
using System.Threading;
using System.Windows;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Data.Tools;
using Telerik.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using FoundOps.SLClient.Data.Tools;
using FoundOps.SLClient.UI.ViewModels;
using Telerik.Windows.Controls.GridView;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion;
using ItemsControlExtensions = FoundOps.Common.Silverlight.Tools.ItemsControlExtensions;

namespace FoundOps.SLClient.UI.Controls.Services
{
    public partial class ServicesGrid
    {
        private bool _isMainGrid;
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

        public ServicesGrid()
        {
            InitializeComponent();

            this.DependentWhenVisible(ServicesVM);
            this.DependentWhenVisible(ServiceTemplatesVM);
            
            //Double click moves infinite accordion through to context
            ServicesRadGridView.AddHandler(GridViewCellBase.CellDoubleClickEvent,
                                           new EventHandler<RadRoutedEventArgs>(OnCellDoubleClick), true);

            ServicesRadGridView.SortDescriptors.Add(new ColumnSortDescriptor { Column = ServicesRadGridView.Columns["DateColumn"], SortDirection = ListSortDirection.Ascending });

            ServicesRadGridView.RowLoaded += (s, e) =>
                                                  {
                                                      //Make sure the SelectedEntity is in the middle
                                                      if (_lastItemScrolledTo != ServicesVM.SelectedEntity)
                                                          ScrollToMiddle(ServicesVM.SelectedEntity);
                                                  };
        }

        public ServicesVM ServicesVM { get { return (ServicesVM)this.DataContext; } }

        public ServiceTemplatesVM ServiceTemplatesVM { get { return (ServiceTemplatesVM)ServiceTemplatesVMHolder.DataContext; } }

        private GridViewScrollViewer _servicesRadGridViewScrollViewer;
        private GridViewScrollViewer ServicesRadGridViewScrollViewer
        {
            get
            {
                if (_servicesRadGridViewScrollViewer == null)
                    _servicesRadGridViewScrollViewer = ServicesRadGridView.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault();

                return _servicesRadGridViewScrollViewer;
            }
        }

        void ServicesGridLoaded(object sender, RoutedEventArgs e)
        {
            //If nearing the beginning or end of the scroll viewer push the GeneratedServices back or forward to show more items
            ServicesRadGridViewScrollViewer.ScrollChanged += (s, args) =>
            {
                //Only allow push forward or back after the current service has been scrolled to
                //Also do not push backward when the VerticalOffset==0 (to prevent looping backward when the data reloads)
                if (_lastItemScrolledTo != ServicesVM.SelectedEntity)
                    return;

                if (args.VerticalChange < 0 && ServicesRadGridViewScrollViewer.VerticalOffset == 0)
                //If the scroll viewer is still at the top in 2 seconds, PushBackGeneratedServices
                {
                    new Timer(
                        (cb) => Dispatcher.BeginInvoke(() =>
                                {
                                    if (ServicesRadGridViewScrollViewer.VerticalOffset == 0)
                                        ServicesVM.PushBackGeneratedServices();
                                })
                                , null, 1000, Timeout.Infinite);
                }

                if (args.VerticalChange > 0 && ServicesRadGridViewScrollViewer.VerticalOffset == ServicesRadGridViewScrollViewer.ScrollableHeight)
                //If the scroll viewer is still at the bottom in 2 seconds, PushForwardGeneratedServices
                {
                    new Timer(
                        (cb) => Dispatcher.BeginInvoke(() =>
                        {
                            if (ServicesRadGridViewScrollViewer.VerticalOffset == ServicesRadGridViewScrollViewer.ScrollableHeight)
                                ServicesVM.PushForwardGeneratedServices();
                        })
                                , null, 1000, Timeout.Infinite);
                }
            };
        }

        private void OnCellDoubleClick(object sender, RadRoutedEventArgs radRoutedEventArgs)
        {
            if (!IsMainGrid)
                ((IProvideContext)this.DataContext).MoveToDetailsView.Execute(null);
        }

        private void ServicesRadGridViewSelectionChanged(object sender, SelectionChangeEventArgs e)
        {
            //Listen to the selected service's property changes, to check if the date changes

            //Clear listening to the last selected service
            var deselectedService = e.RemovedItems.OfType<Service>().FirstOrDefault();
            if (deselectedService != null)
                deselectedService.PropertyChanged -= SelectedServicePropertyChanged;

            var selectedService = e.AddedItems.OfType<Service>().FirstOrDefault();

            //Listen to selectedServiceTuple property change
            if (selectedService == null)
                return;

            selectedService.PropertyChanged += SelectedServicePropertyChanged;

            //Put the selection in the middle
            ScrollToMiddle(selectedService);
        }

        void SelectedServicePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //If the date changes: resort the grid, put the item in the middle
            if (e.PropertyName == "ServiceDate")
            {
                ServicesRadGridView.SortDescriptors.Clear();
                ServicesRadGridView.SortDescriptors.Add(new ColumnSortDescriptor { Column = ServicesRadGridView.Columns["DateColumn"], SortDirection = ListSortDirection.Ascending });
                ScrollToMiddle(ServicesRadGridView.SelectedItem);
            }
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

    public class ServiceStyleSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            var style = new Style(typeof(GridViewRow));
            if (item == null)
                return style;
            var serviceDate = ((Service)item).ServiceDate;

            if (serviceDate < DateTime.Now.Date)
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Gray)));
            else if (serviceDate == DateTime.Now.Date)
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Green)));
            else
                style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.Blue)));
            return style;
        }
    }
}
