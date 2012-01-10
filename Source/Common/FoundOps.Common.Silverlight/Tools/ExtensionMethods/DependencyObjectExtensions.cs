using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FoundOps.Common.Silverlight.Tools
{
    public static class DependencyObjectExtensions
    {
        public static List<T> GetChildObjects<T>(this DependencyObject obj, string name) where T : DependencyObject
        {
            var retVal = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object c = VisualTreeHelper.GetChild(obj, i);
                if (c as T != null && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
                {
                    retVal.Add((T)c);
                }
                var gc = ((DependencyObject)c).GetChildObjects<T>(name);
                if (gc != null)
                    retVal.AddRange(gc);
            }

            return retVal;
        }

        public static T GetChildObject<T>(this DependencyObject obj, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                object c = VisualTreeHelper.GetChild(obj, i);
                if (c as T != null && (String.IsNullOrEmpty(name) || ((FrameworkElement)c).Name == name))
                {
                    return (T)c;
                }
                object gc = ((DependencyObject)c).GetChildObject<T>(name);
                if (gc != null)
                    return (T)gc;
            }

            return null;
        }

        public static List<T> AllChildren<T>(this FrameworkElement ele, Func<DependencyObject, bool> whereFunc = null) where T : class
        {
            if (ele == null)
                return null;
            var output = new List<T>();
            var c = VisualTreeHelper.GetChildrenCount(ele);
            for (var i = 0; i < c; i++)
            {
                var ch = VisualTreeHelper.GetChild(ele, i);
                if (whereFunc != null)
                {
                    if (!whereFunc(ch))
                    {
                        continue;
                    }
                }
                if ((ch is T))
                    output.Add(ch as T);
                if (!(ch is FrameworkElement))
                    continue;

                output.AddRange((ch as FrameworkElement).AllChildren<T>(whereFunc));
            }
            return output;
        }
    }
}
