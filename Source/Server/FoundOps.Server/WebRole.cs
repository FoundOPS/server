using Microsoft.Web.Administration;
using Microsoft.WindowsAzure.ServiceRuntime;
using System.Linq;

namespace FoundOps.Server
{
    /// <summary>
    /// Used to force the project into Classic mode to prevent crashing issues due to parallel domain services.
    /// http://stackoverflow.com/questions/1495144/net-2-0-help-troubleshooting-a-system-accessviolationexception-100-managed-c
    /// </summary>
    public class WebRole : RoleEntryPoint
    {
        public override bool OnStart()
        {
            using (var serverManager = new ServerManager())
            {
                var sitesForInstance = serverManager.Sites.Where(s => s.Name.Contains(RoleEnvironment.CurrentRoleInstance.Role.Name));
                var poolsForInstance = serverManager.ApplicationPools.Where(pool => sitesForInstance.Any(s => s.Applications.Any(a => a.ApplicationPoolName == pool.Name)));

                foreach(var pool in poolsForInstance)
                    pool.ManagedPipelineMode = ManagedPipelineMode.Classic;
                
                serverManager.CommitChanges();
            }

            return base.OnStart();
        }
    }
}