using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods
{
    public static class VisualTreeExtensions
    {
        //Example usage ItemsControl control = ((DependencyObject)sender).Ancestors().TypeOf<ItemsControl>().FirstOrDefault();

        public static IEnumerable<DependencyObject> Ancestors(this DependencyObject root)
        {
            DependencyObject current = VisualTreeHelper.GetParent(root);
            while (current != null)
            {
                yield return current;
                current = VisualTreeHelper.GetParent(current);
            }
        }

        public static IEnumerable<DependencyObject> Children(this DependencyObject root)
        {
            var children = new List<DependencyObject>();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, 0);
                children.Add(child);
                
                if (child != null)
                {
                    children.AddRange(child.Children());
                }
            }

            return children;
        }
    }
}
