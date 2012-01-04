using System;
using System.Collections;
using System.Windows;

namespace FoundOps.Common.Silverlight.UI.Controls.AddEditDelete
{
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

    /// <summary>
    /// A control to allow adding to and deleting from an IList of objects.
    /// </summary>
    public partial class AddToDeleteFrom
    {
        #region AddMode Dependency Property

        /// <summary>
        /// AddMode
        /// </summary>
        public AddMode AddMode
        {
            get { return (AddMode) GetValue(AddModeProperty); }
            set { SetValue(AddModeProperty, value); }
        }

        /// <summary>
        /// AddMode Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddModeProperty =
            DependencyProperty.Register(
                "AddMode",
                typeof (AddMode),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(new PropertyChangedCallback(AddModeChanged)));

        private static void AddModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c == null) return;

            var addMode = (AddMode) e.NewValue;

            //Set the DeleteButton visibility
            c.AddDelete.DeleteButton.Visibility = addMode == AddMode.None ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region DeleteMode Dependency Property

        /// <summary>
        /// DeleteMode
        /// </summary>
        public DeleteMode DeleteMode
        {
            get { return (DeleteMode) GetValue(DeleteModeProperty); }
            set { SetValue(DeleteModeProperty, value); }
        }

        /// <summary>
        /// DeleteMode Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DeleteModeProperty =
            DependencyProperty.Register(
                "DeleteMode",
                typeof (DeleteMode),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(new PropertyChangedCallback(DeleteModeChanged)));

        private static void DeleteModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var c = d as AddToDeleteFrom;
            if (c == null) return;
            
            var deleteMode = (AddMode)e.NewValue;

            //Set the DeleteButton visibility
            c.AddDelete.DeleteButton.Visibility = deleteMode == AddMode.None ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region DisplayMemberPath Dependency Property

        /// <summary>
        /// DisplayMemberPath
        /// </summary>
        public string DisplayMemberPath
        {
            get { return (string) GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }

        /// <summary>
        /// DisplayMemberPath Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(
                "DisplayMemberPath",
                typeof (string),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region ExistingItemsSource Dependency Property

        /// <summary>
        /// ExistingItemsSource
        /// </summary>
        public IEnumerable ExistingItemsSource
        {
            get { return (IEnumerable) GetValue(ExistingItemsSourceProperty); }
            set { SetValue(ExistingItemsSourceProperty, value); }
        }

        /// <summary>
        /// ExistingItemsSource Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ExistingItemsSourceProperty =
            DependencyProperty.Register(
                "ExistingItemsSource",
                typeof (IEnumerable),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        /// <summary>
        /// The function called when creating a new item.
        /// It passes the value of the combobox text.
        /// </summary>
        public Func<string, object> CreateNewItem { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AddToDeleteFrom"/> class.
        /// </summary>
        public AddToDeleteFrom()
        {
            InitializeComponent();
        }
    }
}