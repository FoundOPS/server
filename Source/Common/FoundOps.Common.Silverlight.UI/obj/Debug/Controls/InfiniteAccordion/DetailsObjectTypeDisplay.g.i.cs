﻿#pragma checksum "C:\FoundOps\Agile5\Source-DEV\Common\FoundOps.Common.Silverlight.UI\Controls\InfiniteAccordion\DetailsObjectTypeDisplay.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DD37AC2C3BBED62BFA8FB6FC0FD8D562"
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


namespace FoundOps.Common.Silverlight.UI.Controls.InfiniteAccordion {
    
    
    public partial class ObjectTypeDisplay : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myDetailsObjectDisplay;
        
        internal System.Windows.Controls.ContentPresenter ContentPresenter;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.Common.Silverlight.UI;component/Controls/InfiniteAccordion/DetailsObjec" +
                        "tTypeDisplay.xaml", System.UriKind.Relative));
            this.myDetailsObjectDisplay = ((System.Windows.Controls.UserControl)(this.FindName("myDetailsObjectDisplay")));
            this.ContentPresenter = ((System.Windows.Controls.ContentPresenter)(this.FindName("ContentPresenter")));
        }
    }
}

