using System;
using System.Xaml;
using System.ServiceModel;
using System.Activities.Tracking;
using System.ServiceModel.Activities;
using System.ServiceModel.Description;
using System.ServiceModel.Activities.Description;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FoundOps.QuickBooksWF.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //CallWorkflow();

            //var workflowRuntime = new System.Workflow.Runtime.WorkflowRuntime();

            //create the workflow instance and start it
            //WorkflowInstance instance = workflowRuntime.CreateWorkflow(typeof (FoundOps.QuickBooksWF.GetData));

            var proxy = new ServiceReference1.GetHereClient();
            proxy.Open();
            proxy.GetHere();

            proxy.Close();

            //Works, but only to call individual CodeActivities
            //var objects = WorkflowInvoker.Invoke(new CreateInvoice());

            //serviceClient.Open();

            //var gdr= new GetDataRequest();

            //proxy.Close();
        }

        private void CallWorkflow()
        {
            string baseAddress = "http://localhost:37069/QuickbooksWF";
            using (var host = new WorkflowServiceHost(XamlServices.Load(@"C:\FoundOps\Agile5\Source-DEV\Server\FoundOps.QuickBooksWF\Service1.xamlx"), new Uri(baseAddress)))
            {
                host.Description.Behaviors.Add(new ServiceMetadataBehavior() { HttpGetEnabled = true });

                host.AddServiceEndpoint("ServiceReference", new BasicHttpBinding(), baseAddress);

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
            }

        }
    }
}

