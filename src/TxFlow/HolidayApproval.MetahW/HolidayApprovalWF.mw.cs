//
//Auto-generated, DO NOT EDIT
//Visit https://github.com/knat/Metah for more information
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using HolidayApproval.Entities;

namespace HolidayApproval.MetahW
{
    class Program
    {
        static void Main()
        {
            WorkflowInvoker.Invoke(new HolidayApprovalWF());
        }
    }

    public sealed class HolidayApprovalWF : global::System.Activities.Activity
    {
        public global::System.Activities.InArgument<HolidayRequestEntity> HolidayRequest
        {
            get;
            set;
        }

        private global::System.Activities.Activity __GetImplementation__()
        {
            global::System.Activities.Activity __vroot__;
            {
                var __v__0 = new global::System.Activities.Statements.Sequence();
                var staffManagerDecision = new global::System.Activities.Variable<bool>();
                __v__0.Variables.Add(staffManagerDecision);
                __v__0.Activities.Add(new CreateHolidayApprovalTaskActivity().Initialize(__activity2__ =>
                {
                    __activity2__.HolidayRequest = new global::System.Activities.InArgument<global::HolidayApproval.Entities.HolidayRequestEntity>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.HolidayRequestEntity>(__ctx__ => HolidayRequest.Get(__ctx__)));
                    __activity2__.ResponsibleUser = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => "StaffManager"));
                    __activity2__.Result = new global::System.Activities.OutArgument<bool>(new global::MetahWLocationActivity<bool>(staffManagerDecision));
                }

                ));
                var __v__1 = new global::System.Activities.Statements.If();
                __v__1.Condition = new global::System.Activities.InArgument<bool>(new global::MetahWFuncActivity<bool>(__ctx__ => !staffManagerDecision.Get(__ctx__)));
                var __v__2 = new global::System.Activities.Statements.Sequence();
                __v__2.Activities.Add(new UpdateHolidayRequestStateActivity().Initialize(__activity2__ =>
                {
                    __activity2__.HolidayRequest = new global::System.Activities.InArgument<global::HolidayApproval.Entities.HolidayRequestEntity>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.HolidayRequestEntity>(__ctx__ => HolidayRequest.Get(__ctx__)));
                    __activity2__.NewState = new global::System.Activities.InArgument<global::HolidayApproval.Entities.EHolidayRequestState>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.EHolidayRequestState>(__ctx__ => EHolidayRequestState.Refused));
                }

                ));
                __v__2.Activities.Add(new SendEmailActivity().Initialize(__activity2__ =>
                {
                    __activity2__.Subject = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => "Holiday request"));
                    __activity2__.Body = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => "Your holiday request has been refused!"));
                    __activity2__.ToMailAddress = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => HolidayRequest.Get(__ctx__).Originator.EmailAddress));
                }

                ));
                __v__1.Then = __v__2;
                var __v__3 = new global::System.Activities.Statements.Sequence();
                __v__3.Activities.Add(new UpdateHolidayRequestStateActivity().Initialize(__activity2__ =>
                {
                    __activity2__.HolidayRequest = new global::System.Activities.InArgument<global::HolidayApproval.Entities.HolidayRequestEntity>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.HolidayRequestEntity>(__ctx__ => HolidayRequest.Get(__ctx__)));
                    __activity2__.NewState = new global::System.Activities.InArgument<global::HolidayApproval.Entities.EHolidayRequestState>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.EHolidayRequestState>(__ctx__ => EHolidayRequestState.Approved));
                }

                ));
                __v__3.Activities.Add(new SendEmailActivity().Initialize(__activity2__ =>
                {
                    __activity2__.Subject = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => "Holiday request"));
                    __activity2__.Body = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => "Your holiday request has been approved!"));
                    __activity2__.ToMailAddress = new global::System.Activities.InArgument<string>(new global::MetahWFuncActivity<string>(__ctx__ => HolidayRequest.Get(__ctx__).Originator.EmailAddress));
                }

                ));
                __v__3.Activities.Add(new BookHolidayRequestActivity().Initialize(__activity2__ =>
                {
                    __activity2__.HolidayRequest = new global::System.Activities.InArgument<global::HolidayApproval.Entities.HolidayRequestEntity>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.HolidayRequestEntity>(__ctx__ => HolidayRequest.Get(__ctx__)));
                }

                ));
                __v__3.Activities.Add(new UpdateHolidayRequestStateActivity().Initialize(__activity2__ =>
                {
                    __activity2__.HolidayRequest = new global::System.Activities.InArgument<global::HolidayApproval.Entities.HolidayRequestEntity>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.HolidayRequestEntity>(__ctx__ => HolidayRequest.Get(__ctx__)));
                    __activity2__.NewState = new global::System.Activities.InArgument<global::HolidayApproval.Entities.EHolidayRequestState>(new global::MetahWFuncActivity<global::HolidayApproval.Entities.EHolidayRequestState>(__ctx__ => EHolidayRequestState.Booked));
                }

                ));
                __v__1.Else = __v__3;
                __v__0.Activities.Add(__v__1);
                __vroot__ = __v__0;
            }

            return __vroot__;
        }

        private global::System.Func<global::System.Activities.Activity> __implementation__;
        protected override global::System.Func<global::System.Activities.Activity> Implementation
        {
            get
            {
                return __implementation__ ?? (__implementation__ = __GetImplementation__);
            }

            set
            {
                throw new global::System.NotSupportedException();
            }
        }
    }

    public sealed class CreateHolidayApprovalTaskActivity : global::System.Activities.Activity<bool>
    {
        public global::System.Activities.InArgument<HolidayRequestEntity> HolidayRequest
        {
            get;
            set;
        }

        public global::System.Activities.InArgument<string> ResponsibleUser
        {
            get;
            set;
        }

        private global::System.Activities.Activity __GetImplementation__()
        {
            global::System.Activities.Activity __vroot__;
            __vroot__ = new global::MetahWActionActivity(__ctx__ =>
            {
                Result.SetEx(__ctx__, false);
            }

            );
            return __vroot__;
        }

        private global::System.Func<global::System.Activities.Activity> __implementation__;
        protected override global::System.Func<global::System.Activities.Activity> Implementation
        {
            get
            {
                return __implementation__ ?? (__implementation__ = __GetImplementation__);
            }

            set
            {
                throw new global::System.NotSupportedException();
            }
        }
    }

    public sealed class UpdateHolidayRequestStateActivity : global::System.Activities.Activity
    {
        public global::System.Activities.InArgument<HolidayRequestEntity> HolidayRequest
        {
            get;
            set;
        }

        public global::System.Activities.InArgument<EHolidayRequestState> NewState
        {
            get;
            set;
        }

        private global::System.Activities.Activity __GetImplementation__()
        {
            global::System.Activities.Activity __vroot__;
            __vroot__ = new global::MetahWActionActivity(null);
            return __vroot__;
        }

        private global::System.Func<global::System.Activities.Activity> __implementation__;
        protected override global::System.Func<global::System.Activities.Activity> Implementation
        {
            get
            {
                return __implementation__ ?? (__implementation__ = __GetImplementation__);
            }

            set
            {
                throw new global::System.NotSupportedException();
            }
        }
    }

    public sealed class SendEmailActivity : global::System.Activities.Activity
    {
        public global::System.Activities.InArgument<string> Subject
        {
            get;
            set;
        }

        public global::System.Activities.InArgument<string> Body
        {
            get;
            set;
        }

        public global::System.Activities.InArgument<string> ToMailAddress
        {
            get;
            set;
        }

        private global::System.Activities.Activity __GetImplementation__()
        {
            global::System.Activities.Activity __vroot__;
            __vroot__ = new global::MetahWActionActivity(null);
            return __vroot__;
        }

        private global::System.Func<global::System.Activities.Activity> __implementation__;
        protected override global::System.Func<global::System.Activities.Activity> Implementation
        {
            get
            {
                return __implementation__ ?? (__implementation__ = __GetImplementation__);
            }

            set
            {
                throw new global::System.NotSupportedException();
            }
        }
    }

    public sealed class BookHolidayRequestActivity : global::System.Activities.Activity
    {
        public global::System.Activities.InArgument<HolidayRequestEntity> HolidayRequest
        {
            get;
            set;
        }

        private global::System.Activities.Activity __GetImplementation__()
        {
            global::System.Activities.Activity __vroot__;
            __vroot__ = new global::MetahWActionActivity(null);
            return __vroot__;
        }

        private global::System.Func<global::System.Activities.Activity> __implementation__;
        protected override global::System.Func<global::System.Activities.Activity> Implementation
        {
            get
            {
                return __implementation__ ?? (__implementation__ = __GetImplementation__);
            }

            set
            {
                throw new global::System.NotSupportedException();
            }
        }
    }
}

