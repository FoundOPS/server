using System;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Xaml;
using System.Activities.Tracking;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using System.ServiceModel.Activities.Description;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FoundOps.QuickBooksWF.Tests.Create;

namespace FoundOps.QuickBooksWF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        /// <summary>
        ///   The endpoint address to be used by the test host
        /// </summary>
        private readonly EndpointAddress _serviceAddress = new EndpointAddress("net.pipe://localhost/Create.xamlx");

        [TestMethod]
        public void TestMethod1()
        {
            #region Test

            CallWorkflow();

            //var workflowRuntime = new System.Workflow.Runtime.WorkflowRuntime();
            //create the workflow instance and start it

            //WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof (FoundOps.QuickBooksWF.GetData));

            //var proxy = new ServiceReference1.GetHereClient();
            //proxy.Open();
            //proxy.GetHere();

            //proxy.Close();

            //Works, but only to call individual CodeActivities
            //var objects = WorkflowInvoker.Invoke(new CreateInvoice());

            //serviceClient.Open();

            //var gdr= new GetDataRequest();

            //proxy.Close();
            #endregion
        }

        private void CallWorkflow()
        {
            const string baseAddress = "http://localhost:37070/";

            var serviceImplementation = XamlServices.Load(@"C:\FoundOps\GitHub\Source\Server\FoundOps.QuickBooksWF\Create.xamlx") as WorkflowService;

            if (serviceImplementation == null)
            {
                throw new NullReferenceException(String.Format("Unable to load service definition"));
            }

            using (var host = new WorkflowServiceHost(serviceImplementation, new Uri(baseAddress)))
            {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });

                host.AddServiceEndpoint("Create", new BasicHttpBinding(), baseAddress);

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
                                                       TimeToPersist = TimeSpan.FromSeconds(5),
                                                       TimeToUnload = TimeSpan.FromSeconds(20)
                                                   });


                host.Open();

                var proxy = new CreateClient(new BasicHttpBinding(), new EndpointAddress(baseAddress));

                proxy.Start();

                host.Close();
            }
        }
    }
}

