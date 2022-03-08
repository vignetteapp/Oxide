// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Javascript.Objects;

namespace Oxide.Javascript.Interop
{
    internal static class JSValueRefConverter
    {
        public static IntPtr ConvertHostObject(IntPtr ctx, object value)
        {
            switch (value)
            {
                case JSObject jsObject:
                    return jsObject.Handle;

                case bool booleanValue:
                    return JSCore.JSValueMakeBoolean(ctx, booleanValue);

                case sbyte byteValue:
                    return JSCore.JSValueMakeNumber(ctx, byteValue);

                case byte byteValue:
                    return JSCore.JSValueMakeNumber(ctx, byteValue);

                case short shortValue:
                    return JSCore.JSValueMakeNumber(ctx, shortValue);

                case ushort ushortValue:
                    return JSCore.JSValueMakeNumber(ctx, ushortValue);

                case int intValue:
                    return JSCore.JSValueMakeNumber(ctx, intValue);

                case uint uintValue:
                    return JSCore.JSValueMakeNumber(ctx, uintValue);

                case long longValue:
                    return JSCore.JSValueMakeNumber(ctx, longValue);

                case ulong ulongValue:
                    return JSCore.JSValueMakeNumber(ctx, ulongValue);

                case double doubleValue:
                    return JSCore.JSValueMakeNumber(ctx, doubleValue);

                case string stringValue:
                    return JSCore.JSValueMakeString(ctx, stringValue);

                case Undefined:
                    return JSCore.JSValueMakeUndefined(ctx);

                case object:
                    return JSCore.JSObjectMake(ctx, HostObjectProxy.Handle, GCHandle.ToIntPtr(GCHandle.Alloc(new HostObject(value), GCHandleType.Normal)));

                case null:
                    return JSCore.JSValueMakeNull(ctx);
            }
        }

        public static object ConvertJSValue(IntPtr ctx, IntPtr value)
        {
            var type = JSCore.JSValueGetType(ctx, value);

            IntPtr error = IntPtr.Zero;
            object result = null;

            switch (type)
            {
                case JSType.Boolean:
                    result = JSCore.JSValueToBoolean(ctx, value);
                    break;

                case JSType.Number:
                    result = JSCore.JSValueToNumber(ctx, value, out error);
                    break;

                case JSType.String:
                    result = JSCore.JSValueToString(ctx, value, out error);
                    break;

                case JSType.Undefined:
                    result = Undefined.Value;
                    break;

                case JSType.Symbol:
                case JSType.Object:
                    {
                        if (JSCore.JSValueIsArray(ctx, value))
                        {
                            result = new JSTypedArray(ctx, value);
                            break;
                        }

                        result = new JSObject(ctx, value);

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
                throw JavascriptException.GetException(ctx, error);

            return result;
        }
    }
}
