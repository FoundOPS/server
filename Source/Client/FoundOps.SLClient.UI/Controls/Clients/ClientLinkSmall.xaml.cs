using System.Windows;
using FoundOps.SLClient.UI.ViewModels;
using FoundOps.Core.Models.CoreEntities;

namespace FoundOps.SLClient.UI.Controls.Clients
{
    public partial class ClientLinkSmall
    {
        public ClientLinkSmall()
        {
            InitializeComponent();
         }

        public ClientsVM ClientsVM
        {
            get { return (ClientsVM)this.DataContext; }
        }

        #region SelectedClient Dependency Property

        /// <summary>
        /// SelectedClient
        /// </summary>
        public Client SelectedClient
        {
            get { return (Client) GetValue(SelectedClientProperty); }
            set { SetValue(SelectedClientProperty, value); }
        }

        /// <summary>
        /// SelectedClient Dependency Property.
        /// </summary>
        public static readonly DependencyProperty SelectedClientProperty =
            DependencyProperty.Register(
                "SelectedClient",
                typeof (Client),
                typeof (ClientLinkSmall),
                new PropertyMetadata(null));

        #endregion


        #region IsReadOnly Dependency Property

        /// <summary>
        /// IsReadOnly
        /// </summary>
        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        /// <summary>
        /// IsReadOnly Dependency Property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(ClientLinkSmall),
                new PropertyMetadata(null));

        #endregion

    }
}
