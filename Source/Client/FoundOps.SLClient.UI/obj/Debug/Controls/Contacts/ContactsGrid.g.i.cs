﻿#pragma checksum "C:\FoundOps\GitHub\Source\Client\FoundOps.SLClient.UI\Controls\Contacts\ContactsGrid.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2D3485C680294F9588AA9150DFB1988D"
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


namespace FoundOps.SLClient.UI.Controls.Contacts {
    
    
    public partial class ContactsGrid : System.Windows.Controls.UserControl {
        
        internal System.Windows.Controls.UserControl myContactsGrid;
        
        internal Telerik.Windows.Controls.RadGridView ContactsGridView;
        
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
            System.Windows.Application.LoadComponent(this, new System.Uri("/FoundOps.SLClient.UI;component/Controls/Contacts/ContactsGrid.xaml", System.UriKind.Relative));
            this.myContactsGrid = ((System.Windows.Controls.UserControl)(this.FindName("myContactsGrid")));
            this.ContactsGridView = ((Telerik.Windows.Controls.RadGridView)(this.FindName("ContactsGridView")));
        }
    }
}

