﻿#pragma checksum "C:\FoundOps\Agile5\Source-DEV\Client\FoundOps.SLClient.UI\Controls\Dispatcher\RouteManifestViewer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "D07CB540FA0B27FF99895F884EB3178D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FoundOps.SLClient.UI.Controls.Dispatcher;
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


namespace FoundOps.SLClient.UI.Controls.Dispatcher {
    
    
    public partial class RouteManifestViewer : System.Windows.Controls.ChildWindow {
        
        internal System.Windows.Controls.ChildWindow myRouteManifestViewer;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal Telerik.Windows.Controls.RadNumericUpDown PageUpDown;
        
        internal System.Windows.Controls.TextBlock PageCountTextBox;
        
        internal FoundOps.SLClient.UI.Controls.Dispatcher.RouteManifest RouteManifest;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Dispatcher/RouteManifestViewer.xaml", System.UriKind.Relative));
            this.myRouteManifestViewer = ((System.Windows.Controls.ChildWindow)(this.FindName("myRouteManifestViewer")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.PageUpDown = ((Telerik.Windows.Controls.RadNumericUpDown)(this.FindName("PageUpDown")));
            this.PageCountTextBox = ((System.Windows.Controls.TextBlock)(this.FindName("PageCountTextBox")));
            this.RouteManifest = ((FoundOps.SLClient.UI.Controls.Dispatcher.RouteManifest)(this.FindName("RouteManifest")));
        }
    }
}

