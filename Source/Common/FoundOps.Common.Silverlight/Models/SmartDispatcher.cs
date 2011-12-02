using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace FoundOps.Common.Silverlight.MVVM.Models
{
    public static class SmartDispatcher
    {
        private static readonly bool IsDesigner = DesignerProperties.IsInDesignTool;

        private static Dispatcher Dispatcher 
        {
            get { return Deployment.Current.Dispatcher; }
        }

        public static void BeginInvoke(Action action)
        {
            if (IsDesigner || Dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                Dispatcher.BeginInvoke(action);
            }
        }
    }
}