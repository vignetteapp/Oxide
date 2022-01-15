using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Interop;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript.Objects
{
    public class JSObject : DynamicObject
    {
        internal readonly IntPtr Handle;

        /// <summary>
        /// Gets whether this JavaScript object is a wrapped .NET Object.
        /// </summary>
        public bool IsHostObject => JSCore.JSValueIsObjectOfClass(Context.Handle, Handle, Context.Proxy.Wrapper);

        /// <summary>
        /// Gets whether this JavaScript object is an array.
        /// </summary>
        public bool IsArray => JSCore.JSValueIsArray(Context.Handle, Handle);

        /// <summary>
        /// Gets whther this JavaScript object is a function.
        /// </summary>
        public bool IsFunction => JSCore.JSObjectIsFunction(Context.Handle, Handle);

        /// <summary>
        /// Gets whether this JavaScript object is a promise.
        /// </summary>
        public bool IsPromise => IsFunction && JSCore.JSValueIsUndefined(Context.Handle, JSCore.JSObjectGetProperty(Context.Handle, Handle, "then", IntPtr.Zero));

        /// <summary>
        /// Gets whether this JavaScript object is a constructor.
        /// </summary>
        public bool IsConstructor => JSCore.JSObjectIsConstructor(Context.Handle, Handle);

        /// <summary>
        /// Gets this JavaScript object's array type.
        /// </summary>
        public JSTypedArrayType ArrayType => JSCore.JSValueGetTypedArrayType(Context.Handle, Handle, IntPtr.Zero);

        protected readonly JSContext Context;

        internal JSObject(JSContext ctx, IntPtr handle)
        {
            Context = ctx;
            Handle = handle;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var members = new List<string>();
            var accumulator = JSCore.JSObjectCopyPropertyNames(Context.Handle, Handle);
            uint length = JSCore.JSPropertyNameArrayGetCount(accumulator);

            for (uint i = 0; i < length; i++)
            {
                string item = JSCore.JSPropertyNameArrayGetNameAtIndex(accumulator, i);
                members.Add(item);
            }

            return members;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            result = Undefined.Value;

            if (indexes.Length > 1 && !IsArray)
                return false;

            var value = JSCore.JSObjectGetPropertyAtIndex(Context.Handle, Handle, (uint)indexes[0], IntPtr.Zero);
            result = Context.Converter.ConvertJSValue(value);

            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var value = JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, IntPtr.Zero);
            result = Context.Converter.ConvertJSValue(value);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length > 1 && !IsArray)
                return false;

            JSCore.JSObjectSetPropertyAtIndex(Context.Handle, Handle, (uint)indexes[0], Context.Converter.ConvertHostObject(value), IntPtr.Zero);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
            => JSCore.JSObjectSetProperty(Context.Handle, Handle, binder.Name, Context.Converter.ConvertHostObject(value));

        public override bool TryDeleteMember(DeleteMemberBinder binder)
            => JSCore.JSObjectDeleteProperty(Context.Handle, Handle, binder.Name, IntPtr.Zero);

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            var convertedArgs = args.Select(v => Context.Converter.ConvertHostObject(v)).ToArray();
            var convertedArgsHandle = GCHandle.Alloc(convertedArgs, GCHandleType.Normal);

            result = Context.Converter.ConvertJSValue(
                JSCore.JSObjectCallAsFunction(
                    Context.Handle, Handle, Handle, (uint)args.Length, GCHandle.ToIntPtr(convertedArgsHandle), IntPtr.Zero
                )
            );

            convertedArgsHandle.Free();
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var convertedArgs = args.Select(v => Context.Converter.ConvertHostObject(v)).ToArray();
            var convertedArgsHandle = GCHandle.Alloc(convertedArgs, GCHandleType.Normal);

            var function = JSCore.JSObjectGetProperty(Context.Handle, Handle, binder.Name, IntPtr.Zero);

            result = Context.Converter.ConvertJSValue(
                JSCore.JSObjectCallAsFunction(
                    Context.Handle, function, Handle, (uint)args.Length, GCHandle.ToIntPtr(convertedArgsHandle), IntPtr.Zero
                )
            );

            convertedArgsHandle.Free();
            return true;
        }

        public object GetHostObject()
        {
            if (!IsHostObject)
                return null;

            return GCHandle.FromIntPtr(JSCore.JSObjectGetPrivate(Handle)).Target;
        }
    }
}

namespace Oxide.JavaScript
{
    public partial class JSCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsArray(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsUndefined(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsObjectOfClass(IntPtr ctx, IntPtr value, IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSTypedArrayType JSValueGetTypedArrayType(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassCreate(JSClassDefinition definition);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassRetain(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassRelease(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSClassGetPrivate(IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSClassSetPrivate(IntPtr jsClass, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMake(IntPtr ctx, IntPtr jsClass, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeFunctionWithCallback(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string name, JSObjectCallAsFunctionCallback callAsFunction);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeConstructor(IntPtr ctx, IntPtr jsClass, JSObjectCallAsConstructorCallback callAsConstructor);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeArray(IntPtr ctx, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeDate(IntPtr ctx, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeError(IntPtr ctx, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeRegExp(IntPtr ctx, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeDeferredPromise(IntPtr ctx, IntPtr resolve, IntPtr reject, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectMakeFunction(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string name, uint parameterCount, IntPtr parameterNames, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string body, IntPtr sourceURL, int startingLineNumber, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrototype(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPrototype(IntPtr ctx, IntPtr obj, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectHasProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeleteProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectHasPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, IntPtr value, JSPropertyAttribute attributes, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeletePropertyForKey(IntPtr ctx, IntPtr obj, IntPtr propertyKey, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIndex, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIndex, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivate(IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivate(IntPtr obj, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectIsFunction(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCallAsFunction(IntPtr ctx, IntPtr obj, IntPtr thisObject, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectIsConstructor(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCallAsConstructor(IntPtr ctx, IntPtr obj, uint argumentCount, IntPtr arguments, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectCopyPropertyNames(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSPropertyNameArrayRetain(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSPropertyNameArrayRelease(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSPropertyNameArrayGetCount(IntPtr array);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))]
        internal static extern string JSPropertyNameArrayGetNameAtIndex(IntPtr array, uint index);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSPropertyNameAccumulatorAddName(IntPtr accumulator, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivateProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivateProperty(IntPtr ctx, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeletePrivateProperty(IntPtr ctx, IntPtr obj, IntPtr proeprtyName);
    }
}
