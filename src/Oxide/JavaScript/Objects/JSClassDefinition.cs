using System;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Interop;

namespace Oxide.JavaScript.Objects
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
        internal JSObjectInitializeCallback Initialize;
        internal JSObjectFinalizeCallback Finalize;
        internal JSObjectHasPropertyCallback HasProperty;
        internal JSObjectGetPropertyCallback GetProperty;
        internal JSObjectSetPropertyCallback SetProperty;
        internal JSObjectDeletePropertyCallback DeleteProperty;
        internal JSObjectGetPropertyNamesCallback GetPropertyNames;
        internal JSObjectCallAsFunctionCallback CallAsFunction;
        internal JSObjectCallAsConstructorCallback CallAsConstructor;
        internal JSObjectHasInstanceCallback HasInstance;
        internal JSObjectConvertToTypeCallback ConvertToType;
        internal IntPtr PrivateData;

        internal static JSClassDefinition Empty
        {
            get
            {
                var lib = NativeLibrary.Load(JSCore.LIB_WEBCORE);
                var ptr = NativeLibrary.GetExport(lib, "kJSClassDefinitionEmpty");
                return Marshal.PtrToStructure<JSClassDefinition>(ptr);
            }
        }

        internal delegate void JSObjectInitializeCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj);
        internal delegate void JSObjectFinalizeCallback(IntPtr jsClass, IntPtr obj);
        internal delegate void JSObjectGetPropertyNamesCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyNames);
        internal delegate IntPtr JSObjectCallAsFunctionCallback(IntPtr ctx, IntPtr jsClass, string className, IntPtr func, IntPtr thisObj, uint argumentCount, IntPtr arguments, IntPtr exception);
        internal delegate IntPtr JSObjectCallAsConstructorCallback(IntPtr ctx, IntPtr jsClass, IntPtr constructor, uint argumentCount, IntPtr arguments, IntPtr exception);
        internal delegate IntPtr JSObjectConvertToTypeCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, JSType type, IntPtr exception);
        internal delegate IntPtr JSObjectGetPropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr exception);

        [return: MarshalAs(UnmanagedType.I1)]
        internal delegate bool JSObjectHasPropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName);

        [return: MarshalAs(UnmanagedType.I1)]
        internal delegate bool JSObjectSetPropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr value, IntPtr exception);

        [return: MarshalAs(UnmanagedType.I1)]
        internal delegate bool JSObjectDeletePropertyCallback(IntPtr ctx, IntPtr jsClass, IntPtr obj, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(JSStringRefMarshal))] string propertyName, IntPtr exception);

        [return: MarshalAs(UnmanagedType.I1)]
        internal delegate bool JSObjectHasInstanceCallback(IntPtr ctx, IntPtr jsClass, IntPtr constructor, IntPtr possibleInstance, IntPtr exception);
    }
}
