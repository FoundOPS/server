using System;
using System.Linq;
using System.Collections.Generic;
using FoundOps.Common.Composite.Entities;

namespace FoundOps.Core.Models.CoreEntities
{
    public enum RoleType
    {
        Administrator = 0,
        Custom = 1,
        Mobile = 2
    }

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
                var orderedBlocks = Blocks.Where(b => b.Name != "Logout" && b.Name != "Feedback and Support").OrderBy(b => b.Name).ToList();
                var logoutBlock = Blocks.FirstOrDefault(b => b.Name == "Logout");
                var feedbackBlock = Blocks.FirstOrDefault(b => b.Name == "Feedback and Support");
                if (feedbackBlock != null)
                    orderedBlocks.Add(feedbackBlock);
                if (logoutBlock != null)
                    orderedBlocks.Add(logoutBlock);
                return orderedBlocks;
            }
        }

        public RoleType RoleType
        {
            get
            {
                return (RoleType)Convert.ToInt32(this.RoleTypeInt);
            }
            set
            {
                this.RoleTypeInt = Convert.ToInt16(value);
            }
        }
    }
}