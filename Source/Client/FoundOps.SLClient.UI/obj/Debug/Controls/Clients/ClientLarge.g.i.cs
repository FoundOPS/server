﻿#pragma checksum "C:\FoundOps\GitHub\Source\Client\FoundOps.SLClient.UI\Controls\Clients\ClientLarge.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "50F09332B65736064DFE686891C9369E"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FoundOps.Common.Silverlight.UI.Controls.AddEditDelete;
using FoundOps.SLClient.UI.Controls.Contacts;
using FoundOps.SLClient.UI.Controls.Files;
using FoundOps.SLClient.UI.Controls.Locations;
using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace FoundOps.SLClient.UI.Controls.Clients {
    
    
    public partial class ClientLarge : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myClientDetailsLarge;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal FoundOps.SLClient.UI.Controls.Files.ImageFileUpload accountImage1;
        
        internal FoundOps.SLClient.UI.Controls.Contacts.ContactsGrid ContactsGrid;
        
        internal FoundOps.Common.Silverlight.UI.Controls.AddEditDelete.AddDelete AddDeleteLocation;
        
        internal FoundOps.SLClient.UI.Controls.Locations.LocationsGrid LocationsGrid;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Clients/ClientLarge.xaml", System.UriKind.Relative));
            this.myClientDetailsLarge = ((System.Windows.Controls.UserControl)(this.FindName("myClientDetailsLarge")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.accountImage1 = ((FoundOps.SLClient.UI.Controls.Files.ImageFileUpload)(this.FindName("accountImage1")));
            this.ContactsGrid = ((FoundOps.SLClient.UI.Controls.Contacts.ContactsGrid)(this.FindName("ContactsGrid")));
            this.AddDeleteLocation = ((FoundOps.Common.Silverlight.UI.Controls.AddEditDelete.AddDelete)(this.FindName("AddDeleteLocation")));
            this.LocationsGrid = ((FoundOps.SLClient.UI.Controls.Locations.LocationsGrid)(this.FindName("LocationsGrid")));
        }
    }
}

