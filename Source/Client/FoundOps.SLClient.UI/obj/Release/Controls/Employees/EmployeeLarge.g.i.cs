﻿#pragma checksum "C:\FoundOps\Agile5\Source-DEV\Client\FoundOps.SLClient.UI\Controls\Employees\EmployeeLarge.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2D0FDCA952EC36BB4ECF2BD444DA62BB"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using FoundOps.Common.Silverlight.UI.Controls;
using FoundOps.SLClient.UI.Controls.ContactInfo;
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


namespace FoundOps.Framework.Views.Controls.Employees {
    
    
    public partial class EmployeeLarge : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myEmployeeLarge;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal FoundOps.Common.Silverlight.UI.Controls.ImageUpload accountImage1;
        
        internal FoundOps.SLClient.UI.Controls.ContactInfo.ContactInfoEdit contactInfoEdit1;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Employees/EmployeeLarge.xaml", System.UriKind.Relative));
            this.myEmployeeLarge = ((System.Windows.Controls.UserControl)(this.FindName("myEmployeeLarge")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.accountImage1 = ((FoundOps.Common.Silverlight.UI.Controls.ImageUpload)(this.FindName("accountImage1")));
            this.contactInfoEdit1 = ((FoundOps.SLClient.UI.Controls.ContactInfo.ContactInfoEdit)(this.FindName("contactInfoEdit1")));
        }
    }
}