namespace System
{
    internal delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17);
    internal delegate void Action<in T1, in T2, in T3, in T4, in T5, in T6, in T7, in T8, in T9, in T10, in T11, in T12, in T13, in T14, in T15, in T16, in T17, in T18>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18);
}

internal static class MetahWExtensions
{
    internal static T SetEx<T>(this global::System.Activities.InArgument<T> argOrVar, global::System.Activities.ActivityContext context, T value)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        argOrVar.Set(context, value);
        return value;
    }

    internal static T SetEx<T>(this global::System.Activities.InArgument<T> argOrVar, global::System.Activities.ActivityContext context, global::System.Func<T, T> func, bool isPost)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        var oldValue = argOrVar.Get(context);
        var newValue = func(oldValue);
        argOrVar.Set(context, newValue);
        if (isPost)
            return oldValue;
        return newValue;
    }

    internal static T SetEx<T>(this global::System.Activities.OutArgument<T> argOrVar, global::System.Activities.ActivityContext context, T value)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        argOrVar.Set(context, value);
        return value;
    }

    internal static T SetEx<T>(this global::System.Activities.OutArgument<T> argOrVar, global::System.Activities.ActivityContext context, global::System.Func<T, T> func, bool isPost)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        var oldValue = argOrVar.Get(context);
        var newValue = func(oldValue);
        argOrVar.Set(context, newValue);
        if (isPost)
            return oldValue;
        return newValue;
    }

    internal static T SetEx<T>(this global::System.Activities.InOutArgument<T> argOrVar, global::System.Activities.ActivityContext context, T value)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        argOrVar.Set(context, value);
        return value;
    }

    internal static T SetEx<T>(this global::System.Activities.InOutArgument<T> argOrVar, global::System.Activities.ActivityContext context, global::System.Func<T, T> func, bool isPost)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        var oldValue = argOrVar.Get(context);
        var newValue = func(oldValue);
        argOrVar.Set(context, newValue);
        if (isPost)
            return oldValue;
        return newValue;
    }

    internal static T SetEx<T>(this global::System.Activities.Variable<T> argOrVar, global::System.Activities.ActivityContext context, T value)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        argOrVar.Set(context, value);
        return value;
    }

    internal static T SetEx<T>(this global::System.Activities.Variable<T> argOrVar, global::System.Activities.ActivityContext context, global::System.Func<T, T> func, bool isPost)
    {
        if (argOrVar == null)
            throw new global::System.ArgumentNullException("argOrVar");
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        var oldValue = argOrVar.Get(context);
        var newValue = func(oldValue);
        argOrVar.Set(context, newValue);
        if (isPost)
            return oldValue;
        return newValue;
    }

    internal static T Initialize<T>(this T activity, global::System.Action<T> action)where T : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (action != null)
            action(activity);
        return activity;
    }

    internal static TActivity Initialize<TDelegate, TActivity>(this TDelegate deleg, global::System.Func<TDelegate, TActivity> func)where TDelegate : global::System.Activities.ActivityDelegate where TActivity : global::System.Activities.Activity
    {
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        return func(deleg);
    }

    private static readonly global::System.Reflection.PropertyInfo _allowChainedEnvironmentAccessPropertyInfo = typeof (global::System.Activities.ActivityContext).GetProperty("AllowChainedEnvironmentAccess", (global::System.Reflection.BindingFlags)36);
    internal static void SetAllowChainedEnvironmentAccess(this global::System.Activities.ActivityContext context, bool value)
    {
        _allowChainedEnvironmentAccessPropertyInfo.SetValue(context, value);
    }

    internal static global::System.Activities.ActivityAction ToAction(this global::System.Activities.Activity activity)
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        return new global::System.Activities.ActivityAction()
        {
        Handler = activity
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1> ToAction<TActivity, T1>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1));
        return new global::System.Activities.ActivityAction<T1>()
        {
        Handler = activity, Argument = darg1
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2> ToAction<TActivity, T1, T2>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2));
        return new global::System.Activities.ActivityAction<T1, T2>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3> ToAction<TActivity, T1, T2, T3>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3));
        return new global::System.Activities.ActivityAction<T1, T2, T3>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4> ToAction<TActivity, T1, T2, T3, T4>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5> ToAction<TActivity, T1, T2, T3, T4, T5>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6> ToAction<TActivity, T1, T2, T3, T4, T5, T6>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>, global::System.Activities.InArgument<T15>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        var darg15 = new global::System.Activities.DelegateInArgument<T15>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14), new global::System.Activities.InArgument<T15>(darg15));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14, Argument15 = darg15
        }

        ;
    }

    internal static global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> ToAction<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>, global::System.Activities.InArgument<T15>, global::System.Activities.InArgument<T16>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        var darg15 = new global::System.Activities.DelegateInArgument<T15>();
        var darg16 = new global::System.Activities.DelegateInArgument<T16>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14), new global::System.Activities.InArgument<T15>(darg15), new global::System.Activities.InArgument<T16>(darg16));
        return new global::System.Activities.ActivityAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14, Argument15 = darg15, Argument16 = darg16
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<TResult> ToFunc<TActivity, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<TResult>()
        {
        Handler = activity, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, TResult> ToFunc<TActivity, T1, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, TResult>()
        {
        Handler = activity, Argument = darg1, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, TResult> ToFunc<TActivity, T1, T2, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, TResult> ToFunc<TActivity, T1, T2, T3, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, TResult> ToFunc<TActivity, T1, T2, T3, T4, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>, global::System.Activities.InArgument<T15>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        var darg15 = new global::System.Activities.DelegateInArgument<T15>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14), new global::System.Activities.InArgument<T15>(darg15), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14, Argument15 = darg15, Result = dres
        }

        ;
    }

    internal static global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> ToFunc<TActivity, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(this TActivity activity, global::System.Action<TActivity, global::System.Activities.InArgument<T1>, global::System.Activities.InArgument<T2>, global::System.Activities.InArgument<T3>, global::System.Activities.InArgument<T4>, global::System.Activities.InArgument<T5>, global::System.Activities.InArgument<T6>, global::System.Activities.InArgument<T7>, global::System.Activities.InArgument<T8>, global::System.Activities.InArgument<T9>, global::System.Activities.InArgument<T10>, global::System.Activities.InArgument<T11>, global::System.Activities.InArgument<T12>, global::System.Activities.InArgument<T13>, global::System.Activities.InArgument<T14>, global::System.Activities.InArgument<T15>, global::System.Activities.InArgument<T16>, global::System.Activities.OutArgument<TResult>> initializer)where TActivity : global::System.Activities.Activity
    {
        if (activity == null)
            throw new global::System.ArgumentNullException("activity");
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        var darg1 = new global::System.Activities.DelegateInArgument<T1>();
        var darg2 = new global::System.Activities.DelegateInArgument<T2>();
        var darg3 = new global::System.Activities.DelegateInArgument<T3>();
        var darg4 = new global::System.Activities.DelegateInArgument<T4>();
        var darg5 = new global::System.Activities.DelegateInArgument<T5>();
        var darg6 = new global::System.Activities.DelegateInArgument<T6>();
        var darg7 = new global::System.Activities.DelegateInArgument<T7>();
        var darg8 = new global::System.Activities.DelegateInArgument<T8>();
        var darg9 = new global::System.Activities.DelegateInArgument<T9>();
        var darg10 = new global::System.Activities.DelegateInArgument<T10>();
        var darg11 = new global::System.Activities.DelegateInArgument<T11>();
        var darg12 = new global::System.Activities.DelegateInArgument<T12>();
        var darg13 = new global::System.Activities.DelegateInArgument<T13>();
        var darg14 = new global::System.Activities.DelegateInArgument<T14>();
        var darg15 = new global::System.Activities.DelegateInArgument<T15>();
        var darg16 = new global::System.Activities.DelegateInArgument<T16>();
        var dres = new global::System.Activities.DelegateOutArgument<TResult>();
        initializer(activity, new global::System.Activities.InArgument<T1>(darg1), new global::System.Activities.InArgument<T2>(darg2), new global::System.Activities.InArgument<T3>(darg3), new global::System.Activities.InArgument<T4>(darg4), new global::System.Activities.InArgument<T5>(darg5), new global::System.Activities.InArgument<T6>(darg6), new global::System.Activities.InArgument<T7>(darg7), new global::System.Activities.InArgument<T8>(darg8), new global::System.Activities.InArgument<T9>(darg9), new global::System.Activities.InArgument<T10>(darg10), new global::System.Activities.InArgument<T11>(darg11), new global::System.Activities.InArgument<T12>(darg12), new global::System.Activities.InArgument<T13>(darg13), new global::System.Activities.InArgument<T14>(darg14), new global::System.Activities.InArgument<T15>(darg15), new global::System.Activities.InArgument<T16>(darg16), new global::System.Activities.OutArgument<TResult>(dres));
        return new global::System.Activities.ActivityFunc<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>()
        {
        Handler = activity, Argument1 = darg1, Argument2 = darg2, Argument3 = darg3, Argument4 = darg4, Argument5 = darg5, Argument6 = darg6, Argument7 = darg7, Argument8 = darg8, Argument9 = darg9, Argument10 = darg10, Argument11 = darg11, Argument12 = darg12, Argument13 = darg13, Argument14 = darg14, Argument15 = darg15, Argument16 = darg16, Result = dres
        }

        ;
    }
}

