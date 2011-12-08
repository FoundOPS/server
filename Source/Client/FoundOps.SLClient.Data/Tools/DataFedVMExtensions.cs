using System.Windows;
using FoundOps.SLClient.Data.ViewModels;
using MEFedMVVM.Common;

namespace FoundOps.SLClient.Data.Tools
{
    ///<summary>
    /// Extensions methods for DataFedVM
    ///</summary>
    public static class DataFedVMExtensions
    {
        /// <summary>
        /// Whenever the dependentFrameworkElement is Loaded it will require the DataFedVM
        /// </summary>
        /// <param name="dependentFrameworkElement">The dependent FrameworkElement</param>
        ///<param name="requiredVM">The required VM</param>
        public static void DependentWhenVisible(this FrameworkElement dependentFrameworkElement, DataFedVM requiredVM)
        {
            //TODO: Remove. Must be here because the viewmodels are not currently being imported by MEFedMVVM in DesignMode.
            if (Designer.IsInDesignMode)
                return;

            requiredVM.DependentWhenVisible(dependentFrameworkElement);
        }
    }
}
