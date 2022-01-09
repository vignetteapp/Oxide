using System;
using System.Runtime.InteropServices;

namespace Mikomi.Javascript
{
    public class JSContext : ManagedObject
    {
        public readonly JSObject Global;
        public readonly JSContextGroup Group;

        public JSContext(IntPtr handle)
            : base(handle)
        {
            Global = new JSObject(this, JavascriptCore.JSContextGetGlobalObject(Handle));
            Group = new JSContextGroup(JavascriptCore.JSContextGetGroup(Handle));
        }
    }

    public class JSContextGroup : ManagedObject
    {
        internal JSContextGroup(IntPtr handle)
            : base(handle)
        {
        }

        public JSContextGroup()
            : base(JavascriptCore.JSContextGroupCreate())
        {
        }

        protected override void DisposeUnmanaged()
            => JavascriptCore.JSContextGroupRelease(Handle);
    }

    public partial class JavascriptCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGroupCreate();

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGroupRetain(IntPtr group);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGroupRelease(IntPtr group);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextCreate(IntPtr globalObjectClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextCreateInGroup(IntPtr group, IntPtr globalObjectClass);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextRetain(IntPtr globalCtx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern void JSGlobalContextRelease(IntPtr globalCtx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGlobalObject(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGroup(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGlobalContext(IntPtr ctx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextCopyName(IntPtr globalCtx);

        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSGlobalContextSetName(IntPtr globalCtx, IntPtr name);
    }
}
