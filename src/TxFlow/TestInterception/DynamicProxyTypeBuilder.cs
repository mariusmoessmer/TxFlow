using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;
using System.Linq.Expressions;

namespace DynamicProxyImplementation
{
    public class DynamicProxyTypeBuilder
    {
        private static Assembly assembly = null;
        public static Type CreateType<TInterfaceType, TProxyType>() where TProxyType : DynamicProxy
        {
            if(assembly == null)
            {
                assembly = Assembly.LoadFrom("yeah.dll");
            }
            return assembly.GetTypes().First();
            //return new DynamicProxyTypeBuilder().createType<TInterfaceType, TProxyType>();
        }

        private Dictionary<string, Type> dynamicTypes = new Dictionary<string, Type>();
        private SpinLock dynamicTypeEmitSyncRoot = new SpinLock();

        private string ownClassName = typeof(DynamicProxyTypeBuilder).Name;
        private string getTypeFromHandleMethodName = GetMethodCallExpressionMethodInfo<RuntimeTypeHandle>(t => Type.GetTypeFromHandle(t)).Name;
        private string listAddMethodName = GetMethodCallExpressionMethodInfo<List<object>>(l => l.Add(new object())).Name;
        private string listToArrayMethodName = GetMethodCallExpressionMethodInfo<List<object>>(l => l.ToArray()).Name;
        private string createInstanceMethodName = GetMethodCallExpressionMethodInfo<object>(a => Activator.CreateInstance(a.GetType())).Name;

        private MethodInfo listAddMethodInfo = null;
        private MethodInfo listToArrayMethodInfo = null;
        private MethodInfo getTypeFromHandleMethodInfo = null;
        private MethodInfo activatorCreateInstanceMethodInfo = null;

        private MethodInfo tryInvokeMemberInfo = null;
        private FieldBuilder _proxyField;

        private DynamicProxyTypeBuilder()
        {
            tryInvokeMemberInfo = DynamicProxy.TryInvokeMemberMethodInfo;

            listAddMethodInfo = TypeHelper.GetMethodInfo(typeof(List<object>), listAddMethodName, new Type[] { typeof(object) });
            listToArrayMethodInfo = TypeHelper.GetMethodInfo(typeof(List<object>), listToArrayMethodName, Type.EmptyTypes);

            getTypeFromHandleMethodInfo = TypeHelper.GetMethodInfo(typeof(Type), getTypeFromHandleMethodName, new Type[] { typeof(RuntimeTypeHandle) });
            activatorCreateInstanceMethodInfo = TypeHelper.GetMethodInfo(typeof(Activator), createInstanceMethodName, new Type[] { typeof(Type) });
        }


        public Type createType<TInterfaceType, TProxyType>() where TProxyType : DynamicProxy
        {
            Type ownClass = typeof(DynamicProxyTypeBuilder);
            AssemblyName assemblyName = new AssemblyName(string.Concat(ownClass.Namespace, ".", ownClass.Name, "_", typeof(TInterfaceType).Name, typeof(TProxyType).FullName));

            AssemblyBuilder ab = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = ab.DefineDynamicModule(assemblyName.Name, string.Concat(assemblyName.Name, ".dll"));

            Type ret;

            string typeName = string.Concat(ownClassName, "+", typeof(TInterfaceType).FullName);
            bool gotLock = false;
            try
            {
                dynamicTypeEmitSyncRoot.Enter(ref gotLock);
                dynamicTypes.TryGetValue(typeName, out ret);

                if (ret == null)
                {
                    TypeBuilder tb = moduleBuilder.DefineType(typeName, TypeAttributes.Public);
                    //tb.SetParent(typeof(TProxyType));
                    tb.SetParent(typeof(TInterfaceType));
                    GenerateConstructor(tb);

                    //tb.AddInterfaceImplementation(typeof(TInterfaceType));

                    DynamicImplementInterface(new List<Type> { typeof(TInterfaceType) }, new List<string>(), typeof(TInterfaceType), tb);
                    ret = tb.CreateType();

                    dynamicTypes.Add(typeName, ret);
                }
            }
            finally
            {
                if (gotLock)
                {
                    dynamicTypeEmitSyncRoot.Exit();
                }
            }

            return ret;
        }

