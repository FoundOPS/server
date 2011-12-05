using System;

namespace FoundOps.Common.Silverlight.MVVM.Interfaces
{
    public interface IPreventNavigationFrom
    {
        bool CanNavigateFrom(Action navigate);
        void OnNavigateFrom();
    }
}
