// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Objects;
using UnmanageUtility;

namespace Oxide.JavaScript.Interop
{
    internal class HostObjectProxy : DisposableObject
    {
        internal IntPtr Wrapper
        {
            get
            {
                if (IsDisposed)
                    throw new ObjectDisposedException(nameof(HostObjectProxy));

                return wrapper;
            }
        }

        private readonly IntPtr wrapper;
        private readonly JSContext context;
        private readonly List<TypeCacheEntry> typeCache = new List<TypeCacheEntry>();

        internal HostObjectProxy(JSContext context)
            : base(IntPtr.Zero)
        {
            this.context = context;

            var def = JSClassDefinition.Empty;
            def.Name = @"HostObject";
            def.Version = 1000;

            wrapper = JSCore.JSClassCreate(def);
        }

        internal IntPtr GetJSClassFor(Type type, out IntPtr privateData)
        {
            if (typeCache.Any(k => k.Registry.FullName == type.FullName))
            {
                var entry = typeCache.FirstOrDefault(p => p.Registry.FullName == type.FullName);
                privateData = entry.Registry.Definition.PrivateData;
                return entry.Klass;
            }

            var def = JSClassDefinition.Empty;
            def.Name = type.Name;
            def.Version = 1000;
            def.BaseClass = wrapper;
            def.CallAsConstructor = handleConstructor;
            def.CallAsFunction = handleInstanceFunctionCall;
            def.GetPropertyNames = handlePropertyNames;
            def.DeleteProperty = handleDeleteProperty;
            def.HasProperty = handleHasProperty;
            def.GetProperty = handleInstanceGetProperty;
            def.SetProperty = handleInstanceSetProperty;
            def.Finalize = handleFinalize;

            var registry = new HostTypeRegistry
            {
                Definition = def,
                AssemblyName = type.Assembly.GetName().Name,
                FullName = type.FullName,
                InstanceMembers = type.GetMembers(BindingFlags.Public | BindingFlags.Instance),
                Name = type.Name,
            };

            def.PrivateData = privateData = registry.ToPointer();

            var klass = JSCore.JSClassCreate(def);
            typeCache.Add(new TypeCacheEntry
            {
                Registry = registry,
                Klass = klass,
            });

            return klass;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        internal IntPtr HandleConstructor(IntPtr ctx, IntPtr constructor, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            var prototype = JSCore.JSObjectGetProperty(ctx, constructor, "prototype", out _);
            return createHostObject(ctx, Marshal.PtrToStructure<HostTypeRegistry>(JSCore.JSObjectGetPrivate(prototype)), argumentCount, argumentsHandle, out exception);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr handleConstructor(IntPtr ctx, IntPtr jsClass, IntPtr constructor, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
            => createHostObject(ctx, getUnderlyingType(jsClass), argumentCount, argumentsHandle, out exception);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr createHostObject(IntPtr ctx, HostTypeRegistry registry, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            exception = IntPtr.Zero;

            var converted = Array.Empty<object>();

            if (argumentCount > 0)
            {
                var arguments = new UnmanagedArray<IntPtr>((int)argumentCount);
                arguments.CopyFrom(argumentsHandle, 0, (int)argumentCount);
                converted = arguments.Select(arg => context.Converter.ConvertJSValue(arg)).ToArray();
            }

            try
            {
                var instance = Activator.CreateInstance(
                    registry.AssemblyName,
                    registry.FullName,
                    false,
                    BindingFlags.Default,
                    null,
                    converted,
                    null,
                    null
                );

                var handle = GCHandle.Alloc(instance.Unwrap(), GCHandleType.Normal);
                return JSCore.JSObjectMake(ctx, wrapper, GCHandle.ToIntPtr(handle));
            }
            catch (Exception e)
            {
                exception = makeError(ctx, e.Message);
                return context.Converter.ConvertHostObject(Undefined.Value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool handleHasProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName)
            => int.TryParse(propertyName, out int index) || getUnderlyingType(jsClass).InstanceMembers.Any(m => m.Name == propertyName);

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr handleInstanceGetProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, out IntPtr exception)
        {
            exception = IntPtr.Zero;

            var hostObj = getUnderlyingObject(jsObject);

            if (hostObj is IList list && int.TryParse(propertyName, out int index))
            {
                if (index > -1 && index < list.Count)
                    return context.Converter.ConvertHostObject(list[index]);
            }

            return handleGetProperty(hostObj, getUnderlyingType(jsClass).InstanceMembers, propertyName);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr handleGetProperty(object hostObj, IEnumerable<MemberInfo> members, string propertyName)
        {
            var member = members.FirstOrDefault(m => m.Name == propertyName);

            if (member is FieldInfo field)
                return context.Converter.ConvertHostObject(field.GetValue(hostObj));

            if (member is PropertyInfo property && property.CanRead)
                return context.Converter.ConvertHostObject(property.GetValue(hostObj));

            if (member is MethodInfo method)
            {
                var paramterTypes = method.GetParameters().Select(p => p.ParameterType);
                var returnType = method.ReturnType;

                Type delType = returnType == typeof(void)
                    ? Expression.GetActionType(paramterTypes.ToArray())
                    : Expression.GetFuncType(paramterTypes.Append(returnType).ToArray());

                return context.Converter.ConvertHostObject(method.CreateDelegate(delType, hostObj));
            }

            return context.Converter.ConvertHostObject(Undefined.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool handleInstanceSetProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, IntPtr jsValue, out IntPtr exception)
        {
            exception = IntPtr.Zero;

            var hostObj = getUnderlyingObject(jsObject);
            var hostObjType = hostObj.GetType();
            var value = context.Converter.ConvertJSValue(jsValue);
            var valueType = value.GetType();

            var indexer = hostObjType.GetProperty("Index");
            var indexParams = indexer?.GetIndexParameters();

            if (indexParams?.Length == 1 && indexParams.FirstOrDefault().ParameterType == typeof(int) && int.TryParse(propertyName, out int index))
            {
                var propType = indexer.PropertyType;

                if (!propType.IsAssignableFrom(valueType))
                {
                    exception = makeError(ctx, $"{value} is not assignable to {propType}");
                    return false;
                }

                try
                {
                    indexer.SetValue(hostObj, value, new object[] { index });
                    return true;
                }
                catch (Exception e)
                {
                    exception = makeError(ctx, e.Message);
                    return false;
                }
            }

            return handleSetProperty(ctx, hostObj, getUnderlyingType(jsClass).InstanceMembers, propertyName, value, out exception);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static bool handleSetProperty(IntPtr ctx, object hostObj, IEnumerable<MemberInfo> members, string propertyName, object value, out IntPtr exception)
        {
            exception = IntPtr.Zero;

            var member = members.FirstOrDefault(m => m.Name == propertyName);
            var valueType = value.GetType();

            if (member is FieldInfo field && !field.IsInitOnly)
            {
                if (!field.FieldType.IsAssignableFrom(valueType))
                {
                    exception = makeError(ctx, @$"Cannot assign {field.FieldType} to {valueType}.");
                    return false;
                }

                try
                {
                    field.SetValue(hostObj, value);
                    return true;
                }
                catch (Exception e)
                {
                    exception = makeError(ctx, e.Message);
                    return false;
                }
            }

            if (member is PropertyInfo property && property.CanWrite)
            {
                if (!property.PropertyType.IsAssignableFrom(valueType))
                {
                    exception = makeError(ctx, @$"Cannot assign {property.PropertyType} to {valueType}.");
                    return false;
                }

                try
                {
                    property.SetValue(hostObj, value);
                    return true;
                }
                catch (Exception e)
                {
                    exception = makeError(ctx, e.Message);
                    return false;
                }
            }

            exception = makeError(ctx, "Failed to set value.");
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private bool handleDeleteProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, out IntPtr exception)
        {
            exception = makeError(ctx, @"Cannot delete properties of this object.");
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void handlePropertyNames(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, IntPtr propertyNames)
        {
            foreach (var member in getUnderlyingType(jsClass).InstanceMembers)
                JSCore.JSPropertyNameAccumulatorAddName(propertyNames, member.Name);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr handleInstanceFunctionCall(IntPtr ctx, IntPtr jsClass, string className, IntPtr function, IntPtr thisObject, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            var hostObj = getUnderlyingObject(function);
            var target = getUnderlyingObject(thisObject);
            return handleFunctionCall(ctx, hostObj, target, argumentCount, argumentsHandle, out exception);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private IntPtr handleFunctionCall(IntPtr ctx, object hostObj, object target, uint argumentCount, IntPtr argumentsHandle, out IntPtr exception)
        {
            if (hostObj is not Delegate del)
            {
                exception = makeError(ctx, @"Object is not a callable function");
                return context.Converter.ConvertHostObject(Undefined.Value);
            }

            var converted = Array.Empty<object>();

            if (argumentCount > 0)
            {
                var arguments = new UnmanagedArray<IntPtr>((int)argumentCount);
                arguments.CopyFrom(argumentsHandle, 0, (int)argumentCount);
                converted = arguments.Select(arg => context.Converter.ConvertJSValue(arg)).ToArray();
            }

            if (!Enumerable.SequenceEqual(converted.Select(a => a.GetType()), del.Method.GetParameters().Select(p => p.ParameterType)))
            {
                exception = makeError(ctx, @"Invalid parameters were passed to the method.");
                return context.Converter.ConvertHostObject(Undefined.Value);
            }

            try
            {
                exception = IntPtr.Zero;
                return context.Converter.ConvertHostObject(del.Method.Invoke(target, converted));
            }
            catch (Exception e)
            {
                exception = makeError(ctx, e.Message);
                return context.Converter.ConvertHostObject(Undefined.Value);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void handleFinalize(IntPtr jsClass, IntPtr jsObject)
        {
            var handle = GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(jsObject));
            handle.Free();
        }

        private static IntPtr makeError(IntPtr ctx, string message)
        {
            var arguments = new[]
            {
                JSCore.JSValueMakeString(ctx, message),
            };

            var pointer = Marshal.AllocHGlobal(Marshal.SizeOf<IntPtr>() * arguments.Length);
            Marshal.Copy(arguments, 0, pointer, arguments.Length);

            var error = JSCore.JSObjectMakeError(ctx, (uint)arguments.Length, pointer, out _);
            Marshal.FreeHGlobal(pointer);

            return error;
        }

        private static object getUnderlyingObject(IntPtr jsObject)
        {
            var data = JSCore.JSObjectGetPrivate(jsObject);

            if (data == IntPtr.Zero)
                return null;

            return GCHandle.FromIntPtr(data).Target;
        }

        private static HostTypeRegistry getUnderlyingType(IntPtr jsClass)
            => Marshal.PtrToStructure<HostTypeRegistry>(JSCore.JSClassGetPrivate(jsClass));

        protected override void DisposeUnmanaged()
        {
            foreach (var entry in typeCache)
            {
                entry.Registry.InstanceMembers = null;

                var data = JSCore.JSClassGetPrivate(entry.Klass);
                Marshal.FreeHGlobal(data);
                JSCore.JSClassRelease(entry.Klass);
            }

            typeCache.Clear();

            JSCore.JSClassRelease(wrapper);
        }

        private struct TypeCacheEntry
        {
            public HostTypeRegistry Registry;
            public IntPtr Klass;
        }
    }
}
