using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public class AutocompleteViewModel : DependencyObject
	{
		private readonly DispatcherTimer _timer = new DispatcherTimer();

		public static readonly DependencyProperty TextSearchModeProperty =
			DependencyProperty.Register("TextSearchMode", typeof(TextSearchMode), typeof(AutocompleteViewModel), new PropertyMetadata(TextSearchMode.StartsWith, OnTextSearchModeChanged));

		public static readonly DependencyProperty TextPathProperty =
			DependencyProperty.Register("TextPath", typeof(string), typeof(AutocompleteViewModel), new PropertyMetadata(OnTextPathChanged));

		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(object), typeof(AutocompleteViewModel), null);

		public static readonly DependencyProperty IsDropDownOpenProperty =
			DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(AutocompleteViewModel), null);

		public static readonly DependencyProperty AutocompleteProviderProperty =
			DependencyProperty.Register("AutocompleteProvider", typeof(IAutocompleteProvider), typeof(AutocompleteViewModel), new PropertyMetadata(OnAutocompleteProviderChanged));

		public static readonly DependencyProperty TextProperty =
			DependencyProperty.Register("Text", typeof(string), typeof(AutocompleteViewModel), new PropertyMetadata(OnTextChanged));

		public AutocompleteViewModel()
		{
			this.Suggestions = new ObservableCollection<object>();
		}

		public string TextPath
		{
			get { return (string)GetValue(TextPathProperty); }
			set { SetValue(TextPathProperty, value); }
		}

		public TextSearchMode TextSearchMode
		{
			get { return (TextSearchMode)GetValue(TextSearchModeProperty); }
			set { SetValue(TextSearchModeProperty, value); }
		}

		public ObservableCollection<object> Suggestions
		{
			get;
			private set;
		}

		public int MinimumTextLength 
		{ 
			get; 
			set; 
		}

		public TimeSpan GetItemsDelay 
		{ 
			get; 
			set; 
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public bool IsDropDownOpen
		{
			get { return (bool)GetValue(IsDropDownOpenProperty); }
			set { SetValue(IsDropDownOpenProperty, value); }
		}

		public object SelectedItem
		{
			get { return (object)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		public IAutocompleteProvider AutocompleteProvider
		{
			get { return (IAutocompleteProvider)GetValue(AutocompleteProviderProperty); }
			set { SetValue(AutocompleteProviderProperty, value); }
		}

		private static void OnTextSearchModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var autocompleteProvider = (d as AutocompleteViewModel).AutocompleteProvider;
			if (autocompleteProvider != null)
			{
				autocompleteProvider.TextSearchMode = (TextSearchMode)e.NewValue;
			}
		}

		private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			(d as AutocompleteViewModel).OnTextChanged((string)e.NewValue, (string)e.OldValue);
		}

		private static void OnTextPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var autocompleteProvider = (d as AutocompleteViewModel).AutocompleteProvider;
			if (autocompleteProvider != null)
			{
				autocompleteProvider.TextPath = (string)e.NewValue;
			}
		}

		private static void OnAutocompleteProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var model = d as AutocompleteViewModel;

			var provider = (IAutocompleteProvider)e.OldValue;
			if (provider != null)
			{
				provider.ItemsReceived += model.OnItemsReceived;
			}

			provider = (IAutocompleteProvider)e.NewValue;
			if (provider != null)
			{
				provider.TextPath = model.TextPath;
				provider.TextSearchMode = model.TextSearchMode;
				provider.ItemsReceived += model.OnItemsReceived;
			}
		}

		protected virtual void OnTextChanged(string newText, string oldText)
		{
			var selectedItemText = BindingExpressionHelper.GetValue(this.SelectedItem, this.TextPath) as string;
			if (this.Text != selectedItemText)
			{
				this.IsDropDownOpen = true;
			}

			if (newText != null && (newText.Length >= this.MinimumTextLength || newText.Length == 0))
			{
				if (this.GetItemsDelay != TimeSpan.Zero)
				{
					if (this._timer.IsEnabled)
					{
						this._timer.Stop();
					}
					else
					{
						EventHandler tickHandler = null;
						tickHandler = (sender, e) =>
							{
								this._timer.Tick -= tickHandler;
								this._timer.Stop();

								this.AutocompleteProvider.GetItems(newText);
							};
						this._timer.Tick += tickHandler;
					}
					this._timer.Interval = this.GetItemsDelay;
					this._timer.Start();
				}
				else
				{
					this.AutocompleteProvider.GetItems(newText);
				}
			}
		}

		private void OnItemsReceived(object sender, ItemsReceivedEventArgs e)
		{
			bool emptySelection = string.IsNullOrEmpty(this.Text);

			for (int i = this.Suggestions.Count - 1; i >= 0; i--)
			{
				if (emptySelection || this.Suggestions[i] != this.SelectedItem)
				{
					this.Suggestions.RemoveAt(i);
				}
			}

			foreach (object item in e.Items)
			{
				if (!this.Suggestions.Contains(item))
				{
					this.Suggestions.Insert(0, item);
				}
			}
		}
	}
}