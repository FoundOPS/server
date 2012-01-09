//using System;
//using System.Linq;
//using System.Reactive.Linq;
//using System.Reflection;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Data;
//using FoundOps.Common.Silverlight.UI.Tools.ExtensionMethods;

//namespace FoundOps.Common.Silverlight.UI.Tools.MarkupExtensions
//{
//    public static class MarkupExtensionHelpers
//    {
//        /// <summary>
//        /// NOTE: NEVER GOT THIS TO WORK.
//        /// Resolves a dependency property binding to a bindingHolder added to a target object.
//        /// This is a workaround for the problem that MarkupExtensions cannot contain bindings.
//        /// </summary>
//        /// <param name="frameworkElement">The framework element whose binding is to be resolved.</param>
//        /// <param name="dependencyPropertyToResolve">The dependency property to resolve.</param>
//        /// <param name="targetObject">The target object to get an ancestor panel from. NOTE: Needs to be something defined in the same control a binding would work.</param>
//        public static void ResolveBinding(this FrameworkElement frameworkElement, DependencyProperty dependencyPropertyToResolve, FrameworkElement targetObject)
//        {
//            if (dependencyPropertyToResolve == null || targetObject == null) return;

//            //Get the current binding expression on the markup extension's dependency property
//            var bindingExpression = frameworkElement.GetBindingExpression(dependencyPropertyToResolve);

//            //Copy and clear the binding
//            if (bindingExpression == null || bindingExpression.ParentBinding == null) return;
//            var binding = bindingExpression.ParentBinding.CopyBinding();
//            frameworkElement.ClearValue(dependencyPropertyToResolve);

//            //NOTE: I believe the issue is here and below
//            //Create a binding holder
//            var bindingHolder = new StackPanel();

//            //Bind that property back to the markupExtensionDependencyProperty
//            frameworkElement.SetBinding(dependencyPropertyToResolve, new Binding("DataContext") { Source = bindingHolder });

//            //Wait until the VisualTree is constructed to add the binding holder
//            //to the first ancestor panel of the targetObject

//            var bindingHolderAdded = false;
//            targetObject.Loaded += (s, e) =>
//            {
//                //Add the binding holder
//                var parentPanel = (Panel)targetObject.Ancestors().FirstOrDefault(a => a as Panel != null);
//                if (parentPanel == null || bindingHolderAdded) return;

//                parentPanel.Children.Add(bindingHolder);
//                //Set the binding on the bindingHolder
//                bindingHolder.SetBinding(FrameworkElement.DataContextProperty, binding);

//                bindingHolderAdded = true;
//            };
//        }
//    }
//}