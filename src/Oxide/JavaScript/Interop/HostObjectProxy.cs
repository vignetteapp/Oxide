// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Oxide.Javascript.Interop
{
    internal unsafe static class HostObjectProxy
    {
        public static readonly IntPtr Handle;
        private static readonly JSClassDefinition def;

        static HostObjectProxy()
        {
            def = new JSClassDefinition
            {
                Name = @"HostObject",
                Version = 1000,
                Attributes = JSClassAttributes.NoAutomaticPrototype,
                CallAsConstructor = callAsConstructor,
                CallAsFunction = callAsFunction,
                GetPropertyNames = getPropertyNames,
                DeleteProperty = deleteProperty,
                HasProperty = hasProperty,
                GetProperty = getProperty,
                SetProperty = setProperty,
                Finalize = finalize
            };

            Handle = JSCore.JSClassCreate(def);
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
        private static IntPtr getProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, out IntPtr exception)
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
                        return JSValue.From(ctx, indexer.GetValue(hostObject.Target, new[] { index }));
                }

                var member = hostObject.Type.Members.FirstOrDefault(m => m.Name == propName);

                if (member is FieldInfo field)
                    return JSValue.From(ctx, field.GetValue(hostObject.Target));

                if (member is PropertyInfo prop && prop.CanRead)
                    return JSValue.From(ctx, prop.GetValue(hostObject.Target));

                if (member is MethodInfo method)
                {
                    var hostMethod = hostObject.Type.Methods.FirstOrDefault(m => m.Method == method);
                    if (hostMethod != null)
                        return JSValue.From(ctx, hostMethod);
                }
            }
            catch (Exception e)
            {
                exception = JSValue.From(ctx, e);
            }

            return JSValue.From(ctx, Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool setProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, IntPtr jsValue, out IntPtr exception)
        {
            var hostObject = unwrap(jsObject);

            try
            {
                exception = IntPtr.Zero;

                var value = new JSValue(ctx, jsValue).GetValue();

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
                exception = JSValue.From(ctx, e);
            }

            exception = JSValue.From(ctx, new InvalidOperationException($@"Failed to set property ""{propName}"" on {hostObject.Type.Name}"));
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool deleteProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propName, out IntPtr exception)
        {
            exception = JSValue.From(ctx, new InvalidOperationException($"Cannot delete properties of {nameof(HostObjectProxy)}"));
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr callAsConstructor(IntPtr ctx, IntPtr jsClass, IntPtr jsConstructor, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            var hostObject = unwrap(jsConstructor);

            if (hostObject?.Target is Type type)
            {
                exception = IntPtr.Zero;

                try
                {
                    var arr = new Span<IntPtr>((void*)argumentsHandle, (int)argumentCount).ToArray()
                        .Select(arg => new JSValue(ctx, arg).GetValue()).ToArray();

                    return arr.Length > 0
                        ? JSValue.From(ctx, Activator.CreateInstance(type, arr))
                        : JSValue.From(ctx, Activator.CreateInstance(type));
                }
                catch (Exception e)
                {
                    exception = JSValue.From(ctx, e);
                }
            }

            exception = JSValue.From(ctx, new InvalidOperationException($"{hostObject.Type.Name} is not a constructor."));
            return JSValue.From(ctx, Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static IntPtr callAsFunction(IntPtr ctx, IntPtr jsClass, string className, IntPtr jsFunction, IntPtr jsThisObject, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            try
            {
                var hostMethod = unwrap(jsFunction);
                var hostTarget = unwrap(jsThisObject);

                exception = IntPtr.Zero;

                if (hostMethod.Target is HostMethod method)
                {
                    var arr = new Span<IntPtr>((void*)argumentsHandle, (int)argumentCount);
                    var result = method.Invoke(hostTarget.Target, arr.ToArray().Select(arg => new JSValue(ctx, arg).GetValue()).ToArray());

                    if (method.Method is MethodInfo mi && mi.ReturnType != typeof(void))
                        return JSValue.From(ctx, result);
                }
            }
            catch (Exception e)
            {
                exception = JSValue.From(ctx, e);
            }

            return JSValue.From(ctx, Undefined.Value);
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
    }
}
