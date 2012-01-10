using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Printing;
using FoundOps.Common.Silverlight.Tools.Printing;

namespace FoundOps.Common.Silverlight.UI.Controls.Printing
{
    /// <summary>
    /// This allows paging for printing
    /// NOTE: You must set the BodyItemsControl and PrintableHeight and PrintableWidth
    /// </summary>
    public class PagedPrinter : UserControl, IPagedPrinter, INotifyPropertyChanged
    {
        #region Public Properties

        #region ItemsSource Dependency Property

        /// <summary>
        /// ItemsSource
        /// </summary>
        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// ItemsSource Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable<object>),
                typeof(PagedPrinter),
                new PropertyMetadata(new PropertyChangedCallback(ItemsSourceChanged)));

        private static void ItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as PagedPrinter;
            if (c != null)
            {
                if (e.NewValue == null) return;
                c.CurrentPageIndex = 0;
            }
        }

        #endregion

        #region Implementation of IPagedPrinter

        private int _pageCount;
        public int PageCount
        {
            get { return _pageCount; }
            private set
            {
                _pageCount = value;
                this.RaisePropertyChanged("PageCount");
            }
        }

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get { return _currentPageIndex; }
            set
            {
                //If not trying to move to the first page 
                //return if the value is already the CurrentPageIndex
                if (value != 0 && value == CurrentPageIndex)
                    return;

                _currentPageIndex = value;
                CurrentRenderingPageIndex = value;

                //Render all pages if this is the first page
                UpdateCurrentItems(value != 0);

                this.RaisePropertyChanged("CurrentPageIndex");
            }
        }

        private int _currentRenderingPageIndex;
        public int CurrentRenderingPageIndex
        {
            get { return _currentRenderingPageIndex; }
            private set
            {
                _currentRenderingPageIndex = value;
                this.RaisePropertyChanged("CurrentRenderingPageIndex");
            }
        }

        //Must be forced binding
        public bool IsFirstPage
        {
            get { return CurrentRenderingPageIndex == 0; }
        }

        //Must be forced binding
        public bool IsLastPage
        {
            get { return CurrentRenderingPageIndex == PageCount - 1; }
        }

        public void Print()
        {
            var printDocument = new PrintDocument();

            //Start on first page
            var pageToPrint = 0;

            //Go through each page, render it, then move forward
            printDocument.PrintPage += (sender, e) =>
            {
                //Render the pageToPrint
                CurrentPageIndex = pageToPrint;
                e.PageVisual = this;
                ForceBindingsUpdateLayout(e.PageVisual);
                e.PageVisual.Measure(e.PrintableArea);
                
                //Continue printing if not past the last page
                e.HasMorePages = pageToPrint < (this.PageCount - 1);

                //Increase page to print
                pageToPrint++;

                Debug.WriteLine(e.HasMorePages);
                Debug.WriteLine((pageToPrint));

            };

            //Move back to the first page at the end of printing
            printDocument.EndPrint += (s, e) => CurrentPageIndex = 0;

            //SL5 Release with Vector Printing
            var settings = new PrinterFallbackSettings {ForceVector = true};
            printDocument.Print("Route Manifest", settings);

            //SL4 Printing
            //printDocument.Print("Route Manifest");
        }

        #endregion

        public double PrintableHeight { get; set; }
        public double PrintableWidth { get; set; }

        #endregion

        protected ItemsControl BodyItemsControl;

        public void UpdateCurrentItems(bool onlyRenderCurrentPage = false)
        {
            //If not loaded yet return
            if (ItemsSource == null || this.ActualHeight == 0)
                return;

            CurrentRenderingPageIndex = 0;

            var currentItemIndex = 0;
            var totalItemsPrinted = 0;
            bool collectedItemsForSelectedPage = false;
            object[] selectedCurrentPageItems = null;

            do
            {
                var currentPageIndexItems = new ObservableCollection<object>();
                BodyItemsControl.ItemsSource = currentPageIndexItems;

                //Update Layout and Re-Measure
                ForceBindingsUpdateLayout(BodyItemsControl);

                //Add items until the height is past the printable area
                while (currentItemIndex < ItemsSource.Count() && this.ActualHeight <= PrintableHeight)
                {
                    //Add item
                    currentPageIndexItems.Add(ItemsSource.ElementAt(currentItemIndex));

                    //Update Layout and Re-Measure
                    ForceBindingsUpdateLayout(BodyItemsControl);

                    currentItemIndex++;
                }

                //Remove the item that made it bigger than the current size
                if (this.ActualHeight > PrintableHeight && currentItemIndex > 0)
                {
                    currentPageIndexItems.Remove(ItemsSource.ElementAt((currentItemIndex - 1)));
                    currentItemIndex--;
                }

                //Keep a copy of the proper items to display for the selected current page
                if (CurrentRenderingPageIndex == CurrentPageIndex)
                {
                    selectedCurrentPageItems = currentPageIndexItems.ToArray();
                    collectedItemsForSelectedPage = true;
                }

                if (currentPageIndexItems.Count <= ItemsSource.Count())
                {
                    totalItemsPrinted += currentPageIndexItems.Count;
                    if (totalItemsPrinted <= ItemsSource.Count())
                        CurrentRenderingPageIndex++;
                }

                //Move to the next page until all items are checked
                //If onlyRenderCurrentPage, only go until at the CurrentPageIndex
                //Otherwise go until end to calculate the PageCount
            } while (currentItemIndex < ItemsSource.Count() && (CurrentRenderingPageIndex <= CurrentPageIndex || !onlyRenderCurrentPage));

            if (!onlyRenderCurrentPage) //Update page count if not only rending the current page
                PageCount = CurrentRenderingPageIndex;

            if (collectedItemsForSelectedPage)
            {
                //Set the BodyItemsControl to the proper items source
                BodyItemsControl.ItemsSource = null;
                BodyItemsControl.ItemsSource = selectedCurrentPageItems;
            }

            CurrentRenderingPageIndex = CurrentPageIndex;

            ForceBindingsUpdateLayout();
        }

        private void ForceBindingsUpdateLayout(UIElement element = null)
        {
            if (element == null)
                element = this;

            element.UpdateLayout();
            ForceBindings();
            element.InvalidateArrange();
        }

        /// <summary>
        /// Can be overridden to allow forced bindings
        /// </summary>
        protected virtual void ForceBindings()
        {
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