// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;
using Oxide.Javascript.Interop;

namespace Oxide.Javascript
{
    internal struct JSValue
    {
        public readonly JSType Type;
        public readonly JSContext Context;
        private readonly IntPtr handle;

        public JSValue(IntPtr ctx, IntPtr handle)
        {
            this.handle = handle;
            Type = JSCore.JSValueGetType(ctx, handle);
            Context = JSContext.GetContext(ctx) ?? new JSContext(ctx, false);
        }

        private JSValue(IntPtr ctx)
        {
            Type = JSType.Undefined;
            Context = JSContext.GetContext(ctx) ?? new JSContext(ctx, false);
            handle = JSCore.JSValueMakeUndefined(ctx);
        }

        public object GetValue()
        {
            IntPtr error = IntPtr.Zero;
            object result = null;

            switch (Type)
            {
                case JSType.Object:
                    {
                        var jsObject = new JSObject(this);

                        if (jsObject?.IsHostObject ?? false)
                            return jsObject.GetHostObject();

                        return jsObject;
                    }

                case JSType.Boolean:
                    result = JSCore.JSValueToBoolean(Context.Handle, handle);
                    break;

                case JSType.Number:
                    result = JSCore.JSValueToNumber(Context.Handle, handle, out error);
                    break;

                case JSType.String:
                    result = JSCore.JSValueToString(Context.Handle, handle, out error);
                    break;

                case JSType.Undefined:
                    result = Undefined.Value;
                    break;

                case JSType.Null:
                default:
                    break;
            }

            if (error != IntPtr.Zero)
                throw new JavascriptException(@"An exception has occured while attempting to convert a value.", new JSValue(Context.Handle, error));

            return result;
        }

        public static JSValue From(IntPtr ctx, object value)
        {
            IntPtr ptr = IntPtr.Zero;

            switch (value)
            {
                case JSObject jsObject:
                    return jsObject.Value;

                case bool booleanValue:
                    ptr = JSCore.JSValueMakeBoolean(ctx, booleanValue);
                    break;

                case char charValue:
                    ptr = JSCore.JSValueMakeString(ctx, charValue.ToString());
                    break;

                case sbyte byteValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, byteValue);
                    break;

                case byte byteValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, byteValue);
                    break;

                case short shortValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, shortValue);
                    break;

                case ushort ushortValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, ushortValue);
                    break;

                case int intValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, intValue);
                    break;

                case uint uintValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, uintValue);
                    break;

                case long longValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, longValue);
                    break;

                case ulong ulongValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, ulongValue);
                    break;

                case double doubleValue:
                    ptr = JSCore.JSValueMakeNumber(ctx, doubleValue);
                    break;

                case string stringValue:
                    ptr = JSCore.JSValueMakeString(ctx, stringValue);
                    break;

                case Undefined:
                    return new JSValue(ctx);

                case object:
                    return new JSValue(ctx, JSCore.JSObjectMake(ctx, HostObjectProxy.Handle, GCHandle.ToIntPtr(GCHandle.Alloc(new HostObject(value), GCHandleType.Normal))));
            }

            return new JSValue(ctx, ptr);
        }

        public static implicit operator IntPtr(JSValue value) => value.handle;
    }
}
