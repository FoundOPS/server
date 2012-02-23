using System;
using System.ComponentModel;
using System.Linq;
using Telerik.Windows.Controls;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public class CollectionViewProvider : IAutocompleteProvider
	{
		private readonly ICollectionView _collectionView;

		public CollectionViewProvider(ICollectionView collectionView)
		{
			this._collectionView = collectionView;
		}

		public event ItemsReceivedEventHandler ItemsReceived;

		public TextSearchMode TextSearchMode { get; set; }

		public string TextPath { get; set; }

		Func<object, object> _getText;

		public void GetItems(string text)
		{
			var match = TextSearch.CreatePartialMatchFunc(text, this.TextSearchMode);
			if (this._collectionView.CanFilter)
			{
				this._collectionView.Filter = (o) =>
					{
						return match(o as string);
					};
				this.ItemsReceived(this, new ItemsReceivedEventArgs(this._collectionView.OfType<string>().Take(20)));
			}
			else
			{
				if (_getText == null)
				{
					var originalCollection = this._collectionView.SourceCollection.OfType<object>();
					var firstItem = originalCollection.FirstOrDefault();
					if (firstItem != null)
					{
						_getText = BindingExpressionHelper.CreateGetValueFunc(firstItem.GetType(), this.TextPath);
					}
				}

				this.ItemsReceived(this, new ItemsReceivedEventArgs(this._collectionView.OfType<object>().Where((o) =>
					{
						var t = _getText(o);
						return match(t as string);
					}).Take(20)));
			}
		}
	}
}