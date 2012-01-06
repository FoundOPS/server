using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using System.Windows.Controls.Primitives;
using FoundOps.Common.Silverlight.Converters;
using ItemsControl = System.Windows.Controls.ItemsControl;

namespace FoundOps.Common.Silverlight.UI.Controls.AddEditDelete
{
    public enum AddDeleteMode
    {
        AddDelete, //For adding and deleting only
        AddNewExistingDelete, //For adding and deleting new or existing items
        AddCustomTemplate, //For adding new or existing items. Requires using AddMenuItemTemplate and setting CreateNewItem action and the AddedCurrentItem action
        AddCustomTemplateDelete, //When the Add button is clicked the AddMenuItemCustomTemplate is used. The Delete button operates normally.
        AddDeleteCustomTemplate, //For adding new or existing and deleting items. Requires using AddMenuItemTemplate and setting CreateNewItem action, the RemoveCurrentItem action, and the AddedCurrentItem action
        AddNewItemDeleteCustomTemplate, //For adding and deleting new items. Requires using AddMenuItemTemplate and setting CreateNewItem action, the RemoveCurrentItem action, and the AddedCurrentItem action
        AddItemDelete //For adding and deleting existing items. Requires using ItemsSource and ItemTemplate
    }

    public partial class AddDelete
    {
        #region Public Properties and Variables

        #region Events

        public delegate void AddDeleteEventArgs(AddDelete sender, object item);

        /// <summary>
        /// Occurs when [add] should be executed.
        /// </summary>
        public event AddDeleteEventArgs Add;

        /// <summary>
        /// Occurs when [delete] should be executed.
        /// </summary>
        public event AddDeleteEventArgs Delete;

        #endregion

        #region Dependency Properties

        #region AddDeleteMode

        public AddDeleteMode AddDeleteMode
        {
            get { return (AddDeleteMode)GetValue(AddDeleteModeProperty); }
            set { SetValue(AddDeleteModeProperty, value); }
        }

        public static readonly DependencyProperty AddDeleteModeProperty =
            DependencyProperty.Register("AddDeleteMode", typeof(AddDeleteMode), typeof(AddDelete),
            new PropertyMetadata(AddDeleteMode.AddDelete, new PropertyChangedCallback(OnAddDeleteModeChanged)));

        private Binding _addButtonCommandBinding;

        static void OnAddDeleteModeChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var c = sender as AddDelete;

            if (c == null || e.NewValue == null) return;

            var addDeleteMode = (AddDeleteMode)e.NewValue;

            var addButtonTooltipConverter = new AddButtonTooltipConverter();

            ToolTipService.SetToolTip(c.AddButton, addButtonTooltipConverter.Convert(addDeleteMode, null, null, null));

            //Clear and store AddButtonCommand binding
            c._addButtonCommandBinding = null;
            var addButtonCommandBindingExpression = c.AddButton.GetBindingExpression(ButtonBase.CommandProperty);
            if (addButtonCommandBindingExpression != null)
            {
                c._addButtonCommandBinding = addButtonCommandBindingExpression.ParentBinding;
                c.AddButton.ClearValue(ButtonBase.CommandProperty);
            }

            //Clear ItemTemplate and ItemSource bindings
            c.RadContextMenu.ClearValue(ItemsControl.ItemTemplateProperty);
            c.RadContextMenu.ClearValue(ItemsControl.ItemsSourceProperty);

            c.NewExistingNewMenuItem.Visibility = Visibility.Collapsed;
            c.NewExistingExistingMenuItem.Visibility = Visibility.Collapsed;
            c.CustomAddMenuItem.Visibility = Visibility.Collapsed;

            switch (addDeleteMode)
            {
                case AddDeleteMode.AddDelete:
                    //Resetup AddButton binding
                    if (c._addButtonCommandBinding != null)
                        c.SetBinding(ButtonBase.CommandProperty, c._addButtonCommandBinding);
                    break;
                case AddDeleteMode.AddItemDelete:
                    //Setup ItemSource and ItemTemplate bindings to this control's respective DependencyProperties
                    c.RadContextMenu.SetBinding(ItemsControl.ItemTemplateProperty, new Binding("ItemTemplate") { Source = c });
                    c.RadContextMenu.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ItemsSource") { Source = c });
                    break;
                case AddDeleteMode.AddNewExistingDelete:
                    c.NewExistingNewMenuItem.Visibility = Visibility.Visible;
                    c.NewExistingExistingMenuItem.Visibility = Visibility.Visible;
                    break;
                case AddDeleteMode.AddNewItemDeleteCustomTemplate:
                    c.CustomAddMenuItem.Visibility = Visibility.Visible;
                    break;
                case AddDeleteMode.AddCustomTemplate:
                    c.CustomAddMenuItem.Visibility = Visibility.Visible;
                    c.DeleteButton.IsEnabled = false;
                    break;
                case AddDeleteMode.AddCustomTemplateDelete:
                    c.CustomAddMenuItem.Visibility = Visibility.Visible;
                    c.DeleteButton.IsEnabled = true;
                    break;
                case AddDeleteMode.AddDeleteCustomTemplate:
                    c.CustomAddMenuItem.Visibility = Visibility.Visible;
                    c.DeleteButton.IsEnabled = true;
                    break;
            }
        }

