﻿#pragma checksum "C:\FoundOps\GitHub\Source\Client\FoundOps.SLClient.UI\Controls\Dispatcher\RouteDetails.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DEDC94F19D5B7FDD23AF3B9180F730A8"
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


namespace FoundOps.Framework.Views.Controls.Dispatcher {
    
    
    public partial class RouteDetails : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myRouteDetails;
        
        internal System.Windows.Controls.ScrollViewer LayoutRoot;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Dispatcher/RouteDetails.xaml", System.UriKind.Relative));
            this.myRouteDetails = ((System.Windows.Controls.UserControl)(this.FindName("myRouteDetails")));
            this.LayoutRoot = ((System.Windows.Controls.ScrollViewer)(this.FindName("LayoutRoot")));
        }
    }
}

