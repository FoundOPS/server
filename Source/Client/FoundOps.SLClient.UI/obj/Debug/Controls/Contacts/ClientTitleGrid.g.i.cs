﻿#pragma checksum "C:\FoundOps\GitHub\Source\Client\FoundOps.SLClient.UI\Controls\Contacts\ClientTitleGrid.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "88C49F16D6FFC0A52B401D9150A15C3D"
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


namespace FoundOps.SLClient.UI.Controls.Contacts {
    
    
    public partial class ClientTitleGrid : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myClientTitleGrid;
        
        internal FoundOps.Common.Silverlight.UI.Controls.AddEditDelete.AddDelete AddDeleteClientTitle;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.Controls.StackPanel ClientsVMHolder;
        
        internal Telerik.Windows.Controls.RadGridView ClientTitleRadGridView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Contacts/ClientTitleGrid.xaml", System.UriKind.Relative));
            this.myClientTitleGrid = ((System.Windows.Controls.UserControl)(this.FindName("myClientTitleGrid")));
            this.AddDeleteClientTitle = ((FoundOps.Common.Silverlight.UI.Controls.AddEditDelete.AddDelete)(this.FindName("AddDeleteClientTitle")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ClientsVMHolder = ((System.Windows.Controls.StackPanel)(this.FindName("ClientsVMHolder")));
            this.ClientTitleRadGridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("ClientTitleRadGridView")));
        }
    }
}

