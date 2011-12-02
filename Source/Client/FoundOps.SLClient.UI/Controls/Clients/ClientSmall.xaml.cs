using System.Windows;
using FoundOps.SLClient.UI.ViewModels;

namespace FoundOps.SLClient.UI.Controls.Clients
{
	public partial class ClientSmall
	{
		public ClientSmall()
		{
			// Required to initialize variables
			InitializeComponent();
		}

	    #region ClientsListVM Dependency Property

	    /// <summary>
	    /// ClientsListVM
	    /// </summary>
	    public ClientsVM ClientsListVM
	    {
	        get { return (ClientsVM) GetValue(ClientsListVMProperty); }
	        set { SetValue(ClientsListVMProperty, value); }
	    }

	    /// <summary>
	    /// ClientsListVM Dependency Property.
	    /// </summary>
	    public static readonly DependencyProperty ClientsListVMProperty =
	        DependencyProperty.Register(
	            "ClientsListVM",
	            typeof (ClientsVM),
	            typeof (ClientSmall),
	            new PropertyMetadata(null));

	    #endregion
	}
}