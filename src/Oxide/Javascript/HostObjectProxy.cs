using System;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript
{
    public class HostObjectProxy : IDisposable
    {
        internal readonly IntPtr Wrapper;
        private readonly JSContext context;
        private bool isDisposed;

        internal HostObjectProxy(JSContext context)
        {
            this.context = context;

            var def = JSClassDefinition.Empty;
            def.Name = @"HostObject";
            def.Version = 1000;
            def.CallAsConstructor = HandleConstructor;
            def.CallAsFunction = handleFunctionCall;
            def.GetPropertyNames = handlePropertyNames;
            def.DeleteProperty = handleDeleteProperty;
            def.HasProperty = handleHasProperty;
            def.GetProperty = handleGetProperty;
            def.SetProperty = handleSetProperty;
            def.Finalize = handleFinalize;

            Wrapper = JSCore.JSClassCreate(def);
        }

        internal IntPtr MakeDerivedClass(TypeRegistry registry)
        {
            var def = JSClassDefinition.Empty;
            def.Name = registry.FullName;
            def.Version = 1000;
            def.BaseClass = Wrapper;
            def.PrivateData = GCHandle.ToIntPtr(GCHandle.Alloc(registry, GCHandleType.Pinned));
            return JSCore.JSClassCreate(def);
        }

        internal IntPtr HandleConstructor(IntPtr ctx, IntPtr jsClass, IntPtr constructor, uint argumentCount, IntPtr argumentsHandle, IntPtr exception)
        {
            IntPtr[] arguments = new IntPtr[(int)argumentCount];
            Marshal.Copy(argumentsHandle, arguments, 0, (int)argumentCount);

            var typeInfo = Marshal.PtrToStructure<TypeRegistry>(JSCore.JSClassGetPrivate(jsClass));
            var instance = Activator.CreateInstance(
                typeInfo.AssemblyName,
                typeInfo.FullName,
                false,
                BindingFlags.Default,
                null,
                arguments.Select(s => context.Converter.ConvertJSValue(s)).ToArray(),
                null,
                null
            );

            var handle = GCHandle.Alloc(instance.Unwrap(), GCHandleType.Normal);
            return JSCore.JSObjectMake(ctx, Wrapper, GCHandle.ToIntPtr(handle));
        }

        private bool handleHasProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName)
        {
            var hostObj = getUnderlyingObject(jsObject);
            return hostObj.GetType().GetMember(propertyName, BindingFlags.Public | BindingFlags.Instance).Length > 0;
        }

        private IntPtr handleGetProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, IntPtr exception)
        {
            var hostObj = getUnderlyingObject(jsObject);
            var member = hostObj.GetType().GetMember(propertyName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (member is FieldInfo field)
                return context.Converter.ConvertHostObject(field.GetValue(hostObj));

            if (member is PropertyInfo property && property.CanRead)
                return context.Converter.ConvertHostObject(property.GetValue(hostObj));

            if (member is MethodInfo method)
                return context.Converter.ConvertHostObject(method);

            return context.Converter.ConvertHostObject(Undefined.Value);
        }

        private bool handleSetProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, IntPtr jsValue, IntPtr exception)
        {
            var hostObj = getUnderlyingObject(jsObject);
            var member = hostObj.GetType().GetMember(propertyName, BindingFlags.Public | BindingFlags.Instance).FirstOrDefault();

            if (member is FieldInfo field && !field.IsInitOnly)
            {
                var value = context.Converter.ConvertJSValue(jsValue);

                if (!field.FieldType.IsAssignableFrom(value.GetType()))
                    return false;

                field.SetValue(hostObj, value);
            }

            if (member is PropertyInfo property && property.CanWrite)
            {
                var value = context.Converter.ConvertJSValue(jsValue);

                if (!property.PropertyType.IsAssignableFrom(value.GetType()))
                    return false;

                property.SetValue(hostObj, value);
            }

            return false;
        }

        private bool handleDeleteProperty(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, string propertyName, IntPtr exception)
        {
            return false;
        }

        private void handlePropertyNames(IntPtr ctx, IntPtr jsClass, IntPtr jsObject, IntPtr propertyNames)
        {
            var hostObj = getUnderlyingObject(jsObject);
            var members = hostObj.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance);

            foreach (var member in members)
                JSCore.JSPropertyNameAccumulatorAddName(propertyNames, member.Name);
        }

        private IntPtr handleFunctionCall(IntPtr ctx, IntPtr jsClass, string className, IntPtr function, IntPtr thisObject, uint argumentCount, IntPtr argumentsHandle, IntPtr exception)
        {
            var hostObj = getUnderlyingObject(function);

            if (hostObj is not Delegate del)
                return context.Converter.ConvertHostObject(Undefined.Value);

            IntPtr[] arguments = new IntPtr[(int)argumentCount];
            Marshal.Copy(argumentsHandle, arguments, 0, (int)argumentCount);

            var convertedArgs = arguments.Select(a => context.Converter.ConvertJSValue(a));

            if (!Enumerable.SequenceEqual(convertedArgs.Select(a => a.GetType()), del.Method.GetParameters().Select(p => p.ParameterType)))
                return context.Converter.ConvertHostObject(Undefined.Value);

            return context.Converter.ConvertHostObject(del.Method.Invoke(del.Target, convertedArgs.ToArray()));
        }

        private void handleFinalize(IntPtr jsClass, IntPtr jsObject)
        {
            var handle = GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(jsObject));
            handle.Free();
        }

        private static object getUnderlyingObject(IntPtr jsObject)
            => GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(jsObject)).Target;

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;

            JSCore.JSClassRelease(Wrapper);

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
