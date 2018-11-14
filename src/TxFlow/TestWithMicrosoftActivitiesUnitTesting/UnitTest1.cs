using System;
using System.Activities;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Text;
using System.Xaml;
using System.Xml;
using HolidayApproval.Activities;
using HolidayApproval.Entities;
using HolidayApprovalWFConsoleApp;
using Microsoft.Activities.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestWithMicrosoftActivitiesUnitTesting
{
    [TestClass]
    public class UnitTest1
    {

        private void test()
        {
var xamlInjector = new XamlInjector("HolidayApprovalWorkflow.xaml");
xamlInjector.ReplaceAll(typeof(CreateHolidayApprovalTaskActivity),
    typeof(MockedRefuseHolidayApprovalTaskActivity));
xamlInjector.ReplaceAt(0, typeof(SendEmailActivity),
    typeof(MockedSendEmailActivity));

var mockedHolidayApprovalWorkflow = xamlInjector.GetActivity();
        }

        [TestMethod]
        public void TestMethod1()
        {
            var xamlInjector = new XamlInjector(@"E:\OneDrive\Dokumente\Masterstudium\Masterarbeit\HolidayApproval\HolidayApprovalWFConsoleApp\HolidayApprovalWFConsoleApp\HolidayApprovalWorkflow.xaml",
                typeof(CreateHolidayApprovalTaskActivity).Assembly);
            xamlInjector.ReplaceAll(typeof(CreateHolidayApprovalTaskActivity), 
                typeof(MockedRefuseHolidayApprovalTaskActivity));
            xamlInjector.ReplaceAt(0, typeof(SendEmailActivity),
                typeof(MockedSendEmailActivity));

            var mockedHolidayApprovalWorkflow = compileToDoRemoveMe(xamlInjector.GetActivity());

            //serialize(mockedHolidayApprovalWorkflow);

// create objects which are used to start workflow
var initialHolidayRequest = new HolidayRequestEntity(
    holidayFrom: new DateTime(2018, 12, 12),
    holidayTo: new DateTime(2018, 12, 23),
    originator: new UserEntity("Marius", "marius@domain.xyz")
);
// create workflowinvoker for testing based on previously created mocked workflow
var host = WorkflowInvokerTest.Create(mockedHolidayApprovalWorkflow);
// test workflow by executing it with holiday-request object
host.TestActivity(new Dictionary<string, object>()
        { { "HolidayRequest", initialHolidayRequest } }
    );
// check assertions
host.Tracking.Assert.Exists(
    typeof(MockedRefuseHolidayApprovalTaskActivity).Name, 
    ActivityInstanceState.Closed);
host.Tracking.Assert.ExistsBefore(
    typeof(MockedRefuseHolidayApprovalTaskActivity).Name, 
    typeof(UpdateHolidayRequestStateActivity).Name, 
    ActivityInstanceState.Closed);
host.Tracking.Assert.ExistsBefore(
    typeof(UpdateHolidayRequestStateActivity).Name, 
    typeof(MockedSendEmailActivity).Name, 
    ActivityInstanceState.Closed);
        }

        private Activity compileToDoRemoveMe(Activity activity)
        {
            DynamicActivity dynamicActivity = (DynamicActivity)activity;
            return Program.CompileExpressions(activity);
        }

        private void serialize(Activity activity)
        {
            StringBuilder xaml = new StringBuilder();

            using (XmlWriter xmlWriter = XmlWriter.Create(
                xaml,
                new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true, }))

            using (XamlWriter xamlWriter = new XamlXmlWriter(
                xmlWriter,
                new XamlSchemaContext()))

            using (XamlWriter xamlServicesWriter =
                ActivityXamlServices.CreateBuilderWriter(xamlWriter))
            {
                ActivityBuilder activityBuilder = new ActivityBuilder
                {
                    Implementation = activity
                };
                XamlServices.Save(xamlServicesWriter, activityBuilder);
            }

            string result = xaml.ToString();
        }
    }
}