        #endregion

        #region AddIsEnabled Dependency Property

        /// <summary>
        /// AddIsEnabled
        /// </summary>
        public bool AddIsEnabled
        {
            get { return (bool) GetValue(AddIsEnabledProperty); }
            set { SetValue(AddIsEnabledProperty, value); }
        }

        /// <summary>
        /// AddIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddIsEnabledProperty =
            DependencyProperty.Register(
                "AddIsEnabled",
                typeof (bool),
                typeof (AddDelete),
                new PropertyMetadata(true));

        #endregion
        #region DeleteIsEnabled Dependency Property

        /// <summary>
        /// DeleteIsEnabled
        /// </summary>
        public bool DeleteIsEnabled
        {
            get { return (bool) GetValue(DeleteIsEnabledProperty); }
            set { SetValue(DeleteIsEnabledProperty, value); }
        }

        /// <summary>
        /// DeleteIsEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DeleteIsEnabledProperty =
            DependencyProperty.Register(
                "DeleteIsEnabled",
                typeof (bool),
                typeof (AddDelete),
                new PropertyMetadata(true));

        #endregion

        #region Commands

        #region AddCommand Dependency Property

        /// <summary>
        /// AddCommand
        /// </summary>
        public ICommand AddCommand
        {
            get { return (ICommand)GetValue(AddCommandProperty); }
            set { SetValue(AddCommandProperty, value); }
        }

