using System;
using System.Xaml;
using System.Threading;
using System.ServiceModel;
using System.Activities.Tracking;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using FoundOps.QuickBooksWF.Tests.Create;
using System.ServiceModel.Activities.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoundOps.QuickBooksWF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        ///   The endpoint address to be used by the test host
        /// </summary>

        [TestMethod]
        public void TestMethod1()
        {
            #region Test

            CallWorkflow("Create");

            #endregion
        }

        private void CallWorkflow(string type)
        {
            const string baseAddress = "http://localhost:37070/";

            var serviceImplementation = XamlServices.Load(@"C:\FoundOps\GitHub\Source\Server\FoundOps.QuickBooksWF\" + type + ".xamlx") as WorkflowService;

            if (serviceImplementation == null)
            {
                throw new NullReferenceException(String.Format("Unable to load service definition"));
            }

            using (var host = new WorkflowServiceHost(serviceImplementation, new Uri(baseAddress)))
            {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });

                host.AddServiceEndpoint(type, new BasicHttpBinding(), baseAddress);

                var fileTrackingProfile = new TrackingProfile();
                fileTrackingProfile.Queries.Add(new WorkflowInstanceQuery
                                                    {
                                                        States = { "*" }
                                                    });
                fileTrackingProfile.Queries.Add(new ActivityStateQuery()
                                                    {
                                                        ActivityName = "*",
                                                        States = { "*" },
                                                        // You can use the following to specify specific stages:
                                                        // States = {
                                                        // ActivityStates.Executing,
                                                        // ActivityStates.Closed
                                                        //},
                                                        Variables =
                                                            {
                                                                {"*"}
                                                            } // or you can enter specific variable names instead of “*”

                                                    });
                fileTrackingProfile.Queries.Add(new CustomTrackingQuery()
                                                    {
                                                        ActivityName = "*",
                                                        Name = "*"
                                                    });


                host.Description.Behaviors.Add(new WorkflowIdleBehavior()
                                                   {
                                                       TimeToPersist = TimeSpan.FromSeconds(30),
                                                       TimeToUnload = TimeSpan.FromSeconds(20)
                                                   });


                host.Open();

                var proxy = new CreateClient(new BasicHttpBinding(), new EndpointAddress(baseAddress));

                try
                {
                    proxy.Start();
                }
                catch (Exception e)
                {
                }

                //var syncEvent = new AutoResetEvent(false);


                //var state = proxy.State;

                //while (state != CommunicationState.Closed)
                //    state = proxy.State;

                //Need to set the syncEvent value after WF is complete
                //syncEvent.WaitOne();

                host.Close();
            }
        }
    }
}

