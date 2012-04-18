using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AddEditDelete
{
    #region Mode Enums

    public enum AddMode
    {
        /// <summary>
        /// Hides the add button.
        /// </summary>
        None,
        /// <summary>
        /// Calls the AddCommand.
        /// </summary>
        Add,
        /// <summary>
        /// Displays the Add Existing menu
        /// </summary>
        AddExisting,
        /// <summary>
        /// Displays the Add New/Add Existing menu
        /// </summary>
        AddNewExisting
    }

    public enum DeleteMode
    {
        /// <summary>
        /// Hides the delete button.
        /// </summary>
        None,
        /// <summary>
        /// Calls the DeleteCommand.
        /// </summary>
        Delete,
        /// <summary>
        /// Prompts the delete prompt, then calls the DeleteCommand.
        /// </summary>
        DeletePrompt,
        /// <summary>
        /// Prompts the remove prompt, then either removes it from the context or deletes it.
        /// </summary>
        RemoveOrDeletePrompt
    }

    #endregion

    #region Interfaces

    //Source interface
    /// <summary>
    /// The source of items for the AddToDeleteFrom control
    /// </summary>
    /// <typeparam name="T">The type of source item.</typeparam>
    public interface IAddToDeleteFromSource<out T> : INotifyPropertyChanged
    {
        /// <summary>
        /// A method to create a new item. It is passed the RadComboBox string.
        /// </summary>
        Func<string, T> CreateNewItem { get; }

        /// <summary>
        /// The member path to be used for searching and displaying existing items.
        /// </summary>
        string MemberPath { get; }

        /// <summary>
        /// A method to update the AddToDeleteFrom's AutoCompleteBox with suggestions remotely loaded.
        /// </summary>
        Action<AutoCompleteBox> ManuallyUpdateSuggestions { get; }
    }

    #region Destination interfaces

    /// <summary>
    /// The destination of items for the AddToDeleteFrom control
    /// </summary>
    /// <typeparam name="T">The type of source item this is a destination for.</typeparam>
    public interface IAddToDeleteFromDestination<out T> : INotifyPropertyChanged
    {
        /// <summary>
        /// Commented out because this should be implemented
        /// Returns the destination items source. This is not generic because of inheritance issues and entitylists.
        /// </summary>
        /// IEnumerable DestinationItemsSource { get; }

        /// <summary>
        /// Subscribes to the add delete from control observables.
        /// Workaround for wrestling with the type system.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">The source type of the AddToDeleteFrom control. </param>
        void LinkToAddToDeleteFrom(AddToDeleteFrom control, Type sourceType);

        /// Disposes the subscriptions to the add delete from control observables.
        /// Workaround for wrestling with the type system.
        /// </summary>
        /// <param name="control">The control.</param>
        /// <param name="sourceType">The source type of the AddToDeleteFrom control. </param>
        void UnlinkAddToDeleteFrom(AddToDeleteFrom c, Type type);
    }

    #region Add interfaces

    /// <summary>
    /// A destination which adds new items.
    /// </summary>
    /// <typeparam name="T">The type of items it can add.</typeparam>
    public interface IAddNew<out T>
    {
        /// <summary>
        /// A method to add a new item. It is passed the RadComboBox string.
        /// </summary>
        Func<string, T> AddNewItem { get; }
    }

    /// <summary>
    /// A destination which adds new and existing items.
    /// </summary>
    /// <typeparam name="T">The type of items it can add.</typeparam>
    public interface IAddNewExisting<T> : IAddNew<T>
    {
        /// <summary>
        /// A method to add an existing item. It is passed the selected existing item.
        /// </summary>
        Action<object> AddExistingItem { get; }
    }

    #endregion

    #region Remove and Delete interfaces

    /// <summary>
    /// A destination which removes items.
    /// </summary>
    /// <typeparam name="T">The type of items it can remove.</typeparam>
    public interface IRemove<out T>
    {
        /// <summary>
        /// A method to remove the current item.
        /// Returns the removed item.
        /// </summary>
        Func<T> RemoveItem { get; }
    }

    /// <summary>
    /// A destination which removes and delete items.
    /// </summary>
    /// <typeparam name="T">The type of items it can remove and delete.</typeparam>
    public interface IRemoveDelete<out T> : IRemove<T>
    {
        /// <summary>
        /// A method to delete the current item.
        /// Returns the deleted item.
        /// </summary>
        Func<T> DeleteItem { get; }
    }

    #endregion

    #endregion

    #endregion

    /// <summary>
    /// A control to allow adding to and deleting from an enumerable of objects.
    /// </summary>
    public partial class AddToDeleteFrom
    {
        #region Public Properties

        #region Add, Remove and Delete properties

        #region IsDeleteEnabled Dependency Property

        /// <summary>
        /// IsDeleteEnabled
        /// </summary>
        public bool IsDeleteEnabled
        {
            get { return (bool)GetValue(IsDeleteEnabledProperty); }
            set { SetValue(IsDeleteEnabledProperty, value); }
        }

        /// <summary>
        /// IsDeleteEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsDeleteEnabledProperty =
            DependencyProperty.Register(
                "IsDeleteEnabled",
                typeof(bool),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(new PropertyChangedCallback(IsDeleteEnabledChanged)));

        private static void IsDeleteEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c != null)
                c.AddDelete.DeleteIsEnabled = (bool)e.NewValue;
        }

        #endregion

        #region ItemToRemoveDisplayMember Dependency Property

        /// <summary>
        /// A title of the current item that is being added to or removed.
        /// </summary>
        public string ItemToRemoveDisplayMember
        {
            get { return (string)GetValue(ItemToRemoveDisplayMemberProperty); }
            set { SetValue(ItemToRemoveDisplayMemberProperty, value); }
        }

        /// <summary>
        /// ItemToRemoveDisplayMember Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemToRemoveDisplayMemberProperty =
            DependencyProperty.Register(
                "ItemToRemoveDisplayMember",
                typeof(string),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion
        #region ItemToRemoveFromDisplayMember Dependency Property

        /// <summary>
        /// A title of the current item that is being added to or removed from.
        /// </summary>
        public string ItemToRemoveFromDisplayMember
        {
            get { return (string)GetValue(ItemToRemoveFromDisplayMemberProperty); }
            set { SetValue(ItemToRemoveFromDisplayMemberProperty, value); }
        }

        /// <summary>
        /// EntityToRemoveFromString Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemToRemoveFromDisplayMemberProperty =
            DependencyProperty.Register(
                "ItemToRemoveFromDisplayMember",
                typeof(string),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #endregion

        #region Data source, destination, and selected items

        /// <summary>
        /// Gets or sets the type of the source. 
        /// Used for linking the Destination and the Destination's DestinationItemsSource.
        /// </summary>
        public Type SourceType { get; set; }

        #region Source Dependency Property

        /// <summary>
        /// Source
        /// </summary>
        public IAddToDeleteFromSource<object> Source
        {
            get { return (IAddToDeleteFromSource<object>)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        /// <summary>
        /// Source Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(IAddToDeleteFromSource<object>),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region Destination Dependency Property

        /// <summary>
        /// Destination
        /// </summary>
        public IAddToDeleteFromDestination<object> Destination
        {
            get { return (IAddToDeleteFromDestination<object>)GetValue(DestinationProperty); }
            set { SetValue(DestinationProperty, value); }
        }

        /// <summary>
        /// Destination Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DestinationProperty =
            DependencyProperty.Register(
                "Destination",
                typeof(IAddToDeleteFromDestination<object>),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(DestinationChanged));

        private static void DestinationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c == null) return;

            if (e.OldValue != null)
                ((IAddToDeleteFromDestination<object>)e.OldValue).UnlinkAddToDeleteFrom(c, c.SourceType);

            if (e.NewValue == null || c.SourceType == null) return;
            ((IAddToDeleteFromDestination<object>)e.NewValue).LinkToAddToDeleteFrom(c, c.SourceType);
        }


        #endregion
        #region DestinationItemsSource Dependency Property

        /// <summary>
        /// DestinationItemsSource.
        /// This is a seperate dependency property from Destination because explicit property bindings do not work in silverlight.
        /// </summary>
        public IEnumerable DestinationItemsSource
        {
            get { return (IEnumerable)GetValue(DestinationItemsSourceProperty); }
            set { SetValue(DestinationItemsSourceProperty, value); }
        }

        /// <summary>
        /// DestinationItemsSource Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DestinationItemsSourceProperty =
            DependencyProperty.Register(
                "DestinationItemsSource",
                typeof(IEnumerable),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region ExistingItemsComboBoxText Dependency Property

        /// <summary>
        /// ExistingItemsComboBoxText
        /// </summary>
        public string ExistingItemsComboBoxText
        {
            get { return (string)GetValue(ExistingItemsComboBoxTextProperty); }
            set { SetValue(ExistingItemsComboBoxTextProperty, value); }
        }

        /// <summary>
        /// ExistingItemsComboBoxText Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ExistingItemsComboBoxTextProperty =
            DependencyProperty.Register(
                "ExistingItemsComboBoxText",
                typeof(string),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion
        #region SelectedExistingItem Dependency Property

        /// <summary>
        /// The SelectedExistingItem in the Add New/Existing combobox
        /// </summary>
        public object SelectedExistingItem
        {
            get { return GetValue(SelectedExistingItemProperty); }
            set { SetValue(SelectedExistingItemProperty, value); }
        }

        /// <summary>
        /// SelectedExistingItem Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedExistingItemProperty =
            DependencyProperty.Register(
                "SelectedExistingItem",
                typeof(object),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #endregion

        #region Events

        private Subject<string> _addNewItem = new Subject<string>();
        /// <summary>
        /// Pushes the RadComboBox text when [add new item] is clicked.
        /// </summary>
        public IObservable<string> AddNewItem { get { return _addNewItem; } }

        private Subject<object> _addExistingItem = new Subject<object>();
        /// <summary>
        /// Pushes the selected item when [add existing item] is clicked.
        /// </summary>
        public IObservable<object> AddExistingItem { get { return _addExistingItem; } }

        private Subject<bool> _removeItem = new Subject<bool>();
        /// <summary>
        /// Pushes when [remove item] is clicked.
        /// </summary>
        public IObservable<bool> RemoveItem { get { return _removeItem; } }

        private Subject<bool> _deleteItem = new Subject<bool>();
        /// <summary>
        /// Pushes when [delete item] is clicked.
        /// </summary>
        public IObservable<bool> DeleteItem { get { return _deleteItem; } }

        #endregion

        #region Label Dependency Property

        /// <summary>
        /// Label
        /// </summary>
        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        /// <summary>
        /// Label Dependency Property.
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register(
                "Label",
                typeof(string),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(""));

        #endregion

        #region Modes

        #region AddMode Dependency Property

        /// <summary>
        /// AddMode
        /// </summary>
        public AddMode AddMode
        {
            get { return (AddMode)GetValue(AddModeProperty); }
            set { SetValue(AddModeProperty, value); }
        }

        /// <summary>
        /// AddMode Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddModeProperty =
            DependencyProperty.Register(
                "AddMode",
                typeof(AddMode),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(AddMode.Add, new PropertyChangedCallback(AddModeChanged)));

        private static void AddModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c == null) return;

            var addMode = (AddMode)e.NewValue;

            //Set the AddDelete's AddButton visibility
            c.AddDelete.AddButton.Visibility = addMode == AddMode.None
                                                   ? Visibility.Collapsed
                                                   : Visibility.Visible;

            //Set the AddDelete's AddDeleteMode
            c.AddDelete.AddDeleteMode = addMode == AddMode.AddExisting || addMode == AddMode.AddNewExisting
                                            ? AddDeleteMode.AddCustomTemplateDelete
                                            : AddDeleteMode.AddDelete;
        }

        #endregion
        #region DeleteMode Dependency Property

        /// <summary>
        /// DeleteMode
        /// </summary>
        public DeleteMode DeleteMode
        {
            get { return (DeleteMode)GetValue(DeleteModeProperty); }
            set { SetValue(DeleteModeProperty, value); }
        }

        /// <summary>
        /// DeleteMode Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DeleteModeProperty =
            DependencyProperty.Register(
                "DeleteMode",
                typeof(DeleteMode),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(new PropertyChangedCallback(DeleteModeChanged)));

        private static void DeleteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c == null) return;

            var deleteMode = (AddMode)e.NewValue;

            //Set the DeleteButton visibility
            c.AddDelete.DeleteButton.Visibility = deleteMode == AddMode.None ? Visibility.Collapsed : Visibility.Visible;
        }

        #endregion

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToDeleteFrom"/> class.
        /// </summary>
        public AddToDeleteFrom()
        {
            InitializeComponent();
        }

        #region Logic

        #region AddDelete control

        private void AddDelete_OnAdd(AddDelete sender, object item)
        {
            //The AddMode.None should do nothing and
            //the AddNewExisting logic is handled in the AddMode.AddNewExisting region below
            if (AddMode != AddMode.Add) return;

            AddNewItemHelper();
        }

        private void AddDelete_OnDelete(AddDelete sender, object item)
        {
            //The DeleteMode.None should do nothing

            if (DeleteMode == DeleteMode.Delete)
                DeleteItemHelper();

            if (DeleteMode == DeleteMode.DeletePrompt)
            {
                if (MessageBox.Show(String.Format("Are you sure you want to delete {0}?", ItemToRemoveDisplayMember), String.Format("Delete {0}?", ItemToRemoveDisplayMember), MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    return;

                //Since the user clicked OK, delete the item
                DeleteItemHelper();
            }

            if (DeleteMode == DeleteMode.RemoveOrDeletePrompt)
            {
                //Setup the RemoveDeleteCancel prompt
                var removeDeleteCancel = new RemoveDeleteCancel
                {
                    ItemToRemoveString = ItemToRemoveDisplayMember,
                    ItemToRemoveFromString = ItemToRemoveFromDisplayMember
                };

                removeDeleteCancel.RemoveButton.Click += (s, e) =>
                {
                    RemoveItemHelper();
                    removeDeleteCancel.Close();
                };
                removeDeleteCancel.DeleteButton.Click += (s, e) =>
                {
                    DeleteItemHelper();
                    removeDeleteCancel.Close();
                };

                //If the user clicked Cancel, do nothing and close the window
                removeDeleteCancel.CancelButton.Click += (s, e) => removeDeleteCancel.Close();

                //Now that the RemoveDeleteCancel is setup, show it to the user
                removeDeleteCancel.Show();
            }
        }

        #endregion

        #region AddMode.AddNewExisting Mode logic

        private void AddNewButtonClick(object sender, RoutedEventArgs e)
        {
            AddNewItemHelper();
        }

        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {
            AddExistingItemHelper();
        }

        #endregion

        #region Add, Remove, Delete helpers

        /// <summary>
        /// Pushes the combobox text to the AddNewItem observable
        /// </summary>
        private void AddNewItemHelper()
        {
            _addNewItem.OnNext(ExistingItemsComboBoxText);
        }

        /// <summary>
        /// Pushes the selected entity (from the combobox) to the AddNewItem AddExistingItem observable
        /// </summary>
        private void AddExistingItemHelper()
        {
            _addExistingItem.OnNext(SelectedExistingItem);
        }

        /// <summary>
        /// Pushes true on the remove item observable.
        /// </summary>
        private void RemoveItemHelper()
        {
            _removeItem.OnNext(true);
        }

        /// <summary>
        /// Pushes true on the delete item observable
        /// </summary>
        private void DeleteItemHelper()
        {
            _deleteItem.OnNext(true);
        }

        #endregion

        #endregion

        /// <summary>
        /// Handles the Populating event of the AutoCompleteBox control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Controls.PopulatingEventArgs"/> instance containing the event data.</param>
        private void AutoCompleteBoxPopulating(object sender, PopulatingEventArgs e)
        {
            if (Source == null || Source.ManuallyUpdateSuggestions == null) return;

            e.Cancel = true;
            Source.ManuallyUpdateSuggestions((AutoCompleteBox)sender);
        }

        private void AutoCompleteBoxOnGotFocus(object sender, RoutedEventArgs e)
        {
            ((AutoCompleteBox)sender).IsDropDownOpen = true;
        }
    }
}