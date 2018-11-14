using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using DynamicProxyImplementation;
using Microsoft.Pex.Framework;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TxFlow.CSharpDSL;

namespace TestInterception
{

    public class ActivityToolboxProxy : DynamicProxy
    {
        private ActivityInvocationLog _activityInvocationLog;

        public ActivityToolboxProxy(ActivityInvocationLog activityInvocationLog)
        {
            _activityInvocationLog = activityInvocationLog;            
        }

        public override bool TryInvokeMember(Type interfaceType, string name, object[] args, out object result)
        {
            var interfaceMethod = interfaceType.GetMethod(name);
            var parameters = interfaceMethod.GetParameters();
            int i = 0;
            Dictionary<string, object> parameterValuesByName = new Dictionary<string, object>();
            foreach(var arg in args)
            {
                parameterValuesByName[parameters[i++].Name] = arg;
            }

            _activityInvocationLog.Log(new ActivityInvocation(name, parameterValuesByName, null));

            if(interfaceMethod.ReturnType != typeof(void))
            {
                //result = PexChoose.Value<bool>(name);
                //PexObserve.ValueForViewing(name, result);
                result = null;
                //result = typeof(PexChoose).GetMethod("Value").MakeGenericMethod(interfaceMethod.ReturnType).Invoke(null, new object[] { name });
                return true;
            }else
            {
                result = null;
            }
            return false;
        }
    }

    public class WorkflowTestExecutor<TWorkflow, TActivityToolbox>
    {
        private readonly TWorkflow _workflow;
        private readonly ActivityInvocationLog _activityInvocationLog = null;
        public WorkflowTestExecutor()
        {
            var wfTracer = new WorkflowTestExecutionTracer();
            _activityInvocationLog = wfTracer.ActivityInvocationLog;
            var b = new ContainerBuilder();
            b.Register(i => wfTracer);
            b.RegisterType<TActivityToolbox>().EnableClassInterceptors().InterceptedBy(typeof(WorkflowTestExecutionTracer));
            b.RegisterType<TWorkflow>();
            var container = b.Build();

            _workflow = container.Resolve<TWorkflow>();
            var activityToolbox = container.Resolve<TActivityToolbox>();
            //_workflow.GetType().GetProperty("Activities").SetValue(_workflow, container.Resolve<TActivityToolbox>());
            _workflow.GetType().GetMethod("RegisterActivityToolbox").Invoke(_workflow, new object[] { activityToolbox });
        }

        //public WorkflowTestExecutor()
        //{
        //    var type = DynamicProxyTypeBuilder.CreateType<TActivityToolbox, ActivityToolboxProxy>();
        //    var activityToolbox = (TActivityToolbox)Activator.CreateInstance(type, new ActivityToolboxProxy(_activityInvocationLog));
        //    //var activityToolbox = Activator.CreateInstance<TActivityToolbox>();
        //    _workflow = Activator.CreateInstance<TWorkflow>();
        //    _workflow.GetType().GetMethod("RegisterActivityToolbox").Invoke(_workflow, new object[] { activityToolbox });
        //}


        public TWorkflow Workflow => _workflow;

        public ActivityInvocationLog ActivityInvocationLog => _activityInvocationLog;
    }
}
