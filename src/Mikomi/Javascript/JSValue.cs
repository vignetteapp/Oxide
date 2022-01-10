using System;
using System.Runtime.InteropServices;

namespace Mikomi.Javascript
{
    public abstract class JSValue : DisposableObject, IJSValue, IEquatable<JSValue>
    {
        public JSContext Context { get; }
        public JSType Type => JavascriptCore.JSValueGetType(Context.Handle, Handle);
        public bool IsArray => JavascriptCore.JSValueIsArray(Context.Handle, Handle);
        public JSTypedArrayType ArrayType => JavascriptCore.JSValueGetTypedArrayType(Context.Handle, Handle, IntPtr.Zero);

        internal JSValue(JSContext ctx, IntPtr handle)
            : base(handle)
        {
            Context = ctx;
        }

        public bool Equals(IJSValue other)
            => Equals(other as JSValue);

        public bool Equals(JSValue other)
            => JavascriptCore.JSValueIsStrictEqual(Context.Handle, Handle, other.Handle);

        public override bool Equals(object obj)
            => Equals(obj as JSValue);

        public override int GetHashCode()
            => HashCode.Combine(Handle, Context, Type, IsArray, ArrayType);
    }

    public abstract class JSValue<T> : JSValue, IJSValue<T>
    {
        public abstract T Value { get; }

        internal JSValue(JSContext ctx, IntPtr handle)
            : base(ctx, handle)
        {
        }

        public bool Equals(IJSValue<T> other)
            => Equals(other as JSValue);

        public override bool Equals(object obj)
            => Equals(obj as JSValue);

        public override int GetHashCode()
            => HashCode.Combine(Handle, Context, Value, Type, IsArray, ArrayType);
    }

    public sealed class JSUndefined : JSValue
    {
        internal JSUndefined(JSContext ctx)
            : base(ctx, JavascriptCore.JSValueMakeUndefined(ctx.Handle))
        {
        }
    }

    public sealed class JSNull : JSValue
    {
        internal JSNull(JSContext ctx)
            : base(ctx, JavascriptCore.JSValueMakeNull(ctx.Handle))
        {
        }
    }

    public sealed class JSBoolean : JSValue<bool>
    {
        public override bool Value => JavascriptCore.JSValueToBoolean(Context.Handle, Handle);

        internal JSBoolean(JSContext ctx, bool boolean)
            : base(ctx, JavascriptCore.JSValueMakeBoolean(ctx.Handle, boolean))
        {
        }
    }

    public sealed class JSNumber : JSValue<double>
    {
        public override double Value => JavascriptCore.JSValueToNumber(Context.Handle, Handle, IntPtr.Zero);

        internal JSNumber(JSContext ctx, double number)
            : base(ctx, JavascriptCore.JSValueMakeNumber(ctx.Handle, number))
        {
        }
    }

    public sealed class JSSymbol : JSValue
    {
        public JSSymbol(JSContext ctx, string description)
            : base(ctx, JavascriptCore.JSValueMakeSymbol(ctx.Handle, JavascriptCore.JSStringCreateWithUTF8String(description)))
        {
        }
    }

    public enum JSType
    {
        /// <summary>
        /// The unique undefined value.
        /// </summary>
        Undefined,

        /// <summary>
        /// The unique null value.
        /// </summary>
        Null,

        /// <summary>
        /// A primiteve boolean value, one of true or false.
        /// </summary>
        Boolean,

        /// <summary>
        /// A primitive number value.
        /// </summary>
        Number,

        /// <summary>
        /// A primitive string value.
        /// </summary>
        String,

        /// <summary>
        /// An object value (meaning that this <see cref="JSValue"/> is a <see cref="JSObject"/>)
        /// </summary>
        Object,

        /// <summary>
        /// A primitive symbol value.
        /// </summary>
        Symbol,
    }

    public enum JSTypedArrayType
    {
        Int8Array,
        Int16Array,
        Int32Array,
        Uint8Array,
        Uint8ClampedArray,
        Uint32Array,
        Float32Array,
        Float64Array,
        ArrayBuffer,
        None,
    }

    public partial class JavascriptCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSType JSValueGetType(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsUndefined(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsNull(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsBoolean(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsNumber(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsString(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsSymbol(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsObjectOfClass(IntPtr ctx, IntPtr value, IntPtr jsClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsArray(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsDate(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern JSTypedArrayType JSValueGetTypedArrayType(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsEqual(IntPtr ctx, IntPtr a, IntPtr b, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsStrictEqual(IntPtr ctx, IntPtr a, IntPtr b);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueIsInstanceOfConstructor(IntPtr ctx, IntPtr value, IntPtr constructor, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeUndefined(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNull(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeBoolean(IntPtr ctx, [MarshalAs(UnmanagedType.I1)] bool boolean);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeNumber(IntPtr ctx, double number);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeSymbol(IntPtr ctx, IntPtr description);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueMakeFromJSONString(IntPtr ctx, IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueCreateJSONString(IntPtr ctx, IntPtr value, uint indent, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSValueToBoolean(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern double JSValueToNumber(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueToStringCopy(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSValueToObject(IntPtr ctx, IntPtr value, IntPtr exception);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSValueProtect(IntPtr ctx, IntPtr value);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSValueUnprotect(IntPtr ctx, IntPtr value);
    }
}
