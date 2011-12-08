using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Entities;
using FoundOps.Core.Server.Blocks;

namespace FoundOps.Core.Models.CoreEntities
{
    public partial class Role : IEntityDefaultCreation
    {
        #region Implementation of IEntityDefaultCreation

#if SILVERLIGHT
        partial void OnCreated()
        {
            ((IEntityDefaultCreation) this).OnCreate();
        }
#else
        public Role()
        {
            ((IEntityDefaultCreation)this).OnCreate();
        }
#endif

        partial void OnCreation(); //For Extensions on Silverlight Side

        void IEntityDefaultCreation.OnCreate()
        {
            Id = Guid.NewGuid();
            OnCreation();
        }

        #endregion

        public IEnumerable<Block> OrderedBlocks
        {
            get
            {
                //Put the LogoutBlock at the bottom
                var orderedBlocks = Blocks.Where(b => b.Name != "Logout").OrderBy(b => b.Name).ToList();
                var logoutBlock = Blocks.FirstOrDefault(b => b.Name == "Logout");
                if (logoutBlock != null)
                    orderedBlocks.Add(logoutBlock);
                return orderedBlocks;
            }
        }
    }
}