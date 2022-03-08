// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnmanageUtility;

namespace Oxide.Javascript.Interop
{
    internal sealed class HostObjectProxy : IDisposable
    {
        public readonly IntPtr Handle;
        private readonly JSClassDefinition def;
        private readonly JSContext context;
        private bool isDisposed;

        public HostObjectProxy(JSContext context)
        {
            def = JSClassDefinition.Empty;
            def.Name = @"HostObject";
            def.Version = 1000;
            def.CallAsConstructor = callAsConstructor;
            def.CallAsFunction = callAsFunction;
            def.GetPropertyNames = getPropertyNames;
            def.DeleteProperty = deleteProperty;
            def.HasProperty = hasProperty;
            def.GetProperty = getProperty;
            def.SetProperty = setProperty;
            def.Finalize = finalize;

            Handle = JSCore.JSClassCreate(def);
            this.context = context;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void getPropertyNames(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, IntPtr propertyNames)
        {
            var hostObject = unwrap(jsObject);

            if (hostObject == null)
                return;

            foreach (string name in hostObject.Type.Members.Select(m => m.Name))
                JSCore.JSPropertyNameAccumulatorAddName(propertyNames, name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool hasProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName) => true;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr getProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, out IntPtr exception)
        {
            try
            {
                exception = IntPtr.Zero;

                var hostObject = unwrap(jsObject);

                var indexer = hostObject.Type.Members.OfType<PropertyInfo>().FirstOrDefault(p => p.GetIndexParameters().Length == 1);

                if (indexer != null)
                {
                    var param = indexer.GetIndexParameters().FirstOrDefault();
                    if (tryChangeType(propName, param.ParameterType, out var index))
                        return context.Converter.ConvertHostObject(indexer.GetValue(hostObject.Target, new[] { index }));
                }

                var member = hostObject.Type.Members.FirstOrDefault(m => m.Name == propName);

                if (member is FieldInfo field)
                    return context.Converter.ConvertHostObject(field.GetValue(hostObject.Target));

                if (member is PropertyInfo prop && prop.CanRead)
                    return context.Converter.ConvertHostObject(prop.GetValue(hostObject.Target));

                if (member is MethodInfo method)
                {
                    var hostMethod = hostObject.Type.Methods.FirstOrDefault(m => m.Method == method);
                    if (hostMethod != null)
                        return context.Converter.ConvertHostObject(hostMethod);
                }
            }
            catch (Exception e)
            {
                exception = context.Converter.ConvertHostObject(e);
            }

            return context.Converter.ConvertHostObject(Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool setProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, IntPtr jsValue, out IntPtr exception)
        {
            var hostObject = unwrap(jsObject);

            try
            {
                exception = IntPtr.Zero;

                var value = context.Converter.ConvertJSValue(jsValue);

                var indexer = hostObject.Type.Members.OfType<PropertyInfo>().FirstOrDefault(p => p.GetIndexParameters().Length == 1);

                if (indexer != null)
                {
                    var param = indexer.GetIndexParameters().FirstOrDefault();

                    if (tryChangeType(propName, param.ParameterType, out var index))
                        indexer.SetValue(hostObject.Target, value, new object[] { index });

                    return true;
                }

                var member = hostObject.Type.Members.FirstOrDefault(m => m is not MethodInfo && m.Name == propName);

                if (member is FieldInfo field && !field.IsInitOnly)
                {
                    if (tryChangeType(value, field.FieldType, out var result))
                        field.SetValue(hostObject.Target, result);

                    return true;
                }

                if (member is PropertyInfo prop && prop.CanWrite)
                {
                    if (tryChangeType(value, prop.PropertyType, out var result))
                        prop.SetValue(hostObject.Target, result);

                    return true;
                }
            }
            catch (Exception e)
            {
                exception = context.Converter.ConvertHostObject(e);
            }

            exception = context.Converter.ConvertHostObject(new InvalidOperationException($@"Failed to set property ""{propName}"" on {hostObject.Type.Name}"));
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool deleteProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, out IntPtr exception)
        {
            exception = context.Converter.ConvertHostObject(new InvalidOperationException($"Cannot delete properties of {nameof(HostObjectProxy)}"));
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr callAsConstructor(IntPtr ctx, IntPtr jsClass, IntPtr jsConstructor, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            var hostObject = unwrap(jsConstructor);

            if (hostObject?.Target is Type type)
            {
                exception = IntPtr.Zero;
                return context.Converter.ConvertHostObject(Activator.CreateInstance(type, convertArgsFromHandle(argumentCount, argumentsHandle)));
            }

            exception = context.Converter.ConvertHostObject(new InvalidOperationException($"{hostObject.Type.Name} is not a constructor."));
            return context.Converter.ConvertHostObject(Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr callAsFunction(IntPtr ctx, IntPtr jsClass, string className, IntPtr jsFunction, IntPtr jsThisObject, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            try
            {
                var hostMethod = unwrap(jsFunction);
                var hostTarget = unwrap(jsThisObject);

                exception = IntPtr.Zero;

                if (hostMethod.Target is HostMethod method)
                {
                    var result = method.Invoke(hostTarget.Target, convertArgsFromHandle(argumentCount, argumentsHandle));
                    if (method.Method is MethodInfo mi && mi.ReturnType != typeof(void))
                        return context.Converter.ConvertHostObject(result);
                }
            }
            catch (Exception e)
            {
                exception = context.Converter.ConvertHostObject(e);
            }

            return context.Converter.ConvertHostObject(Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void finalize(IntPtr jsClass, IntPtr jsObject)
            => GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(jsObject)).Free();

        private static HostObject unwrap(IntPtr jsObject)
        {
            var data = JSCore.JSObjectGetPrivate(jsObject);

            if (data == IntPtr.Zero)
                return null;

            return GCHandle.FromIntPtr(data).Target as HostObject;
        }

        private object[] convertArgsFromHandle(uint argumentCount, IntPtr argumentsHandle)
        {
            object[] convertedArgs = Array.Empty<object>();

            if (argumentCount > 0)
            {
                var args = new UnmanagedArray<IntPtr>((int)argumentCount);
                args.CopyFrom(argumentsHandle, 0, (int)argumentCount);
                convertedArgs = args.Select(arg => context.Converter.ConvertJSValue(arg)).ToArray();
            }

            return convertedArgs;
        }

        private IntPtr convertArgsToHandle(params object[] args)
        {
            if (args.Length == 0)
                return IntPtr.Zero;

            var argsSpan = new Span<IntPtr>(args.Select(a => context.Converter.ConvertHostObject(a)).ToArray());
            var argsHandle = new UnmanagedArray<IntPtr>(argsSpan);
            return argsHandle.Ptr;
        }

        private static bool tryChangeType(object value, Type type, out object result)
        {
            try
            {
                result = Convert.ChangeType(value, type);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
        }

        internal void Dispose(bool _)
        {
            if (isDisposed)
                return;

            JSCore.JSClassRelease(Handle);

            isDisposed = true;
        }

        ~HostObjectProxy()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
