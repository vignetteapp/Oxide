using System;
using System.Runtime.InteropServices;

namespace Oxide.Javascript
{
    public sealed class JSString : DisposableObject, IJSValue<string>, IEquatable<JSString>, IEquatable<string>
    {
        public JSContext Context { get; }
        public int Length => (int)JavascriptCore.JSStringGetLength(Handle);
        public JSType Type => Context != null ? JavascriptCore.JSValueGetType(Context.Handle, Handle) : JSType.String;
        public bool IsArray => Context != null && JavascriptCore.JSValueIsArray(Context.Handle, Handle);
        public JSTypedArrayType ArrayType => Context != null ? JavascriptCore.JSValueGetTypedArrayType(Context.Handle, Handle, IntPtr.Zero) : JSTypedArrayType.None;

        public string Value
        {
            get
            {
                var buffer = new IntPtr();

                uint length = JavascriptCore.JSStringGetUTF8CString(Handle, buffer, JavascriptCore.JSStringGetLength(Handle));
                if (length > 0)
                {
                    return Marshal.PtrToStringAnsi(buffer);
                }
                else
                {
                    return string.Empty;
                }
            }
        }


        public JSString(string str)
            : this(null, str)
        {
        }

        internal JSString(JSContext ctx, string str)
            : this(ctx, JavascriptCore.JSStringCreateWithUTF8String(str))
        {
        }

        internal JSString(JSContext ctx, IntPtr handle)
            : base(handle)
        {
            Context = ctx;
        }

        protected override void DisposeUnmanaged()
        {
            if (Context == null)
                JavascriptCore.JSStringRelease(Handle);
        }

        public bool Equals(JSString other)
            => JavascriptCore.JSStringIsEqual(Handle, other?.Handle ?? IntPtr.Zero);

        public bool Equals(IJSValue other)
            => Equals(other as IJSValue<string>);

        public bool Equals(IJSValue<string> other)
            => Equals(other as JSString);

        bool IEquatable<string>.Equals(string other)
            => JavascriptCore.JSStringIsEqualToUTF8CString(Handle, other ?? string.Empty);

        public override bool Equals(object obj)
            => Equals(obj as JSString);

        public override int GetHashCode()
            => HashCode.Combine(Handle, Context, Value, Type, IsArray, ArrayType);
    }

    public partial class JavascriptCore
    {

#pragma warning disable CA2101 // char* is a null-terminating UTF-8 encoded string

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr JSStringCreateWithUTF8String([MarshalAs(UnmanagedType.LPUTF8Str)] string str);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSStringRetain(IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSStringRelease(IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSStringGetLength(IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSStringGetCharactersPtr(IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSStringGetMaximumUTF8CStringSize(IntPtr jsString);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern uint JSStringGetUTF8CString(IntPtr str, IntPtr buffer, uint bufferSize);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSStringIsEqual(IntPtr a, IntPtr b);

        [DllImport(LIB_WEBCORE, ExactSpelling = true, CharSet = CharSet.Ansi, BestFitMapping = true, ThrowOnUnmappableChar = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static extern bool JSStringIsEqualToUTF8CString(IntPtr a, [MarshalAs(UnmanagedType.LPUTF8Str)] string b);

#pragma warning restore CA2101

    }
}
