using Microsoft.VisualBasic.Activities;
using System.Activities;
using System.Activities.Presentation.ViewState;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xaml;

namespace TxFlow.WFBuilder
{
    public static class WorkflowSerializationHelper
    {

        public static string ToXaml(Activity activity)
        {
            return ToXaml(new ActivityBuilder() { Implementation = activity });
        }

        public static string ToXaml(ActivityBuilder ab)
        {
            // Make sure the activity builder has the right information about the c# expressions (even though its visual basic)
            VisualBasic.SetSettings(ab, VisualBasic.GetSettings(ab));

            // Set c# as the language
            System.Activities.Presentation.Expressions.ExpressionActivityEditor.SetExpressionActivityEditor(ab, "C#");

            //// This is what I was hoping would correctly set the Viewstate
            WorkflowViewState.SetViewStateManager(
                ab.Implementation, WorkflowViewState.GetViewStateManager(ab.Implementation));


            StringBuilder sb = new StringBuilder();
            using (StringWriter tw = new StringWriter(sb))
            {
                XamlWriter xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, new XamlSchemaContext()));
                XamlServices.Save(xw, ab);

                // quick hack: remove "<?xml version="1.0" encoding="utf-16"?>" from result because it is not part of xaml
                string result = sb.ToString();
                if(result.StartsWith("<?xml"))
                {
                    result = result.Substring(result.IndexOf("?>") + 2);
                }

                return result;
            }
        }

        public static Activity CreateActivity(string xaml)
        {
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = false,
            };

            List<Assembly> assemblies = new List<Assembly>();
            assemblies.AddRange(System.AppDomain.CurrentDomain.GetAssemblies());

            //return ActivityXamlServices.Load(new StringReader(xaml), settings);


            return ActivityXamlServices.Load(new XamlXmlReader(new StringReader(xaml), new XamlSchemaContext(assemblies, new XamlSchemaContextSettings()
            {
                FullyQualifyAssemblyNamesInClrNamespaces = false
            })
                
                ), settings);
        }

        public static ActivityBuilder CreateActivityBuilder(string xaml)
        {
            return XamlServices.Load(
                ActivityXamlServices.CreateBuilderReader(
                    new XamlXmlReader(new StringReader(xaml)))) as ActivityBuilder;
        }

    }
}
