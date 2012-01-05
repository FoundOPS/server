﻿using System;
using System.Windows;
using System.Collections;
using System.ComponentModel;

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

    public interface IAddToDeleteFromProvider : INotifyPropertyChanged
    {
        /// <summary>
        /// Returns the existing items.
        /// </summary>
        IEnumerable ExistingItemsSource { get; }

        /// <summary>
        /// The function is called to create a new item.
        /// It is passed the value of the combobox text.
        /// Note: Make sure to add it to a list for tracking as well if necessary for storage.
        /// </summary>
        Func<string, object> CreateNewItemFromString { get; }

        /// <summary>
        /// The member path to be used for searching and displaying existing items.
        /// </summary>
        string MemberPath { get; }
    }

    /// <summary>
    /// A control to allow adding to and deleting from an enumerable of objects.
    /// </summary>
    public partial class AddToDeleteFrom
    {
        #region Public Properties

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
                                                   ? Visibility.Visible
                                                   : Visibility.Collapsed;

            //Set the AddDelete's AddDeleteMode
            c.AddDelete.AddDeleteMode = addMode != AddMode.AddNewExisting
                                            ? AddDeleteMode.AddDelete
                                            : AddDeleteMode.AddCustomTemplateDelete;

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
            c.AddDelete.DeleteButton.Visibility = deleteMode == AddMode.None ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion

        #region Collection Dependency Property

        /// <summary>
        /// The collection to add objects to or remove from.
        /// </summary>
        public IEnumerable Collection
        {
            get { return (IEnumerable)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Collection Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(
                "Collection",
                typeof(IEnumerable),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region Provider Dependency Property

        /// <summary>
        /// A provider of the objects to Create New or choose Existing from.
        /// </summary>
        public IAddToDeleteFromProvider Provider
        {
            get { return (IAddToDeleteFromProvider)GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        /// <summary>
        /// Provider Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ProviderProperty =
            DependencyProperty.Register(
                "Provider",
                typeof(IAddToDeleteFromProvider),
                typeof(AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region SelectedExistingItem Dependency Property

        /// <summary>
        /// SelectedExistingItem
        /// </summary>
        public object SelectedExistingItem
        {
            get { return (object) GetValue(SelectedExistingItemProperty); }
            set { SetValue(SelectedExistingItemProperty, value); }
        }

        /// <summary>
        /// SelectedExistingItem Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedExistingItemProperty =
            DependencyProperty.Register(
                "SelectedExistingItem",
                typeof (object),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(null));

        #endregion

        #region ExistingItemsComboBoxText Dependency Property

        /// <summary>
        /// ExistingItemsComboBoxText
        /// </summary>
        public string ExistingItemsComboBoxText
        {
            get { return (string) GetValue(ExistingItemsComboBoxTextProperty); }
            set { SetValue(ExistingItemsComboBoxTextProperty, value); }
        }

        /// <summary>
        /// ExistingItemsComboBoxText Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ExistingItemsComboBoxTextProperty =
            DependencyProperty.Register(
                "ExistingItemsComboBoxText",
                typeof (string),
                typeof (AddToDeleteFrom),
                new PropertyMetadata(null));

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
            switch (AddMode)
            {
                case AddMode.None:
                    return;
                case AddMode.Add:

                    break;
                case AddMode.AddNewExisting:
                    return; //This logic is handled in the AddMode.AddNewExisting region below
            }
        }

        private void AddDelete_OnDelete(AddDelete sender, object item)
        {
            switch (DeleteMode)
            {
                case DeleteMode.None:
                    return;
                case DeleteMode.Delete:

                    break;
                case DeleteMode.DeletePrompt:

                    break;
                case DeleteMode.RemoveOrDeletePrompt:

                    break;
            }
        }

        #endregion

        #region AddMode.AddNewExisting

        private void AddNewButtonClick(object sender, RoutedEventArgs e)
        {
            if(Provider == null)return;

            Provider.CreateNewItemFromString("");

            dynamic addableCollection = Collection;
            addableCollection.Add(SelectedExistingItem);
        }

        private void AddExistingButtonClick(object sender, RoutedEventArgs e)
        {
            if (SelectedExistingItem == null) return;

            dynamic addableCollection = Collection;
            addableCollection.Add(SelectedExistingItem);
        }

        #endregion

        #endregion
    }
}