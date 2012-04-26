using System.Linq;
using System.Windows;
using System.ComponentModel;
using System.Windows.Browser;
using FoundOps.SLClient.Data.Services;
using Telerik.Windows.Controls;
using FoundOps.Core.Models.CoreEntities;
using FoundOps.SLClient.Navigator.ViewModels;
using System;

namespace FoundOps.SLClient.Navigator.Controls
{
    /// <summary>
    /// The UI for navigating different blocks in the application.
    /// </summary>
    public partial class NavigationSideBar : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationSideBar"/> class.
        /// </summary>
        public NavigationSideBar()
        {
            InitializeComponent();
        }

        public NavigationBarVM NavigationBarVM
        {
            get { return (NavigationBarVM)DataContext; }
        }

        private void RadRibbonButtonLoaded(object sender, RoutedEventArgs e)
        {
            //Sets the style of the button, based on the ResourceDictionary of the Block.Name+"ButtonStyle"
            var radRibbonButton = (RadRibbonButton)sender;
            var styleName = radRibbonButton.CommandParameter + "ButtonStyle";
            if (Application.Current.Resources.Contains(styleName)) //Set style if available
                radRibbonButton.Style = (Style)Application.Current.Resources[styleName];
        }

        private void RadRibbonButtonClick(object sender, RoutedEventArgs e)
        {
            //Find the SelectedRole from the parent ListBoxItem and update the NavigationBarVM.SelectedRole accordingly

            var radButton = (RadButton)sender;
            var selectedBlock = (Block)radButton.DataContext;

            //SelectedRole is the Role of the SelectedOwnerAccountOfRole that contains the clicked on Block
            //Note: A user will never have two roles with overlapping Blocks
            var selectedRole =
                NavigationBarVM.CurrentUserAccount.AccessibleRoles.FirstOrDefault(
                    r =>
                    r.OwnerParty == NavigationBarVM.SelectedOwnerAccountOfRole &&
                    r.Blocks.Any(b => b == selectedBlock));

            NavigationBarVM.SelectedRole = selectedRole;
            NavigationBarVM.SetSelectedBlock(selectedBlock);
            
            //Open the uservoice popup
            if (selectedBlock.Id == BlockConstants.FeedbackSupportBlockId)
            {
                HtmlPage.Window.Invoke("openUserVoice");
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
