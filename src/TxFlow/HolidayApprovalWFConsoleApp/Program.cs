using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using Microsoft.CSharp.Activities;
using System.Activities.XamlIntegration;
using System.Activities.Expressions;
using System.Xaml;
using System.Text;
using System.IO;
using HolidayApproval.Entities;

namespace HolidayApprovalWFConsoleApp
{
  public class Program
  {

    private static string serialize(ActivityBuilder activity)
    {
      // Serialize the workflow to XAML and store it in a string.  
      StringBuilder sb = new StringBuilder();
      StringWriter tw = new StringWriter(sb);
      XamlWriter xw = ActivityXamlServices.CreateBuilderWriter(new XamlXmlWriter(tw, new XamlSchemaContext()));
      XamlServices.Save(xw, activity);
      return sb.ToString();

    }
    static void Main(string[] args)
    {
                //WorkflowApplication workflowApplication = new WorkflowApplication(new Workflow1(), new Dictionary<string, object>() { { "x", 1 }, { "y", 2 } });
                //workflowApplication.Run();

      try
      {
        var activityBuilder = HolidayApprovalWorkflowImperative.Create3();


        string text = serialize(activityBuilder);

        Activity activity = ActivityXamlServices.Load(new StringReader(text)) as Activity;
        activity = CompileExpressions(activity);

        var holidayRequest = new HolidayRequestEntity()
        {
          HolidayFrom = DateTime.Now,
          HolidayTo = DateTime.Now,
          State= EHolidayRequestState.New,
          Originator = new UserEntity()
          {
            EmailAddress = "mariusmoessmer@gmail.com",
            UserName = "ma"
          }
        };
        WorkflowInvoker.Invoke(activity, new Dictionary<string, object>() { { "HolidayRequest", holidayRequest } });

      }catch(Exception ex)
      {

      }
      


      UserEntity user = new UserEntity();

      //Activity workflow1 = new Workflow1();



      //WorkflowInvoker.Invoke(CompileExpressions(workflow), new Dictionary<string, object>() { { "x", 1 }, { "y", 2 } });




    }


    public static Activity CompileExpressions(Activity dynamicActivity)
    {
      // activityName is the Namespace.Type of the activity that contains the  
      // C# expressions. For Dynamic Activities this can be retrieved using the  
      // name property , which must be in the form Namespace.Type.  
      string activityName = "T";

      // Split activityName into Namespace and Type.Append _CompiledExpressionRoot to the type name  
      // to represent the new type that represents the compiled expressions.  
      // Take everything after the last . for the type name.  
      string activityType = activityName.Split('.').Last() + "_CompiledExpressionRoot";
      // Take everything before the last . for the namespace.  
      string activityNamespace = string.Join(".", activityName.Split('.').Reverse().Skip(1).Reverse());

      // Create a TextExpressionCompilerSettings.  
      TextExpressionCompilerSettings settings = new TextExpressionCompilerSettings
      {
        Activity = dynamicActivity,
        Language = "C#",
        ActivityName = activityType,
        ActivityNamespace = "HolidayApprovalWFConsoleApp",
        RootNamespace = null,
        GenerateAsPartialClass = false,
        AlwaysGenerateSource = true,
        ForImplementation = true
      };

      TextExpression.SetReferencesForImplementation(dynamicActivity, new AssemblyReference { Assembly = typeof(HolidayRequestEntity).Assembly });

      // Compile the C# expression.  
      TextExpressionCompilerResults results =
          new TextExpressionCompiler(settings).Compile();

      // Any compilation errors are contained in the CompilerMessages.  
      if (results.HasErrors)
      {
        throw new Exception("Compilation failed.");
      }

      // Create an instance of the new compiled expression type.  
      ICompiledExpressionRoot compiledExpressionRoot =
          Activator.CreateInstance(results.ResultType,
              new object[] { dynamicActivity }) as ICompiledExpressionRoot;

      // Attach it to the activity.  
      CompiledExpressionInvoker.SetCompiledExpressionRootForImplementation(
          dynamicActivity, compiledExpressionRoot);

      return dynamicActivity;
    }
  }
}