internal sealed class MetahWFuncActivity<T> : global::System.Activities.CodeActivity<T>
{
    internal MetahWFuncActivity(global::System.Func<global::System.Activities.ActivityContext, T> func)
    {
        if (func == null)
            throw new global::System.ArgumentNullException("func");
        Func = func;
    }

    internal readonly global::System.Func<global::System.Activities.ActivityContext, T> Func;
    protected override void CacheMetadata(global::System.Activities.CodeActivityMetadata metadata)
    {
    }

    protected override T Execute(global::System.Activities.CodeActivityContext context)
    {
        try
        {
            context.SetAllowChainedEnvironmentAccess(true);
            return Func(context);
        }
        finally
        {
            context.SetAllowChainedEnvironmentAccess(false);
        }
    }
}

internal sealed class MetahWActionActivity : global::System.Activities.CodeActivity
{
    internal MetahWActionActivity(global::System.Action<global::System.Activities.ActivityContext> action)
    {
        Action = action;
    }

    internal readonly global::System.Action<global::System.Activities.ActivityContext> Action;
    protected override void CacheMetadata(global::System.Activities.CodeActivityMetadata metadata)
    {
    }

    protected override void Execute(global::System.Activities.CodeActivityContext context)
    {
        try
        {
            context.SetAllowChainedEnvironmentAccess(true);
            if (Action != null)
                Action(context);
        }
        finally
        {
            context.SetAllowChainedEnvironmentAccess(false);
        }
    }
}

