// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Runtime.InteropServices;

namespace Oxide.Javascript.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct JSClassDefinition
    {
        internal int Version;
        internal JSClassAttributes Attributes;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal IntPtr BaseClass;
        internal IntPtr StaticValues;
        internal IntPtr StaticFunctions;
        internal JSObjectInitializeCallbackEx Initialize;
        internal JSObjectFinalizeCallbackEx Finalize;
        internal JSObjectHasPropertyCallback HasProperty;
        internal JSObjectGetPropertyCallbackEx GetProperty;
        internal JSObjectSetPropertyCallback SetProperty;
        internal JSObjectDeletePropertyCallback DeleteProperty;
        internal JSObjectGetPropertyNamesCallbackEx GetPropertyNames;
        internal JSObjectCallAsFunctionCallbackEx CallAsFunction;
        internal JSObjectCallAsConstructorCallbackEx CallAsConstructor;
        internal JSObjectHasInstanceCallback HasInstance;
        internal JSObjectConvertToTypeCallbackEx ConvertToType;
        internal IntPtr PrivateData;
    }

    [Flags]
    internal enum JSPropertyAttribute : uint
    {
        None = 0,
        ReadOnly = 1 << 1,
        DontEnum = 1 << 2,
        DontDelete = 1 << 3,
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct JSStaticFunction
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;
        internal JSObjectCallAsFunctionCallbackEx Call;
        internal JSPropertyAttribute Attributes;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct JSStaticValue
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;
        internal JSObjectGetPropertyCallbackEx GetProperty;
        internal JSObjectSetPropertyCallback SetProperty;
        internal JSPropertyAttribute Attributes;
    }

    [Flags]
    internal enum JSClassAttributes
    {
        None = 0,
        NoAutomaticPrototype = 1 << 1,
    }

    internal delegate void JSObjectInitializeCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj);
    internal delegate void JSObjectFinalizeCallbackEx(IntPtr jsClass, IntPtr obj);
    internal delegate void JSObjectGetPropertyNamesCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyNames);
    internal delegate IntPtr JSObjectCallAsFunctionCallbackEx(IntPtr ctx, IntPtr jsClass, string className, IntPtr func, IntPtr thisObj, uint argumentCount, IntPtr arguments, out IntPtr exception);
    internal delegate IntPtr JSObjectCallAsConstructorCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr constructor, uint argumentCount, IntPtr arguments, out IntPtr exception);
    internal delegate IntPtr JSObjectConvertToTypeCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, JSType type, out IntPtr exception);
    internal delegate IntPtr JSObjectGetPropertyCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, out IntPtr exception);

    internal delegate IntPtr JSObjectCallAsConstructorCallback(IntPtr ctx, IntPtr constructor, uint argumentCount, IntPtr arguments, out IntPtr exception);
    internal delegate IntPtr JSObjectCallAsFunctionCallback(IntPtr ctx, string className, IntPtr func, IntPtr thisObj, uint argumentCount, IntPtr arguments, out IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasPropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectSetPropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value, out IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectDeletePropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, out IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasInstanceCallback(IntPtr ctx, IntPtr jsClass, IntPtr constructor, IntPtr possibleInstance, out IntPtr exception);
}
