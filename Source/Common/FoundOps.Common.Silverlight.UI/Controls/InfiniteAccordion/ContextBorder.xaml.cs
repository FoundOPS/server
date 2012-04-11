using System;
using ReactiveUI;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel.Composition;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    public partial class ContextBorder
    {
        #region Public

        #region BorderBrush Dependency Property

        /// <summary>
        /// BorderBrush
        /// </summary>
        public new Brush BorderBrush
        {
            get { return (Brush)GetValue(BorderBrushProperty); }
            set { SetValue(BorderBrushProperty, value); }
        }

        /// <summary>
        /// BorderBrush Dependency Property.
        /// </summary>
        public static readonly DependencyProperty BorderBrushProperty =
            DependencyProperty.Register(
                "BorderBrush",
                typeof(Brush),
                typeof(ContextBorder),
                new PropertyMetadata(new SolidColorBrush(Colors.Blue)));

        #endregion

        private object _borderContext;
        /// <summary>
        /// The DataContext for the control.
        /// </summary>
        public object BorderContext
        {
            get { return _borderContext; }
            set
            {
                _borderContext = value;

                if (HeaderContent == null) return;
                HeaderContent.DataContext = null;
                HeaderContent.DataContext = BorderContext;
                HeaderContent.InvalidateArrange();
            }
        }

        private UIElement _contextContent;
        /// <summary>
        /// The content of this Border.
        /// </summary>
        public UIElement ContextContent
        {
            get { return _contextContent; }
            set
            {
                _contextContent = value;
                ContextContentGrid.Children.Clear();
                if (ContextContent != null)
                    ContextContentGrid.Children.Add(ContextContent);
            }
        }

        /// <summary>
        /// The ContextType of this Border.
        /// </summary>
        public string ContextType { get; set; }

        #region DeleteButtonVisibility Dependency Property

        /// <summary>
        /// DeleteButtonVisibility
        /// </summary>
        public Visibility DeleteButtonVisibility
        {
            get { return (Visibility)GetValue(DeleteButtonVisibilityProperty); }
            set { SetValue(DeleteButtonVisibilityProperty, value); }
        }

        /// <summary>
        /// DeleteButtonVisibility Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DeleteButtonVisibilityProperty =
            DependencyProperty.Register(
                "DeleteButtonVisibility",
                typeof(Visibility),
                typeof(ContextBorder),
                new PropertyMetadata(null));

        #endregion

        #region HeaderContent Dependency Property

        /// <summary>
        /// HeaderContent
        /// </summary>
        public FrameworkElement HeaderContent
        {
            get { return (FrameworkElement)GetValue(HeaderContentProperty); }
            set { SetValue(HeaderContentProperty, value); }
        }

        /// <summary>
        /// HeaderContent Dependency Property.
        /// </summary>
        public static readonly DependencyProperty HeaderContentProperty =
            DependencyProperty.Register(
                "HeaderContent",
                typeof(FrameworkElement),
                typeof(ContextBorder),
                new PropertyMetadata(null));

        #endregion

        #endregion

        //Locals

        [Import]
        public ContextBorderClickManager ContextBorderClickManager { get; set; }

        public ContextBorder()
        {
            InitializeComponent();

            //Load the ContextBorderClickManager
            CompositionInitializer.SatisfyImports(this);
        }

        #region Logic

        //public

        public void ClearInside()
        {
            ContextContentGrid.Children.Clear();
            ContextContentGrid.InvalidateArrange();
        }

        //private

        /// <summary>
        /// Whenever a Border is double-clicked, move up to this ContextType.
        /// </summary>
        private void BorderMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount != 2) return;

            //Only action if the LastContextBorderDoubleClick was > 1/2 second ago
            if (DateTime.UtcNow - ContextBorderClickManager.LastContextBorderDoubleClick < new TimeSpan(0,0,0,0,500))
                return;

            //Track this double click
            ContextBorderClickManager.LastContextBorderDoubleClick = DateTime.UtcNow;
            
            //Move to this ContextType
            MessageBus.Current.SendMessage(new MoveToDetailsViewMessage(ContextType, MoveStrategy.MoveBackwards));
        }

        private void RemoveContextClick(object sender, RoutedEventArgs e)
        {
            MessageBus.Current.SendMessage(new RemoveContextMessage(BorderContext));
        }

        #endregion
    }

    /// <summary>
    /// When you double click on a ContextBorder it double clicks each parent ContextBorder.
    /// This keeps track of the last context border double click.
    /// </summary>
    [Export]
    public class ContextBorderClickManager
    {
        public DateTime LastContextBorderDoubleClick { get; set; }

        public ContextBorderClickManager()
        {
            LastContextBorderDoubleClick = DateTime.UtcNow;
        }
    }
}
