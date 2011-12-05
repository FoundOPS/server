using System.ComponentModel;
using Microsoft.Windows.Data.DomainServices;

namespace FoundOps.Common.Silverlight.MVVM.Services
{
    public class DesignLoader : CollectionViewLoader
    {
        public override bool CanLoad { get { return true; } }
        public override void Load(object userState)
        {
            this.OnLoadCompleted(new AsyncCompletedEventArgs(null, false, userState));
        }
    }
}
