using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel.Composition;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    /// <summary>
    /// The page which holds the InfiniteAccordion and it's ObjectDisplayControls.
    /// </summary>
    public interface IInfiniteAccordionPage
    {
        /// <summary>
        /// The selected object type.
        /// </summary>
        string SelectedObjectType { set; }

        /// <summary>
        /// Exposes the parent grid.
        /// </summary>
        Grid ParentGrid { get; }

        /// <summary>
        /// Returns the Object the display to display.
        /// </summary>
        /// <param name="objectTypeToDisplay">The object type to display.</param>
        /// <returns></returns>
        IObjectTypeDisplay ObjectDisplayToDisplay(Type objectTypeToDisplay);

        /// <summary>
        /// Gets the UI element.
        /// </summary>
        UIElement ThisUIElement { get; }
    }

    /// <summary>
    /// A PageWrapper for the InfiniteAccordion to support navigation.
    /// It inserts the InfiniteAccordion on this page when navigated to.
    /// </summary>
    public partial class InfiniteAccordionPageWrapper
    {
        /// <summary>
        /// The InfiniteAccordionPage to wrap.
        /// </summary>
        private readonly IInfiniteAccordionPage _infiniteAccordionPage;

        /// <summary>
        /// The ElementType this Page represents.
        /// </summary>
        private readonly Type _representingElementType;

        /// <summary>
        /// Initializes a new instance of the <see cref="InfiniteAccordionPageWrapper"/> class.
        /// </summary>
        /// <param name="infiniteAccordionPage">The infinite accordion page to insert.</param>
        /// <param name="representingElementType">The representing element type.</param>
        [ImportingConstructor]
        public InfiniteAccordionPageWrapper(IInfiniteAccordionPage infiniteAccordionPage, Type representingElementType)
        {
            InitializeComponent();

            _infiniteAccordionPage = infiniteAccordionPage;
            _representingElementType = representingElementType;
            //Set this DataContext to the DataContext of the corresponding Display FrameworkElement
            this.SetBinding(DataContextProperty,
                            new Binding("DataContext") { Source = infiniteAccordionPage.ObjectDisplayToDisplay(representingElementType).Display });
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            //Clear the InfiniteAccordionPage's controls
            if (_infiniteAccordionPage.ParentGrid != null)
                _infiniteAccordionPage.ParentGrid.Children.Clear();

            //Add the InfiniteAccordionPage to this
            LayoutRoot.Children.Add(_infiniteAccordionPage.ThisUIElement);

            //Set the InfiniteAccordionPage to this ElementType
            _infiniteAccordionPage.SelectedObjectType = _representingElementType.ToString();

            //Update the UI
            this.InvalidateArrange();

            base.OnNavigatedTo(e);
        }
    }
}
