using FoundOps.Common.Composite.Entities;
using System;

//Partial class must be part of same namespace
// ReSharper disable CheckNamespace
namespace FoundOps.Core.Models.CoreEntities
// ReSharper restore CheckNamespace
{
    public partial class TaskHolder : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public TaskHolder()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        public void OnCreate()
        {
            // ReSharper disable CSharpWarnings::CS0612
            Id = Guid.NewGuid();
            // ReSharper restore CSharpWarnings::CS0612

            OnCreation();
        }

        partial void OnCreation(); //For Extensions on Silverlight Side

        #endregion

    }
}
