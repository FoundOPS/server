using System;

namespace FoundOps.Core.Tools
{
    public static class CommonExceptions
    {
        /// <summary>
        /// Will throw an exception if not authorized to access a business account for a role id.
        /// </summary>
        public static Exception NotAuthorizedBusinessAccount
        {
            get { return new Exception("Not authorized for this Business Account. Invalid access attempt logged."); }
        }
    }
}
