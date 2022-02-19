// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Javascript.Objects;

namespace Oxide.Javascript.Interop
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
                    {
                        var klass = context.RegisterHostType(value.GetType(), null, false);
                        var handle = GCHandle.Alloc(value, GCHandleType.Normal);
                        var pointer = GCHandle.ToIntPtr(handle);
                        return JSCore.JSObjectMake(context.Handle, klass, pointer);
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
                    return JSCore.JSValueToNumber(context.Handle, value, out _);

                case JSType.Symbol:
                case JSType.Object:
                    {
                        if (JSCore.JSValueIsArray(context.Handle, value) && JSCore.JSValueGetTypedArrayType(context.Handle, value, out _) != JSTypedArrayType.None)
                            return new JSTypedArray(context, value);

                        var obj = new JSObject(context, value);

                        if (obj.IsHostObject)
                            return obj.GetHostObject();

                        return obj;
                    }

                case JSType.String:
                    return JSCore.JSValueToString(context.Handle, value, out _);

                case JSType.Undefined:
                    return Undefined.Value;

                case JSType.Null:
                default:
                    return null;
            }
        }
    }
}