internal sealed class MetahWLocationActivity<T> : global::System.Activities.CodeActivity<global::System.Activities.Location<T>>
{
    internal MetahWLocationActivity(global::System.Activities.Variable<T> variable)
    {
        if (variable == null)
            throw new global::System.ArgumentNullException("variable");
        Variable = variable;
    }

    internal MetahWLocationActivity(global::System.Activities.Argument argument)
    {
        if (argument == null)
            throw new global::System.ArgumentNullException("argument");
        Argument = argument;
    }

    internal readonly global::System.Activities.Variable<T> Variable;
    internal readonly global::System.Activities.Argument Argument;
    protected override void CacheMetadata(global::System.Activities.CodeActivityMetadata metadata)
    {
    }

    protected override global::System.Activities.Location<T> Execute(global::System.Activities.CodeActivityContext context)
    {
        try
        {
            context.SetAllowChainedEnvironmentAccess(true);
            if (Variable != null)
                return Variable.GetLocation(context);
            return (global::System.Activities.Location<T>)Argument.GetLocation(context);
        }
        finally
        {
            context.SetAllowChainedEnvironmentAccess(false);
        }
    }
}

internal sealed class MetahWWrapperActivity<T> : global::System.Activities.NativeActivity
{
    [global::System.Activities.RequiredArgumentAttribute()]
    public global::System.Activities.InArgument<T> Argument
    {
        get;
        set;
    }

