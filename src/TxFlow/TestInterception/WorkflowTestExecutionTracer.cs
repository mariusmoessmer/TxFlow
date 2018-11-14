using Autofac;
using Autofac.Extras.DynamicProxy;
using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TxFlow.CSharpDSL;

namespace TestInterception
{
    public class WorkflowTestExecutionTracer : IInterceptor
    {
        private readonly ActivityInvocationLog _activityInvocationLog = new ActivityInvocationLog();

        public ActivityInvocationLog ActivityInvocationLog => _activityInvocationLog;

        public void Intercept(IInvocation invocation)
        {
            throw new Exception("following line is not supported anymore!");
            //this.ActivityInvocationLog.Log(new ActivityInvocation(invocation));
            try
            {
                invocation.Proceed();
            }catch(Exception ex)
            {
                if(invocation.Method.IsAbstract)
                {
                    if(invocation.Method.ReturnType != typeof(void) || invocation.Method.GetParameters().Any(p => p.IsOut || p.IsRetval))
                    {
                        throw new Exception($"Activity {invocation.Method.Name } has no implemenation in used ActivityToolbox", ex);
                    }else
                    {
                        // ignore this unimplemented activitiy because it does not have side-effects (return is void and params are neither out nor ref)
                    }
                }else
                {
                    throw;
                }
            }
        }
    }
}
