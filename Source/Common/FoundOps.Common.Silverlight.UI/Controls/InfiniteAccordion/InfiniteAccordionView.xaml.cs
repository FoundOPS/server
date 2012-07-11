using System;
using FoundOps.Common.Silverlight.UI.Interfaces;
using ReactiveUI;
using System.Linq;
using System.Windows;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion
{
    public partial class InfiniteAccordionView : INotifyPropertyChanged
    {
        #region Public

        #region ContextBorderControls Dependency Property

        /// <summary>
        /// ContextBorderControls
        /// </summary>
        public ObservableCollection<ContextBorder> ContextBorderControls
        {
            get { return (ObservableCollection<ContextBorder>)GetValue(ContextBorderControlsProperty); }
            set { SetValue(ContextBorderControlsProperty, value); }
        }

        /// <summary>
        /// ContextBorderControls Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ContextBorderControlsProperty =
            DependencyProperty.Register(
                "ContextBorderControls",
                typeof(ObservableCollection<ContextBorder>),
                typeof(InfiniteAccordionView),
                new PropertyMetadata(null));

        #endregion

        #region ObjectDisplayTypeToDisplay Dependency Property

        /// <summary>
        /// InitialListControlType
        /// </summary>
        public string ObjectDisplayTypeToDisplay
        {
            get { return (string)GetValue(ObjectDisplayTypeToDisplayProperty); }
            set { SetValue(ObjectDisplayTypeToDisplayProperty, value); }
        }

        /// <summary>
        /// InitialListControlType Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ObjectDisplayTypeToDisplayProperty =
            DependencyProperty.Register(
                "ObjectDisplayTypeToDisplay",
                typeof(string),
                typeof(InfiniteAccordionView),
                new PropertyMetadata(new PropertyChangedCallback(ObjectDisplayTypeToDisplayChanged)));

        private static void ObjectDisplayTypeToDisplayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as InfiniteAccordionView;
            if (c != null)
                c.SetObjectDisplayControl((string)e.NewValue);
        }

        #endregion

        private ObservableCollection<IObjectTypeDisplay> _objectTypeDisplayControls;
        /// <summary>
        /// Gets or sets the object display controls.
        /// </summary>
        public ObservableCollection<IObjectTypeDisplay> ObjectTypeDisplayControls
        {
            get { return _objectTypeDisplayControls; }
            set
            {
                _objectTypeDisplayControls = value;
                ObjectTypeDisplayControls.CollectionChanged += (s, e) => SetObjectDisplayControl(ObjectDisplayTypeToDisplay);
            }
        }

        #endregion

        #region Private

        private List<object> CurrentContext { get; set; }
        private IProvideContext _currentlySelectedContextProvider;
        public IProvideContext CurrentlySelectedContextProvider
        {
            get { return _currentlySelectedContextProvider; }
            set
            {
                _currentlySelectedContextProvider = value;
                this.RaisePropertyChanged("CurrentlySelectedContextProvider");
                MessageBus.Current.SendMessage(new ContextProviderChangedMessage(value));
            }
        }

        #endregion

        #region Constructor

        public InfiniteAccordionView()
        {
            InitializeComponent();
            ObjectTypeDisplayControls = new ObservableCollection<IObjectTypeDisplay>();
            ContextBorderControls = new ObservableCollection<ContextBorder>();
            CurrentContext = new List<object>();

            MessageBus.Current.Listen<RemoveContextMessage>().SubscribeOnDispatcher().Subscribe(message =>
            {
                this.CurrentContext.Remove(message.Context);
                SetupUI();
            });

            MessageBus.Current.Listen<MoveToDetailsViewMessage>().SubscribeOnDispatcher().Subscribe(OnMoveToDetailsViewMessage);

            CurrentlySelectedContextProvider = null;

            this.Loaded += InfiniteAccordionViewLoaded;
        }

        void InfiniteAccordionViewLoaded(object sender, RoutedEventArgs e)
        {
            //Refresh the ObjectDisplayTypeToDisplay, in case it was set before the InfiniteAccordionView was loaded
            var type = ObjectDisplayTypeToDisplay;
            ObjectDisplayTypeToDisplay = null;
            ObjectDisplayTypeToDisplay = type;
        }

        /// <summary>
        /// Called when you are supposed to [move to details view message].
        /// </summary>
        /// <param name="message">The message.</param>
        private void OnMoveToDetailsViewMessage(MoveToDetailsViewMessage message)
        {
            //Get the corresponding ObjectToDisplay
            var objectToDisplay = ObjectTypeDisplayControls.FirstOrDefault(objectDisplayControl => objectDisplayControl.ObjectTypeToDisplay == message.TypeOfDetailsView.ToString());

            if (objectToDisplay == null)
                return;

            var contextProvider = objectToDisplay.ContextProvider;
            if (contextProvider == null)
                return;

            //Check if the CurrentlySelectedContextProvider prevents navigation
            var currentProvider = CurrentlySelectedContextProvider as IPreventNavigationFrom;
            if (currentProvider == null)
                MoveToDetailsViewHelper(message);
            else
                currentProvider.CanNavigateFrom(() => MoveToDetailsViewHelper(message));
        }

        /// <summary>
        /// Moves to details view helper.
        /// </summary>
        /// <param name="message">The message.</param>
        private void MoveToDetailsViewHelper(MoveToDetailsViewMessage message)
        {
            switch (message.Strategy)
            {
                case MoveStrategy.AddContextToExisting:
                    if (CurrentlySelectedContextProvider.SelectedContext != null)
                    {
                        this.CurrentContext.Add(CurrentlySelectedContextProvider.SelectedContext);
                        //Notify that the context was added
                        MessageBus.Current.SendMessage(new AddContextMessage(CurrentlySelectedContextProvider.SelectedContext));
                    }
                    break;
                case MoveStrategy.StartFresh:
                    foreach (var context in CurrentContext.ToArray())
                    {
                        this.CurrentContext.Remove(context);
                        //Notify that the context was removed
                        MessageBus.Current.SendMessage(new RemoveContextMessage(context));
                    }
                    break;
                case MoveStrategy.MoveBackwards:
                    var contextToMoveTo = this.CurrentContext.Where(o => o.GetType().ToString() == message.TypeOfDetailsView).LastOrDefault();
                    if (contextToMoveTo != null)
                    {
                        SetObjectDisplayControl(contextToMoveTo.GetType().ToString(), contextToMoveTo);
                    }
                    break;
            }

            var objectDisplay = ObjectTypeDisplay(message.TypeOfDetailsView);
            if (objectDisplay != null && objectDisplay.ContextProvider != null)
                CurrentlySelectedContextProvider = objectDisplay.ContextProvider;

            //Setup the UI
            SetupUI();
        }

        #endregion

        #region Logic

        //Public

        /// <summary>
        /// Gets the ObjectTypeDisplay for a type
        /// </summary>
        /// <param name="objectTypeToDisplay">The object type to display.</param>
        /// <returns></returns>
        public IObjectTypeDisplay ObjectTypeDisplay(string objectTypeToDisplay)
        {
            var objectDisplayControl =
                ObjectTypeDisplayControls.FirstOrDefault(
                    ltdc => ltdc.ObjectTypeToDisplay == objectTypeToDisplay.ToString());
            return objectDisplayControl;
        }

        #region Private

        /// <summary>
        /// Sets up the UI based on the Context.
        /// </summary>
        private void SetupUI()
        {
            //Clear existing borders
            foreach (var contextBorder in this.ContextBorderControls.Where(cbc => cbc.ContextContent != null))
            {
                contextBorder.ClearInside();
            }

            //Clear control
            this.InfiniteAccordionViewLayoutRoot.Children.Clear();
            this.InfiniteAccordionViewLayoutRoot.InvalidateArrange();

            foreach (var parent in from objectDisplayControl in this.ObjectTypeDisplayControls
                                   where ((FrameworkElement)objectDisplayControl).Parent != null
                                   select (Grid)objectDisplayControl.Display.Parent)
            {
                parent.Children.Clear();
                parent.InvalidateArrange();
            }

            var objectDisplayToDisplay = ObjectTypeDisplay(CurrentlySelectedContextProvider.ObjectTypeProvided.ToString());

            if (CurrentContext.Count <= 0) //No need to setup ContextBorders
            {
                this.InfiniteAccordionViewLayoutRoot.Children.Add((FrameworkElement)objectDisplayToDisplay);
                return;
            }
            //Setup the ContextBorders

            //InnerMostContextBorder is the last in the CurrentContext list
            var innerMostContextBorder = ContextBorderControls.LastOrDefault(contextBorderControl => contextBorderControl.ContextType == CurrentContext[CurrentContext.Count - 1].GetType().ToString());
            innerMostContextBorder.BorderContext = CurrentContext[CurrentContext.Count - 1];

            //Set the inner most ContextBorder's Content to the ObjectDisplay to display
            innerMostContextBorder.ContextContent = (UIElement)objectDisplayToDisplay;

            ContextBorder outerMostContextBorder = innerMostContextBorder;

            foreach (var contextIterator in CurrentContext.Where(c => c != null).Reverse())
            {
                if (contextIterator == innerMostContextBorder.BorderContext) continue; //Skip the first Context

                //Setup the borders inside the OuterMostContextBorder
                var newOuterMostContextBorder = ContextBorderControls.FirstOrDefault(contextBorderControl => contextBorderControl.ContextType == contextIterator.GetType().ToString());
                newOuterMostContextBorder.BorderContext = contextIterator;
                newOuterMostContextBorder.ContextContent = outerMostContextBorder;
                outerMostContextBorder = newOuterMostContextBorder;
            }

            this.InfiniteAccordionViewLayoutRoot.Children.Add(outerMostContextBorder);

            this.InfiniteAccordionViewLayoutRoot.InvalidateArrange();
        }

        /// <summary>
        /// Sets the object display control.
        /// </summary>
        /// <param name="objectDisplayType">Display type of the object.</param>
        /// <param name="removeUntilAfterContext">The context to remove until.</param>
        private void SetObjectDisplayControl(string objectDisplayType, object removeUntilAfterContext = null)
        {
            //Reverse order of the currentContextItems in case removeUntilAfterContext !=null
            //Also copies it to an IEnumerable to prevent collection changed error in the following foreach loop
            var currentContextItemsInReverse = this.CurrentContext.Reverse<object>();

            bool breakAfter = false;

            foreach (var context in currentContextItemsInReverse)
            {
                if (removeUntilAfterContext != null && context == removeUntilAfterContext)
                    breakAfter = true;

                MessageBus.Current.SendMessage(new RemoveContextMessage(context));

                if (breakAfter)
                    break;
            }

            if (objectDisplayType == null) return;

            var objectDisplay =
                ObjectTypeDisplayControls.FirstOrDefault(
                    objectDisplayControl =>
                    objectDisplayControl.ObjectTypeToDisplay == objectDisplayType);

            CurrentlySelectedContextProvider = objectDisplay.ContextProvider;

            MessageBus.Current.SendMessage(new MoveToDetailsViewMessage(CurrentlySelectedContextProvider.ObjectTypeProvided, MoveStrategy.MoveBackwards));
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

        #endregion
    }
}