    public global::System.Activities.Activity Child
    {
        get;
        set;
    }

    internal global::System.Activities.ActivityAction<T> ToAction()
    {
        return this.ToAction<global::MetahWWrapperActivity<T>, T>((ac, arg) =>
        {
            ac.Argument = arg;
        }

        );
    }

    protected override void CacheMetadata(global::System.Activities.NativeActivityMetadata metadata)
    {
        var rtArgument = new global::System.Activities.RuntimeArgument("Argument", typeof (T), global::System.Activities.ArgumentDirection.In, true);
        metadata.Bind(Argument, rtArgument);
        metadata.AddArgument(rtArgument);
        metadata.AddChild(Child);
    }

    protected override void Execute(global::System.Activities.NativeActivityContext context)
    {
        if (Child != null)
            context.ScheduleActivity(Child);
    }
}

internal sealed class MetahWSequenceActivity<T> : global::System.Activities.NativeActivity<T>
{
    internal MetahWSequenceActivity(global::System.Action<global::MetahWSequenceActivity<T>> initializer)
    {
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        Variables = new global::System.Collections.ObjectModel.Collection<global::System.Activities.Variable>();
        Activities = new global::System.Collections.ObjectModel.Collection<global::System.Activities.Activity>();
        initializer(this);
        if (Variables.Count == 0)
            throw new global::System.InvalidOperationException("Variables.Count == 0");
        if (Activities.Count == 0)
            throw new global::System.InvalidOperationException("Activities.Count == 0");
        _commonCallback = CommonCallback;
        _finalCallback = FinalCallback;
    }

