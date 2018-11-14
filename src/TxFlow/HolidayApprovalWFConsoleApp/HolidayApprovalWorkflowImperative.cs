using HolidayApproval.Activities;
using HolidayApproval.Entities;
using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApprovalWFConsoleApp
{
  public static class HolidayApprovalWorkflowImperative
  {
    public static Activity Create()
    {
var staffManagerDecisionDecisionVariable = new Variable<int>() { Name = "staffManagerDecision" };
var workflow = new DynamicActivity
{
  Properties = { // declare In-argument HolidayRequest
  new DynamicActivityProperty {
      Name = "HolidayRequest", Type = typeof(InArgument<HolidayRequestEntity>)},
  },
  Implementation = () => new Flowchart
  { // define flowchart
    Variables = { staffManagerDecisionDecisionVariable }, // declare decision-variable which holds decision of staff manager
    StartNode = new FlowStep
    {
      Action = new CreateHolidayApprovalTaskActivity
      {
        HolidayRequest = new InArgument<HolidayRequestEntity>(
          new CSharpValue<HolidayRequestEntity>("HolidayRequest")
        ),
        ResponsibleUser = "StaffManager",
        Result = new OutArgument<bool>(staffManagerDecisionDecisionVariable)
      },
      Next = new FlowDecision
      {
        Condition = staffManagerDecisionDecisionVariable,
        False = new FlowStep
        {
          Action = new UpdateHolidayRequestStateActivity
          {
            HolidayRequest = new InArgument<HolidayRequestEntity>(
              new CSharpValue<HolidayRequestEntity>("HolidayRequest")
            ),
            NewState = EHolidayRequestState.Refused
          },
          Next = new FlowStep
          {
            Action = new SendEmailActivity
            {
              Subject = "Holiday request",
              Body = "Your holiday request has been refused!",
              ToMailAddress = new InArgument<string>(
                new CSharpValue<string>("HolidayRequest.Originator.EmailAddress")
              )
            }
          }
        },
        True = new FlowStep
        {
          // omitted
        }
      }
    }
  }
};

      return workflow;
    }

    public static DynamicActivity Create2()
    {
var workflow = new DynamicActivity
{
  Properties = { // declare In-argument HolidayRequest
    new DynamicActivityProperty {
      Name = "HolidayRequest", Type = typeof(InArgument<HolidayRequestEntity>)},
  },
  
};

workflow.Implementation = () =>
{
  Flowchart flowchart = new Flowchart();
  // declare variable staffManagerDecision which will hold decision of staffManager
  var staffManagerDecisionVariable = new Variable<bool>("staffManagerDecision");
  flowchart.Variables.Add(staffManagerDecisionVariable);
  // describe sequence of activities, beginning at startnode
  flowchart.StartNode = new FlowStep
  {
    Action = new CreateHolidayApprovalTaskActivity {
      HolidayRequest = new InArgument<HolidayRequestEntity>(
        new CSharpValue<HolidayRequestEntity>("HolidayRequest")
      ),
      ResponsibleUser = "StaffManager",
      Result = new OutArgument<bool>(staffManagerDecisionVariable)
    },
    Next = new FlowDecision
    {
      Condition = staffManagerDecisionVariable,
      False = new FlowStep {
        Action = new UpdateHolidayRequestStateActivity {
          HolidayRequest = new InArgument<HolidayRequestEntity>(
            new CSharpValue<HolidayRequestEntity>("HolidayRequest")
          ),
          NewState = EHolidayRequestState.Refused
        },
        Next = new FlowStep {
          Action = new SendEmailActivity {
            Subject = "Holiday request",
            Body = "Your holiday request has been refused!",
            ToMailAddress = new InArgument<string>(
              new CSharpValue<string>("HolidayRequest.Originator.EmailAddress")
            )
          }
        }
      },
      True = new FlowStep{
        // omitted
      }
    }
  };
  return flowchart;
};

      return workflow;
    }


    public static ActivityBuilder Create3()
    {
var workflow = new ActivityBuilder
{
  Properties = { // declare In-argument HolidayRequest
    new DynamicActivityProperty {
      Name = "HolidayRequest", Type = typeof(InArgument<HolidayRequestEntity>)},
    },
};

// declare variable staffManagerDecision which will hold decision of staffManager
var staffManagerDecisionVariable = new Variable<bool>("staffManagerDecision");
workflow.Implementation = new Flowchart()
{
  Variables = { staffManagerDecisionVariable },
  StartNode = new FlowStep
  {
    Action = new CreateHolidayApprovalTaskActivity
    {
      HolidayRequest = new InArgument<HolidayRequestEntity>(
        new CSharpValue<HolidayRequestEntity>("HolidayRequest")
      ),
      ResponsibleUser = "StaffManager",
      Result = new OutArgument<bool>(staffManagerDecisionVariable)
    },
    Next = new FlowDecision
    {
      Condition = staffManagerDecisionVariable,
      False = new FlowStep
      {
        // ommited
      },
      True = new FlowStep
      {
        // omitted
      }
    }
  }
};

return workflow;
    }

  }
}
