﻿#pragma checksum "C:\FoundOps\Agile5\Source-DEV\Client\FoundOps.SLClient.UI\Controls\Vehicles\Maintenance\MaintenanceGrid.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "649FE8EBE64E26A4204431A362265213"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using Telerik.Windows.Controls;


namespace FoundOps.SLClient.UI.Controls.Vehicles.Maintenance {
    
    
    public partial class MaintenanceGrid : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myMaintenanceGrid;
        
        internal Telerik.Windows.Controls.RadGridView MaintenanceRadGridView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Vehicles/Maintenance/MaintenanceGrid.xam" +
                        "l", System.UriKind.Relative));
            this.myMaintenanceGrid = ((System.Windows.Controls.UserControl)(this.FindName("myMaintenanceGrid")));
            this.MaintenanceRadGridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("MaintenanceRadGridView")));
        }
    }
}