    internal readonly global::System.Collections.ObjectModel.Collection<global::System.Activities.Variable> Variables;
    internal readonly global::System.Collections.ObjectModel.Collection<global::System.Activities.Activity> Activities;
    private readonly global::System.Activities.CompletionCallback _commonCallback;
    private readonly global::System.Activities.CompletionCallback<T> _finalCallback;
    protected override void CacheMetadata(global::System.Activities.NativeActivityMetadata metadata)
    {
        metadata.SetVariablesCollection(Variables);
        metadata.SetChildrenCollection(Activities);
    }

    protected override void Execute(global::System.Activities.NativeActivityContext context)
    {
        CommonCallback(context, null);
    }

    private void CommonCallback(global::System.Activities.NativeActivityContext context, global::System.Activities.ActivityInstance completedInstance)
    {
        try
        {
            context.SetAllowChainedEnvironmentAccess(true);
            var indexVar = (global::System.Activities.Variable<int>)Variables[0];
            var index = indexVar.Get(context);
            var child = Activities[index];
            if (index == Activities.Count - 1)
                context.ScheduleActivity<T>((global::System.Activities.Activity<T>)child, _finalCallback);
            else
            {
                context.ScheduleActivity(child, _commonCallback);
                indexVar.Set(context, index + 1);
            }
        }
        finally
        {
            context.SetAllowChainedEnvironmentAccess(false);
        }
    }

    private void FinalCallback(global::System.Activities.NativeActivityContext context, global::System.Activities.ActivityInstance completedInstance, T result)
    {
        Result.Set(context, result);
    }
}

internal sealed class MetahWSequenceActivity : global::System.Activities.NativeActivity
{
    internal MetahWSequenceActivity(global::System.Action<global::MetahWSequenceActivity> initializer)
    {
        if (initializer == null)
            throw new global::System.ArgumentNullException("initializer");
        Variables = new global::System.Collections.ObjectModel.Collection<global::System.Activities.Variable>();
        Activities = new global::System.Collections.ObjectModel.Collection<global::System.Activities.Activity>();
        initializer(this);
        if (Variables.Count == 0)
            throw new global::System.InvalidOperationException("Variables.Count == 0");
        if (Activities.Count == 0)
            throw new global::System.InvalidOperationException("Activities.Count == 0");
        _commonCallback = CommonCallback;
    }

    internal readonly global::System.Collections.ObjectModel.Collection<global::System.Activities.Variable> Variables;
    internal readonly global::System.Collections.ObjectModel.Collection<global::System.Activities.Activity> Activities;
    private readonly global::System.Activities.CompletionCallback _commonCallback;
    protected override void CacheMetadata(global::System.Activities.NativeActivityMetadata metadata)
    {
        metadata.SetVariablesCollection(Variables);
        metadata.SetChildrenCollection(Activities);
    }

    protected override void Execute(global::System.Activities.NativeActivityContext context)
    {
        CommonCallback(context, null);
    }

    private void CommonCallback(global::System.Activities.NativeActivityContext context, global::System.Activities.ActivityInstance completedInstance)
    {
        try
        {
            context.SetAllowChainedEnvironmentAccess(true);
            var indexVar = (global::System.Activities.Variable<int>)Variables[0];
            var index = indexVar.Get(context);
            if (index < Activities.Count)
            {
                context.ScheduleActivity(Activities[index], _commonCallback);
                indexVar.Set(context, index + 1);
            }
        }
        finally
        {
            context.SetAllowChainedEnvironmentAccess(false);
        }
    }
}