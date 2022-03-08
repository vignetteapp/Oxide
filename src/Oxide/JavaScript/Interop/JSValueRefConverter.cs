// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Javascript.Objects;

namespace Oxide.Javascript.Interop
{
    internal class JSValueRefConverter
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
                case JSObject jsObject:
                    return jsObject.Handle;

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
                    return JSCore.JSObjectMake(context.Handle, context.Proxy.Handle, GCHandle.ToIntPtr(GCHandle.Alloc(new HostObject(value), GCHandleType.Normal)));

                case null:
                    return JSCore.JSValueMakeNull(context.Handle);
            }
        }

        public object ConvertJSValue(IntPtr value)
        {
            var type = JSCore.JSValueGetType(context.Handle, value);

            IntPtr error = IntPtr.Zero;
            object result = null;

            switch (type)
            {
                case JSType.Boolean:
                    result = JSCore.JSValueToBoolean(context.Handle, value);
                    break;

                case JSType.Number:
                    result = JSCore.JSValueToNumber(context.Handle, value, out error);
                    break;

                case JSType.String:
                    result = JSCore.JSValueToString(context.Handle, value, out error);
                    break;

                case JSType.Undefined:
                    result = Undefined.Value;
                    break;

                case JSType.Symbol:
                case JSType.Object:
                    {
                        if (JSCore.JSValueIsArray(context.Handle, value))
                        {
                            result = new JSTypedArray(context, value);
                            break;
                        }

                        result = new JSObject(context, value);

                        if (((JSObject)result).IsHostObject)
                        {
                            result = ((JSObject)result).GetHostObject();
                            break;
                        }

                        break;
                    }

                case JSType.Null:
                default:
                    break;
            }

            if (error != IntPtr.Zero)
                throw JavascriptException.Throw(context, error);

            return result;
        }
    }
}
