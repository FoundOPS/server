using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;
using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
    /// <summary>
    /// A behavior to add to a ComboBox to provide suggestions while entering in text.
    /// </summary>
    public class AutocompleteBehavior
    {
        /// <summary>
        /// A delay (throttle) to set of calling GetItems. 
        /// </summary>
        public static readonly DependencyProperty GetItemsDelayProperty =
            DependencyProperty.RegisterAttached("GetItemsDelay", typeof(TimeSpan), typeof(AutocompleteBehavior), new PropertyMetadata(OnGetItemsDelayChanged));

        /// <summary>
        /// The minimum text length before triggering a GetItems.
        /// </summary>
        public static readonly DependencyProperty MinimumTextLengthProperty =
            DependencyProperty.RegisterAttached("MinimumTextLength", typeof(int), typeof(AutocompleteBehavior), new PropertyMetadata(OnMinimumTextLengthChanged));

        /// <summary>
        /// An AutocompleteProvider for retrieving items based on text.
        /// </summary>
        public static readonly DependencyProperty AutocompleteProviderProperty =
            DependencyProperty.RegisterAttached("AutocompleteProvider", typeof(IAutocompleteProvider), typeof(AutocompleteBehavior), new PropertyMetadata(OnAutocompleteProviderChanged));

        /// <summary>
        /// An (optional) itemsource of the suggestions. 
        /// NOTE: Cannot use this and a AutocompleteProvider.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable), typeof(AutocompleteBehavior), new PropertyMetadata(OnItemsSourceChanged));

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //TODO: Working on this to make it works
            //Add the SelectedValue to the Suggestions ItemSource so that it does not show up as empty

            //Remove the last SelectedValue from the suggestions
            if(e.OldValue!=null)
                GetAutocompleteViewModel(d as RadComboBox).Suggestions.Remove(e.OldValue);

            //Add the SelectedValue to the suggestions
            if (e.NewValue != null)
                GetAutocompleteViewModel(d as RadComboBox).Suggestions.Add(e.NewValue);
        }

        private static readonly DependencyProperty AutocompleteViewModelProperty =
            DependencyProperty.RegisterAttached("AutocompleteViewModel", typeof(AutocompleteViewModel), typeof(AutocompleteBehavior), null);

        public static TimeSpan GetGetItemsDelay(DependencyObject obj)
        {
            return (TimeSpan)obj.GetValue(GetItemsDelayProperty);
        }

        public static void SetGetItemsDelay(DependencyObject obj, TimeSpan value)
        {
            obj.SetValue(GetItemsDelayProperty, value);
        }

        public static int GetMinimumTextLength(DependencyObject d)
        {
            return (int)d.GetValue(MinimumTextLengthProperty);
        }

        public static void SetMinimumTextLength(DependencyObject d, int value)
        {
            d.SetValue(MinimumTextLengthProperty, value);
        }

        public static IEnumerable GetItemsSource(DependencyObject d)
        {
            return (IEnumerable)d.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(DependencyObject d, IEnumerable value)
        {
            d.SetValue(ItemsSourceProperty, value);
        }

        public static IAutocompleteProvider GetAutocompleteProvider(DependencyObject obj)
        {
            return (IAutocompleteProvider)obj.GetValue(AutocompleteProviderProperty);
        }

        public static void SetAutocompleteProvider(DependencyObject d, IAutocompleteProvider value)
        {
            d.SetValue(AutocompleteProviderProperty, value);
        }

        private static void OnGetItemsDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetAutocompleteViewModel(d as RadComboBox).GetItemsDelay = (TimeSpan)e.NewValue;
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetValue(AutocompleteProviderProperty) != null)
            {
                ThrowCannotUseBothAutocompleteProviderAndItemsSourceException();
            }

            var collectionView = e.NewValue as ICollectionView;
            if (collectionView == null)
            {
                var collectionViewSource = new CollectionViewSource { Source = e.NewValue };
                collectionView = collectionViewSource.View;
            }
            GetAutocompleteViewModel(d as RadComboBox).AutocompleteProvider = new CollectionViewProvider(collectionView);
        }

        private static void OnAutocompleteProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d.GetValue(ItemsSourceProperty) != null)
            {
                ThrowCannotUseBothAutocompleteProviderAndItemsSourceException();
            }

            GetAutocompleteViewModel(d as RadComboBox).AutocompleteProvider = (IAutocompleteProvider)e.NewValue;
        }

        private static void OnMinimumTextLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GetAutocompleteViewModel(d as RadComboBox).MinimumTextLength = (int)e.NewValue;
        }

        private static AutocompleteViewModel GetAutocompleteViewModel(RadComboBox comboBox)
        {
            var model = comboBox.GetValue(AutocompleteViewModelProperty) as AutocompleteViewModel;
            if (model == null)
            {
                model = new AutocompleteViewModel();

                comboBox.SetValue(AutocompleteViewModelProperty, model);

                comboBox.IsEditable = true;
                comboBox.IsTextSearchEnabled = false;
                comboBox.CanAutocompleteSelectItems = false;
                comboBox.CanKeyboardNavigationSelectItems = false;
                comboBox.ItemsSource = model.Suggestions;

                BindingOperations.SetBinding(model, AutocompleteViewModel.TextProperty, new Binding("Text") { Source = comboBox, Mode = BindingMode.TwoWay });
                BindingOperations.SetBinding(model, AutocompleteViewModel.SelectedItemProperty, new Binding("SelectedItem") { Source = comboBox, Mode = BindingMode.TwoWay });
                BindingOperations.SetBinding(model, AutocompleteViewModel.IsDropDownOpenProperty, new Binding("IsDropDownOpen") { Source = comboBox, Mode = BindingMode.TwoWay });
                BindingOperations.SetBinding(model, AutocompleteViewModel.TextSearchModeProperty, new Binding("TextSearchMode") { Source = comboBox, Mode = BindingMode.TwoWay });

                //Keep track of SelectedValue changes
                comboBox.RegisterForNotification("SelectedValue", OnSelectedValueChanged);

                // In Silverlight you cannot bind a Dependency property to Attached property in code-behind! So we are doing the oposite.
                model.TextPath = TextSearch.GetTextPath(comboBox);
                BindingOperations.SetBinding(comboBox, TextSearch.TextPathProperty, new Binding { Path = new PropertyPath("TextPath"), Source = model, Mode = BindingMode.TwoWay });
            }
            return model;
        }

        private static void ThrowCannotUseBothAutocompleteProviderAndItemsSourceException()
        {
            throw new Exception("You cannot use both AutocompleteProvider and ItemsSource");
        }
    }
}