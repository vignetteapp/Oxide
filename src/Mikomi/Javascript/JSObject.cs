using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Mikomi.Javascript
{
    public class JSObject : DynamicObject, IJSValue, IEquatable<JSObject>
    {
        internal readonly IntPtr Handle;

        public JSContext Context { get; }
        public JSType Type => JavascriptCore.JSValueGetType(Context.Handle, Handle);
        public bool IsArray => JavascriptCore.JSValueIsArray(Context.Handle, Handle);
        public JSTypedArrayType ArrayType => JavascriptCore.JSValueGetTypedArrayType(Context.Handle, Handle, IntPtr.Zero);

        internal JSObject(JSContext ctx, IntPtr handle)
        {
            Context = ctx;
            Handle = handle;
        }

        public bool Equals(IJSValue other)
            => Equals(other as JSObject);

        public bool Equals(JSObject other)
            => JavascriptCore.JSValueIsStrictEqual(Context.Handle, Handle, other?.Handle ?? IntPtr.Zero);

        public override bool Equals(object obj)
            => Equals(obj as JSObject);

        public override int GetHashCode()
            => HashCode.Combine(Handle, Context, Type, IsArray, ArrayType);
    }

    [Flags]
    public enum JSPropertyAttribute : uint
    {
        /// <summary>
        /// Specifies that a property has no special attributes.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that a property is read-only.
        /// </summary>
        ReadOnly = 1 << 1,

        /// <summary>
        /// Specifies that a property should not be enumerated by JSPropertyEnumerators and Javascript for...in loops.
        /// </summary>
        DontEnum = 1 << 2,

        /// <summary>
        /// Specifies that the delete operation should fail on a property.
        /// </summary>
        DontDelete = 1 << 3,
    }

    [Flags]
    public enum JSClassAttributes : uint
    {
        /// <summary>
        /// Specifies that a class has no special attributes.
        /// </summary>
        None = 0,

        /// <summary>
        /// Specifies that a class should not automatically generate a shared prototype for its instance objects.
        /// </summary>
        NoAutomaticPrototype = 1 << 1,
    }

    internal delegate void JSObjectInitializeCallback(IntPtr ctx, IntPtr obj);
    internal delegate void JSObjectInitializeCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj);
    internal delegate void JSObjectFinalizeCallback(IntPtr obj);
    internal delegate void JSObjectFinalizeCallbackEx(IntPtr jsClass, IntPtr obj);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasPropertyCallback(IntPtr ctx, IntPtr obj, IntPtr propertyName);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasPropertyCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyName);

    internal delegate IntPtr JSObjectGetPropertyCallback(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr exception);
    internal delegate IntPtr JSObjectGetPropertyCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyName, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectSetPropertyCallback(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr value, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectSetPropertyCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyName, IntPtr value, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectDeletePropertyCallback(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectDeletePropertyCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyName, IntPtr exception);

    internal delegate void JSObjectGetPropertyNamesCallback(IntPtr ctx, IntPtr obj, IntPtr propertyNames);
    internal delegate void JSObjectGetPropertyNamesCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr propertyNames);
    internal delegate IntPtr JSObjectCallAsFunctionCallback(IntPtr ctx, IntPtr obj, IntPtr thisObj, uint argumentCount, IntPtr arguments, IntPtr exception);
    internal delegate IntPtr JSObjectCallAsFunctionCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, IntPtr thisObj, uint argumentCount, IntPtr arguments, IntPtr exception);
    internal delegate IntPtr JSObjectCallAsConstructorCallback(IntPtr ctx, IntPtr constructor, uint argumentCount, IntPtr arguments, IntPtr exception);
    internal delegate IntPtr JSObjectCallAsConstructorCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr constructor, uint argumentCount, IntPtr arguments, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasInstanceCallback(IntPtr ctx, IntPtr constructor, IntPtr possibleInstance, IntPtr exception);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool JSObjectHasInstanceCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr constructor, IntPtr possibleInstance, IntPtr exception);

    internal delegate IntPtr JSObjectConvertToTypeCallback(IntPtr ctx, IntPtr obj, JSType type, IntPtr exception);
    internal delegate IntPtr JSObjectConvertToTypeCallbackEx(IntPtr ctx, IntPtr jsClass, IntPtr obj, JSType type, IntPtr exception);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct JSStaticValue
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectGetPropertyCallback GetProperty;

        internal JSObjectSetPropertyCallback SetProperty;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct JSStaticValueEx
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectGetPropertyCallbackEx GetProperty;

        internal JSObjectSetPropertyCallbackEx SetProperty;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct JSStaticFunction
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectCallAsFunctionCallback GetProperty;

        internal JSPropertyAttribute Attributes;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    internal struct JSStaticFunctionEx
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectCallAsFunctionCallbackEx GetProperty;

        internal JSPropertyAttribute Attributes;
    }

    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi)]
    internal struct JSClassDefinition
    {
        [FieldOffset(0)]
        internal int Version;

        [FieldOffset(4)]
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        [FieldOffset(0)]
        internal JSStaticValue StaticValues;

        [FieldOffset(0)]
        internal JSStaticFunction StaticFunctions;

        [FieldOffset(0)]
        internal JSObjectInitializeCallback Initialize;

        [FieldOffset(0)]
        internal JSObjectFinalizeCallback Finalize;

        [FieldOffset(0)]
        internal JSObjectHasPropertyCallback HasProperty;

        [FieldOffset(0)]
        internal JSObjectGetPropertyCallback GetProperty;

        [FieldOffset(0)]
        internal JSObjectSetPropertyCallback SetProperty;

        [FieldOffset(0)]
        internal JSObjectDeletePropertyCallback DeleteProperty;

        [FieldOffset(0)]
        internal JSObjectGetPropertyNamesCallback GetPropertyNames;

        [FieldOffset(0)]
        internal JSObjectCallAsFunctionCallback CallAsFunction;

        [FieldOffset(0)]
        internal JSObjectCallAsConstructorCallback CallAsConstructor;

        [FieldOffset(0)]
        internal JSObjectHasInstanceCallback HasInstance;

        [FieldOffset(0)]
        internal JSObjectConvertToTypeCallback ConvertToType;

        [FieldOffset(0)]
        internal JSStaticValueEx StaticValuesEx;

        [FieldOffset(0)]
        internal JSStaticFunctionEx StaticFunctionsEx;

        [FieldOffset(0)]
        internal JSObjectInitializeCallbackEx InitializeEx;

        [FieldOffset(0)]
        internal JSObjectFinalizeCallbackEx FinalizeEx;

        [FieldOffset(0)]
        internal JSObjectHasPropertyCallbackEx HasPropertyEx;

        [FieldOffset(0)]
        internal JSObjectGetPropertyCallbackEx GetPropertyEx;

        [FieldOffset(0)]
        internal JSObjectSetPropertyCallbackEx SetPropertyEx;

        [FieldOffset(0)]
        internal JSObjectDeletePropertyCallbackEx DeletePropertyEx;

        [FieldOffset(0)]
        internal JSObjectGetPropertyNamesCallbackEx GetPropertyNamesEx;

        [FieldOffset(0)]
        internal JSObjectCallAsFunctionCallbackEx CallAsFunctionEx;

        [FieldOffset(0)]
        internal JSObjectCallAsConstructorCallbackEx CallAsConstructorEx;

        [FieldOffset(0)]
        internal JSObjectHasInstanceCallbackEx HasInstanceEx;

        [FieldOffset(0)]
        internal JSObjectConvertToTypeCallbackEx ConvertToTypeEx;

        [FieldOffset(0)]
        internal IntPtr PrivateData;
    }

    public partial class JavascriptCore
    {
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

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        internal static extern IntPtr JSObjectMakeFunctionWithCallback(IntPtr ctx, IntPtr name, JSObjectCallAsFunctionCallback callAsFunction);

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
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
        internal static extern IntPtr JSObjectMakeFunction(IntPtr ctx, IntPtr name, uint parameterCount, IntPtr parameterNames, IntPtr body, IntPtr sourceURL, int startingLineNumber, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrototype(IntPtr ctx, IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPrototype(IntPtr ctx, IntPtr obj, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectHasProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeleteProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr exception);

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
        internal static extern IntPtr JSObjectGetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIntex, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSObjectSetPropertyAtIndex(IntPtr ctx, IntPtr obj, uint propertyIndex, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivate(IntPtr obj);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivate(IntPtr obj, IntPtr data);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectIsFunction(IntPtr obj, IntPtr data);

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
        internal static extern IntPtr JSPropertyNameArrayGetNameAtIndex(IntPtr array, uint index);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSPropertyNameAccumulatorAddName(IntPtr accumulator, IntPtr propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectSetPrivateProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSObjectGetPrivateProperty(IntPtr ctx, IntPtr obj, IntPtr propertyName);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSObjectDeletePrivateProperty(IntPtr ctx, IntPtr obj, IntPtr proeprtyName);
    }
}