        private void GenerateConstructor(TypeBuilder tb)
        {
            _proxyField = tb.DefineField("_proxy", typeof(DynamicProxy), FieldAttributes.Private );

            var ctor = tb.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(DynamicProxy) });
            var ctorILGen = ctor.GetILGenerator();
            //getIL.Emit(OpCodes.Nop);

            ctorILGen.Emit(OpCodes.Ldarg_0);
            ctorILGen.Emit(OpCodes.Ldarg_1);
            ctorILGen.Emit(OpCodes.Stfld, _proxyField);

            ctorILGen.Emit(OpCodes.Ret);
        }

        private void DynamicImplementInterface(List<Type> implementedInterfaceList, List<string> usedNames, Type interfaceType, TypeBuilder tb)
        {
            if (interfaceType != typeof(IDisposable))
            {
                List<MethodInfo> propAccessorList = new List<MethodInfo>();

                GenerateMethods(usedNames, interfaceType, tb, propAccessorList);

                foreach (Type i in interfaceType.GetInterfaces())
                {
                    if (!implementedInterfaceList.Contains(i))
                    {
                        DynamicImplementInterface(implementedInterfaceList, usedNames, i, tb);
                        implementedInterfaceList.Add(i);
                    }
                }
            }
        }

        private void EmitAndStoreGetTypeFromHandle(ILGenerator ilGenerator, Type type, OpCode storeCode)
        {
            //C#: Type.GetTypeFromHandle(interfaceType)
            ilGenerator.Emit(OpCodes.Ldtoken, type);
            ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandleMethodInfo, null);
            ilGenerator.Emit(storeCode);
        }

        private void EmitInvokeMethod(MethodInfo mi, MethodBuilder mb)
        {
            ILGenerator ilGenerator = mb.GetILGenerator();

            string methodName = mb.Name;
            LocalBuilder typeLb = ilGenerator.DeclareLocal(typeof(Type), true);
            LocalBuilder paramsLb = ilGenerator.DeclareLocal(typeof(List<object>), true);
            LocalBuilder resultLb = ilGenerator.DeclareLocal(typeof(object), true);
            LocalBuilder retLb = ilGenerator.DeclareLocal(typeof(bool), true);

            //C#: Type.GetTypeFromHandle(interfaceType)
            EmitAndStoreGetTypeFromHandle(ilGenerator, mi.DeclaringType, OpCodes.Stloc_0);

            //C#: params = new List<object>()
            ilGenerator.Emit(OpCodes.Newobj, typeof(List<object>).GetConstructor(Type.EmptyTypes));
            ilGenerator.Emit(OpCodes.Stloc_1);

            int i = 0;
            foreach (ParameterInfo pi in mi.GetParameters())
            {
                //C#: params.Add(param[i])
                i++;
                ilGenerator.Emit(OpCodes.Ldloc_1);
                ilGenerator.Emit(OpCodes.Ldarg, i);
                if (pi.ParameterType.IsValueType)
                {
                    ilGenerator.Emit(OpCodes.Box, pi.ParameterType);
                }
                ilGenerator.EmitCall(OpCodes.Callvirt, listAddMethodInfo, null);
            }
            //C#: ret = DynamicProxy.TryInvokeMember(interfaceType, propertyName, params, out result)
            ilGenerator.Emit(OpCodes.Ldarg_0); // this
            ilGenerator.Emit(OpCodes.Ldfld, _proxyField);
            ilGenerator.Emit(OpCodes.Ldloc_0); // type
            ilGenerator.Emit(OpCodes.Ldstr, methodName); // methodename
            ilGenerator.Emit(OpCodes.Ldloc_1); // list<>
            ilGenerator.EmitCall(OpCodes.Callvirt, listToArrayMethodInfo, null);
            ilGenerator.Emit(OpCodes.Ldloca_S, 2); // result
            ilGenerator.EmitCall(OpCodes.Callvirt, tryInvokeMemberInfo, null);
            //ilGenerator.EmitCall(OpCodes.Callvirt, _proxyField.FieldType.GetMethod("TryInvokeMember"), null);
            ilGenerator.Emit(OpCodes.Stloc_3);

            if (mi.ReturnType != typeof(void))
            {
                ilGenerator.Emit(OpCodes.Ldloc_2);
                //C#: if(ret == ValueType && ret == null){
                //    ret = Activator.CreateInstance(returnType) }
                if (mi.ReturnType.IsValueType)
                {
                    Label retisnull = ilGenerator.DefineLabel();
                    Label endofif = ilGenerator.DefineLabel();

                    ilGenerator.Emit(OpCodes.Ldnull);
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Brtrue_S, retisnull);
                    ilGenerator.Emit(OpCodes.Ldloc_2);
                    ilGenerator.Emit(OpCodes.Unbox_Any, mi.ReturnType);
                    ilGenerator.Emit(OpCodes.Br_S, endofif);
                    ilGenerator.MarkLabel(retisnull);
                    ilGenerator.Emit(OpCodes.Ldtoken, mi.ReturnType);
                    ilGenerator.EmitCall(OpCodes.Call, getTypeFromHandleMethodInfo, null);
                    ilGenerator.EmitCall(OpCodes.Call, activatorCreateInstanceMethodInfo, null);
                    ilGenerator.Emit(OpCodes.Unbox_Any, mi.ReturnType);
                    ilGenerator.MarkLabel(endofif);
                }
            }
            //C#: return ret
            ilGenerator.Emit(OpCodes.Ret);
        }

        private void GenerateMethods(List<string> usedNames, Type interfaceType, TypeBuilder tb, List<MethodInfo> propAccessors)
        {
            foreach (MethodInfo mi in interfaceType.GetMethods())
            {
                if (mi.DeclaringType != typeof(object) && (interfaceType.IsInterface || mi.IsAbstract || mi.IsVirtual))
                {
                    var parameterInfoArray = mi.GetParameters();
                    var genericArgumentArray = mi.GetGenericArguments();

                    string paramNames = string.Join(", ", parameterInfoArray.Select(pi => pi.ParameterType));
                    string nameWithParams = string.Concat(mi.Name, "(", paramNames, ")");
                    if (usedNames.Contains(nameWithParams))
                    {
                        throw new NotSupportedException(string.Format("Error in interface {1}! Method '{0}' already used in other child interface!", nameWithParams, interfaceType.Name)); //LOCSTR
                    }
                    else
                    {
                        usedNames.Add(nameWithParams);
                    }

                    if (!propAccessors.Contains(mi))
                    {
                        MethodAttributes attributes = mi.IsAbstract ? MethodAttributes.Public | MethodAttributes.ReuseSlot | MethodAttributes.Virtual | MethodAttributes.HideBySig
                        : MethodAttributes.Public | MethodAttributes.Virtual;

                        MethodBuilder mb = tb.DefineMethod(mi.Name, attributes, mi.ReturnType, parameterInfoArray.Select(pi => pi.ParameterType).ToArray());

                        if (genericArgumentArray.Any())
                        {
                            mb.DefineGenericParameters(genericArgumentArray.Select(s => s.Name).ToArray());
                        }

                        EmitInvokeMethod(mi, mb);

                        if (!mi.IsAbstract)
                        {
                            tb.DefineMethodOverride(mb, mi);
                        }
                    }
                }
            }
        }

        public static MethodInfo GetMethodCallExpressionMethodInfo<T>(Expression<Action<T>> exp)
        {
            return getMethodCallExpressionMethodInfo((Expression)exp);
        }

        public static MethodInfo GetMethodCallExpressionMethodInfo<T>(Expression<Func<T, object>> exp)
        {
            return getMethodCallExpressionMethodInfo((Expression)exp);
        }

        private static MethodInfo getMethodCallExpressionMethodInfo(Expression exp)
        {
            MethodInfo ret = null;

            if (exp == null)
            {
                throw new ArgumentNullException("exp");
            }

            MethodCallExpression mcexp = exp as MethodCallExpression;
            if (mcexp == null)
            {
                LambdaExpression lex = exp as LambdaExpression;
                if (lex == null)
                {
                    UnaryExpression uex = exp as UnaryExpression;
                    if ((uex == null) || (uex.NodeType != ExpressionType.Convert))
                    {
                        throw new InvalidOperationException("Neither a MethodCallExpression nor a LambdaExpression wrapped MethodCallExpression nor a ConvertExpression wrapped MethodCallExpression!");   //LOCSTR
                    }
                    else
                    {
                        ret = getMethodCallExpressionMethodInfo(uex.Operand);
                    }
                }
                else
                {
                    ret = getMethodCallExpressionMethodInfo(lex.Body);
                }
            }
            else
            {
                ret = mcexp.Method;
            }

            return ret;
        }
    }

    public static class TypeHelper
    {
        public static bool IsImplementingInterface(Type toCheck, Type interfaceType)
        {
            if (toCheck == null)
            {
                throw new ArgumentNullException("toCheck");
            }
            if (interfaceType == null)
            {
                throw new ArgumentNullException("interfaceType");
            }

            bool ret = false;

            string cacheKey = String.Concat(toCheck.FullName, "+", interfaceType.FullName);
            object c = null;
            if (c == null)
            {
                if (toCheck == interfaceType)
                {
                    ret = true;
                }
                else
                {
                    if (interfaceType.IsGenericTypeDefinition)
                    {
                        if (toCheck.Assembly.ReflectionOnly || interfaceType.Assembly.ReflectionOnly)
                        {
                            ret = toCheck.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition().AssemblyQualifiedName == interfaceType.AssemblyQualifiedName);
                        }
                        else
                        {
                            ret = toCheck.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == interfaceType);
                        }
                    }
                    else
                    {
                        if (toCheck.Assembly.ReflectionOnly || interfaceType.Assembly.ReflectionOnly)
                        {
                            ret = toCheck.GetInterfaces().Any(x => x.AssemblyQualifiedName == interfaceType.AssemblyQualifiedName);
                        }
                        else
                        {
                            ret = toCheck.GetInterfaces().Any(x => x == interfaceType);
                        }
                    }
                }
            }
            else
            {
                ret = (bool)c;
            }

            return ret;
        }

        public static bool IsSubclassOf(Type toCheck, Type baseType)
        {
            if (toCheck == null)
            {
                throw new ArgumentNullException("toCheck");
            }
            if (baseType == null)
            {
                throw new ArgumentNullException("baseType");
            }

            bool ret = false;

            string cacheKey = String.Concat(toCheck.FullName, "+", baseType.FullName);
            object c = null;
            if (c == null)
            {
                if (toCheck.IsSubclassOf(baseType))
                {
                    ret = true;
                }
                else
                {
                    if (toCheck.Assembly.ReflectionOnly || baseType.Assembly.ReflectionOnly)
                    {
                        while (toCheck.AssemblyQualifiedName != typeof(object).AssemblyQualifiedName && toCheck != null)
                        {
                            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                            if (baseType.AssemblyQualifiedName == cur.AssemblyQualifiedName)
                            {
                                ret = true;
                                break;
                            }
                            toCheck = toCheck.BaseType;
                        }
                    }
                    else
                    {
                        while (toCheck != typeof(object) && toCheck != null)
                        {
                            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                            if (baseType == cur)
                            {
                                ret = true;
                                break;
                            }
                            toCheck = toCheck.BaseType;
                        }
                    }
                }
            }
            else
            {
                ret = (bool)c;
            }

            return ret;
        }


        public static bool AreMatchingTypes(Type typeToCheck, Type baseTypeOrImplementedInterface)
        {
            bool ret = true;

            if (!TypeHelper.IsSubclassOf(typeToCheck, baseTypeOrImplementedInterface) &&
                !TypeHelper.IsImplementingInterface(typeToCheck, baseTypeOrImplementedInterface))
            {
                ret = false;
            }

            return ret;
        }

        public static MethodInfo GetMethodInfo(Type declaringType, string methodName, Type[] argumentTypes)
        {
            if (declaringType == null)
            {
                throw new ArgumentNullException("declaringType");
            }
            if (String.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentNullException("methodName");
            }

            return GetMethodInfoInternal(declaringType, methodName, argumentTypes, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy);
        }

        private static MethodInfo GetMethodInfoInternal(Type declaringType, string methodName, Type[] parameterTypes, BindingFlags bindingFlags)
        {
            MethodInfo ret = null;

            var parameterTypesKeyPart = (parameterTypes == null) ? new string[] { String.Empty } : parameterTypes.Select(p => (p != null) ? p.ToString() : "null").ToArray();
            string cacheKey = String.Concat(declaringType.FullName, "+", methodName, String.Join(":", parameterTypesKeyPart));

            Tuple<bool, MethodInfo> item = null;
            if (item == null)
            {
                foreach (var mi in declaringType.GetMethods(bindingFlags).Where(m => m.Name == methodName))
                {
                    bool isMatchingMethodInfo = true;

                    if (parameterTypes != null)
                    {
                        var declaredParameters = mi.GetParameters();
                        if (declaredParameters.Length == parameterTypes.Length)
                        {
                            for (int i = 0; i < parameterTypes.Length; i++)
                            {
                                var methodParameterType = parameterTypes[i];
                                if (methodParameterType != null)
                                {

                                    var declaredParameterType = declaredParameters[i].ParameterType;
                                    if (declaredParameterType.IsGenericParameter)
                                    {
                                        if ((declaredParameterType.GenericParameterAttributes & GenericParameterAttributes.SpecialConstraintMask) != GenericParameterAttributes.None)
                                        {
                                            // TODO: implement class, new(), etc. constraints
                                            isMatchingMethodInfo = false;
                                        }

                                        foreach (var constraintType in declaredParameterType.GetGenericParameterConstraints())
                                        {
                                            if (!AreMatchingTypes(methodParameterType, constraintType))
                                            {
                                                isMatchingMethodInfo = false;
                                                break;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (declaredParameterType.IsGenericType)
                                        {
                                            declaredParameterType = declaredParameterType.GetGenericTypeDefinition();
                                        }

                                        if (!AreMatchingTypes(methodParameterType, declaredParameterType))
                                        {
                                            isMatchingMethodInfo = false;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            isMatchingMethodInfo = false;
                        }
                    }

                    if (isMatchingMethodInfo)
                    {
                        if (ret == null)
                        {
                            ret = mi;
                        }
                        else
                        {
                            throw new AmbiguousMatchException(String.Format("At least between {0} and {1}", ret, mi)); //LOCSTR
                        }
                    }

                }

                item = new Tuple<bool, MethodInfo>((ret != null), ret);
            }
            else
            {
                ret = item.Item2;
            }

            if (ret == null)
            {
                // fallback to type's interfaces
                foreach (var iface in declaringType.GetInterfaces())
                {
                    ret = GetMethodInfoInternal(iface, methodName, parameterTypes, bindingFlags);
                    if (ret != null)
                    {
                        break;
                    }
                }
            }

            return ret;
        }

    }

    public abstract class DynamicProxy : IDisposable
    {
        private static object dummyOut;
        public static MethodInfo TryInvokeMemberMethodInfo = DynamicProxyTypeBuilder.GetMethodCallExpressionMethodInfo<DynamicProxy>(o => o.TryInvokeMember(null, null, null, out dummyOut));

        public virtual bool TryInvokeMember(Type interfaceType, string name, object[] args, out object result)
        {
            result = null;
            return false;
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DynamicProxy()
        {
            Dispose(false);
        }
    }
}