        /// <summary>
        /// AddCommand Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddCommandProperty =
            DependencyProperty.Register(
                "AddCommand",
                typeof(ICommand),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion

        #region AddItemCommand Dependency Property

        /// <summary>
        /// AddExistingCommand
        /// </summary>
        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        /// <summary>
        /// AddExistingCommand Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register(
                "AddItemCommand",
                typeof(ICommand),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion

        #region CommandParameter Dependency Property

        /// <summary>
        /// CommandParameter
        /// </summary>
        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// CommandParameter Dependency Property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion

        #region DeleteCommand Dependency Property

        /// <summary>
        /// DeleteCommand
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return (ICommand)GetValue(DeleteCommandProperty); }
            set { SetValue(DeleteCommandProperty, value); }
        }

        /// <summary>
        /// DeleteCommand Dependency Property.
        /// </summary>
        public static readonly DependencyProperty DeleteCommandProperty =
            DependencyProperty.Register(
                "DeleteCommand",
                typeof(ICommand),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion

        #endregion

        #region Templates

        #region AddMenuItemCustomTemplate Dependency Property

        /// <summary>
        /// AddMenuItemCustomTemplate
        /// </summary>
        public DataTemplate AddMenuItemCustomTemplate
        {
            get { return (DataTemplate)GetValue(AddMenuItemCustomTemplateProperty); }
            set { SetValue(AddMenuItemCustomTemplateProperty, value); }
        }

        /// <summary>
        /// AddMenuItemCustomTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty AddMenuItemCustomTemplateProperty =
            DependencyProperty.Register(
                "AddMenuItemCustomTemplate",
                typeof(DataTemplate),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion
        #region ItemTemplate Dependency Property

        /// <summary>
        /// ExistingItemTemplate
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// ExistingItemTemplate Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(
                "ItemTemplate",
                typeof(DataTemplate),
                typeof(AddDelete),
                new PropertyMetadata(new PropertyChangedCallback(ItemTemplateChanged)));

        private static void ItemTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            AddDelete c = d as AddDelete;
            if (c != null)
            {
                if (e.NewValue != null)
                {
                    var dataTemplate = (DataTemplate)e.NewValue;
                    ContainerBinding.SetContainerBindings(dataTemplate, (ContainerBindingCollection)c.Resources["CommandParameterBinding"]);
                }
            }
        }

        #endregion

        #endregion

        #region ItemsSource Dependency Property

        /// <summary>
        /// ExistingItemsSource
        /// </summary>
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        /// <summary>
        /// ExistingItemsSource Dependency Property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(AddDelete),
                new PropertyMetadata(null));

        #endregion

        #endregion

        #endregion

        public AddDelete()
        {
            InitializeComponent();
            RadContextMenu.DataContext = this;

            AddButton.Click += (s, args) =>
            {
                if ((AddDeleteMode != AddDeleteMode.AddNewItemDeleteCustomTemplate) && (AddDeleteMode != AddDeleteMode.AddCustomTemplate) && (AddDeleteMode != AddDeleteMode.AddCustomTemplateDelete) && (AddDeleteMode != AddDeleteMode.AddDeleteCustomTemplate) || RadContextMenu.IsOpen)
                    return;

                //When the AddButton is clicked open the RadContextMenu and
                RadContextMenu.IsOpen = true;

                //create the default item
                if (CreateNewItem != null)
                    CustomAddMenuItem.Content = CreateNewItem.Invoke();
            };

            RadContextMenu.Closed += (s, args) =>
            {
                if ((AddDeleteMode != AddDeleteMode.AddNewItemDeleteCustomTemplate) && (AddDeleteMode != AddDeleteMode.AddCustomTemplate) && (AddDeleteMode != AddDeleteMode.AddCustomTemplateDelete) && (AddDeleteMode != AddDeleteMode.AddDeleteCustomTemplate)) return;

                var item = CustomAddMenuItem.Content;

                //When the RadContextMenu is closed

                //if the currentItem was not added, remove it
                if (!_currentItemAdded)
                {
                    if (RemoveCurrentItem != null)
                    {
                        RemoveCurrentItem.Invoke(item);
                        return;
                    }
                }

                //if the currentItem was added, call Add && AddCommand.Execute
                _currentItemAdded = false;

                //Call AddHelper
                AddHelper();
            };
        }

        #region TODO: Remove below two function, DEPRECATED. Use AddToDeleteFrom

        private bool _currentItemAdded;

        public Func<object> CreateNewItem { get; set; }
        public Action<object> RemoveCurrentItem { get; set; }

        /// <summary>
        /// Call this for the AddDeleteMode.AddNewItemDeleteCustomTemplate or AddDeleteCustomTemplate when you have added the item.
        /// This will call AddCommand.Execute and close the RadContextMenu.
        /// </summary>
        public void AddedCurrentItem()
        {
            if (AddDeleteMode != AddDeleteMode.AddNewItemDeleteCustomTemplate && AddDeleteMode != AddDeleteMode.AddCustomTemplate && AddDeleteMode != AddDeleteMode.AddDeleteCustomTemplate) return;

            _currentItemAdded = true;
            this.RadContextMenu.IsOpen = false;
        }

        /// <summary>
        /// Call this for the AddDeleteMode.AddNewItemDeleteCustomTemplate or AddDeleteCustomTemplate when you want to close the context menu.
        /// This will call RemoveCurrentItem and close the RadContextMenu.
        /// </summary>
        public void CloseContextMenu()
        {
            if (AddDeleteMode != AddDeleteMode.AddNewItemDeleteCustomTemplate && AddDeleteMode != AddDeleteMode.AddCustomTemplate && AddDeleteMode != AddDeleteMode.AddDeleteCustomTemplate) return;

            _currentItemAdded = true;
            this.RadContextMenu.IsOpen = false;
        }

        #endregion

        #region Logic

        /// <summary>
        /// Executes the AddCommand and calls the Add event
        /// </summary>
        private void AddHelper()
        {
            if (AddCommand != null)
                AddCommand.Execute(null);

            if (Add != null)
                Add(this, null);
        }

        private void AddButtonClick(object sender, RoutedEventArgs e)
        {
            if (AddDeleteMode == AddDeleteMode.AddItemDelete || AddDeleteMode == AddDeleteMode.AddNewExistingDelete)
                RadContextMenu.IsOpen = true;

            //Call AddHelper if the proper mode
            if (AddDeleteMode == AddDeleteMode.AddNewItemDeleteCustomTemplate || AddDeleteMode == AddDeleteMode.AddCustomTemplate || AddDeleteMode == AddDeleteMode.AddDeleteCustomTemplate) return;
            AddHelper();
        }

        /// Executes the DeleteCommand and calls the Delete event
        private void DeleteButtonClick(object sender, RoutedEventArgs e)
        {
            if (DeleteCommand != null)
                DeleteCommand.Execute(null);

            if (Delete != null)
                Delete(this, null);
        }

        #endregion
    }
}