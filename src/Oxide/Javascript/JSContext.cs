using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Interop;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript
{
    public class JSContext : DisposableObject
    {
        /// <summary>
        /// Gets the global object for this <see cref="JSContext"/>.
        /// </summary>
        public readonly JSObject Global;

        internal readonly JSValueRefConverter Converter;
        internal readonly HostObjectProxy Proxy;

        private readonly Dictionary<TypeRegistry, IntPtr> typeCache = new Dictionary<TypeRegistry, IntPtr>();

        public JSContext(IntPtr handle)
            : base(handle)
        {
            Converter = new JSValueRefConverter(this);
            Global = new JSObject(this, JSCore.JSContextGetGlobalObject(Handle));
            Proxy = new HostObjectProxy(this);
        }

        public void RegisterHostType<T>() => RegisterHostType(typeof(T), true);

        internal IntPtr RegisterHostType(Type type, bool makeConstructor)
        {
            var cached = typeCache.FirstOrDefault(pair => pair.Key.FullName == type.FullName);

            if (typeCache.Keys.Any(k => k.FullName == type.FullName))
            {
                if (makeConstructor)
                    makeTypeAvailable(type, cached.Value);

                return cached.Value;
            }

            var registry = new TypeRegistry
            {
                AssemblyName = type.AssemblyQualifiedName,
                FullName = type.FullName
            };

            var klass = Proxy.MakeDerivedClass(registry);
            typeCache.Add(registry, klass);

            if (makeConstructor)
                makeTypeAvailable(type, klass);

            return klass;
        }

        protected override void DisposeUnmanaged()
        {
            foreach (var klass in typeCache.Values)
                JSCore.JSClassRelease(klass);

            Proxy.Dispose();
        }

        private void makeTypeAvailable(Type type, IntPtr klass)
        {
            dynamic global = Global;
            global[type.Name] = new JSObject(this, JSCore.JSObjectMakeConstructor(Handle, klass, Proxy.HandleConstructor));
        }
    }

    public partial class JSCore
    {
        [DllImport(LIB_WEBCORE, ExactSpelling = true)]
        internal static extern IntPtr JSContextGetGlobalObject(IntPtr ctx);
    }
}
