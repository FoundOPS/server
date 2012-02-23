using System;
using System.Collections;

namespace FoundOps.Common.Silverlight.UI.Controls.AutoComplete
{
	public class ItemsReceivedEventArgs : EventArgs
	{
		public ItemsReceivedEventArgs(IEnumerable items)
		{
			this.Items = items;
		}

		public IEnumerable Items { get; private set; }
	}
}