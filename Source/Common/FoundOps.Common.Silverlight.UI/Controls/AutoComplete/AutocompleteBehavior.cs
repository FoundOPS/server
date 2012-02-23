using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public class AutocompleteBehavior
	{
		public static readonly DependencyProperty GetItemsDelayProperty =
			DependencyProperty.RegisterAttached("GetItemsDelay", typeof(TimeSpan), typeof(AutocompleteBehavior), new PropertyMetadata(OnGetItemsDelayChanged));

		public static readonly DependencyProperty MinimumTextLengthProperty =
			DependencyProperty.RegisterAttached("MinimumTextLength", typeof(int), typeof(AutocompleteBehavior), new PropertyMetadata(OnMinimumTextLengthChanged));

		public static readonly DependencyProperty AutocompleteProviderProperty =
			DependencyProperty.RegisterAttached("AutocompleteProvider", typeof(IAutocompleteProvider), typeof(AutocompleteBehavior), new PropertyMetadata(OnAutocompleteProviderChanged));

		public static readonly DependencyProperty ItemsSourceProperty =
			DependencyProperty.RegisterAttached("ItemsSource", typeof(IEnumerable), typeof(AutocompleteBehavior), new PropertyMetadata(OnItemsSourceChanged));

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
				var collectionViewSource = new CollectionViewSource();
				collectionViewSource.Source = e.NewValue;
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

				// In Silverlight you cannot bind a Dependency property to Attached property in code-behind! So we are doing the oposite.
				model.TextPath = TextSearch.GetTextPath(comboBox);
				BindingOperations.SetBinding(comboBox, TextSearch.TextPathProperty, new Binding() { Path = new PropertyPath("TextPath"), Source = model, Mode = BindingMode.TwoWay });
			}
			return model;
		}

		private static void ThrowCannotUseBothAutocompleteProviderAndItemsSourceException()
		{
			throw new Exception("You cannot use both AutocompleteProvider and ItemsSource");
		}
	}
}