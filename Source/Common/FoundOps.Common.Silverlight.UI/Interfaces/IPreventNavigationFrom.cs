using System;

namespace FoundOps.Common.Silverlight.UI.Interfaces
{
    public interface IPreventNavigationFrom
    {
        bool CanNavigateFrom(Action navigate);
        void OnNavigateFrom();
    }
}
