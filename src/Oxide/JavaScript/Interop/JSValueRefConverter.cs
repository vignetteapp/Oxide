
using System;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Interop;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript.Interop
{
    public class JSValueRefConverter
    {
        private readonly JSContext context;

        public JSValueRefConverter(JSContext context)
        {
            this.context = context;
        }

        public IntPtr ConvertHostObject(object value)
        {
            switch (value)
            {
                case bool booleanValue:
                    return JSCore.JSValueMakeBoolean(context.Handle, booleanValue);

                case sbyte byteValue:
                    return JSCore.JSValueMakeNumber(context.Handle, byteValue);

                case byte byteValue:
                    return JSCore.JSValueMakeNumber(context.Handle, byteValue);

                case short shortValue:
                    return JSCore.JSValueMakeNumber(context.Handle, shortValue);

                case ushort ushortValue:
                    return JSCore.JSValueMakeNumber(context.Handle, ushortValue);

                case int intValue:
                    return JSCore.JSValueMakeNumber(context.Handle, intValue);

                case uint uintValue:
                    return JSCore.JSValueMakeNumber(context.Handle, uintValue);

                case long longValue:
                    return JSCore.JSValueMakeNumber(context.Handle, longValue);

                case ulong ulongValue:
                    return JSCore.JSValueMakeNumber(context.Handle, ulongValue);

                case double doubleValue:
                    return JSCore.JSValueMakeNumber(context.Handle, doubleValue);

                case string stringValue:
                    return JSCore.JSValueMakeString(context.Handle, stringValue);

                case Undefined:
                    return JSCore.JSValueMakeUndefined(context.Handle);

                case object:
                    {
                        var klass = context.RegisterHostType(value.GetType(), false);
                        var handle = GCHandle.Alloc(value, GCHandleType.Normal);
                        return JSCore.JSObjectMake(context.Handle, klass, GCHandle.ToIntPtr(handle));
                    }

                case null:
                    return JSCore.JSValueMakeNull(context.Handle);
            }
        }

        public object ConvertJSValue(IntPtr value)
        {
            var type = JSCore.JSValueGetType(context.Handle, value);

            switch (type)
            {
                case JSType.Boolean:
                    return JSCore.JSValueToBoolean(context.Handle, value);

                case JSType.Number:
                    return JSCore.JSValueToNumber(context.Handle, value, IntPtr.Zero);

                case JSType.Symbol:
                case JSType.Object:
                    {
                        if (JSCore.JSValueIsArray(context.Handle, value) && JSCore.JSValueGetTypedArrayType(context.Handle, value, IntPtr.Zero) != JSTypedArrayType.None)
                            return new JSTypedArray(context, value);

                        var obj = new JSObject(context, value);

                        if (obj.IsHostObject)
                            return obj.GetHostObject();

                        return obj;
                    }

                case JSType.String:
                    return JSCore.JSValueToString(context.Handle, value, IntPtr.Zero);

                case JSType.Undefined:
                    return Undefined.Value;

                case JSType.Null:
                default:
                    return null;
            }
        }
    }
}

namespace Oxide.JavaScript
{
    public partial class JSCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSType JSValueGetType(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeBoolean(IntPtr ctx, [MarshalAs(UnmanagedType.I1)] bool boolean);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueToBoolean(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNull(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNumber(IntPtr ctx, double number);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern double JSValueToNumber(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeString(IntPtr ctx, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string value);

        [DllImport(LIB_WEBCORE, EntryPoint = "JSValueToStringCopy")]
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))]
        internal static extern string JSValueToString(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeUndefined(IntPtr ctx);
    }
}

