using System;

namespace FoundOps.Core.Tools
{
   public static  class ExceptionHelper
    {
       /// <summary>
       /// Will throw an exception if not authorized to access a business account for a role id.
       /// </summary>
       public static void ThrowNotAuthorizedBusinessAccount()
       {
           throw new Exception("Not authorized for this Business Account. Invalid access attempt logged.");
       }
    }
}
