// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using Oxide.JavaScript.Interop;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript
{
    public class JSContext : DisposableObject
    {
        /// <summary>
        /// Gets the global object for this <see cref="JSContext"/>.
        /// </summary>
        public readonly dynamic Global;

        /// <summary>
        /// Gets whether this context is locked.
        /// </summary>
        public bool IsLocked { get; internal set; }

        /// <summary>
        /// Gets whether this context is available.
        /// </summary>
        public bool IsAvailable => !IsDisposed && IsLocked;

        internal new IntPtr Handle
        {
            get
            {
                if (!IsAvailable && !IsOwned)
                    throw new InvalidOperationException(@"Attempted to perform operations while the context is unavailable.");

                return base.Handle;
            }
        }

        internal readonly JSValueRefConverter Converter;
        internal readonly HostObjectProxy Proxy;
        private readonly List<JSObjectCallAsConstructorCallback> constructors = new List<JSObjectCallAsConstructorCallback>();

        internal JSContext(IntPtr handle, bool owned = true)
            : base(handle, owned)
        {
            Converter = new JSValueRefConverter(this);
            Global = new JSObject(this, JSCore.JSContextGetGlobalObject(Handle), false);
            Proxy = new HostObjectProxy(this);
        }

        /// <summary>
        /// Registers a host type making it available for use in JavaScript.
        /// </summary>
        /// <param name="name">
        /// The alias this type will go under as. Leave null to use the type's name.
        /// However it must be set when the type has generic type parameters.
        /// </param>
        /// <typeparam name="T">The type to make available to JavaScript.</typeparam>
        public void RegisterHostType<T>(string name = null) => RegisterHostType(typeof(T), name);

        /// <summary>
        /// Registers a host type making it available for use in JavaScript.
        /// </summary>
        /// <param name="name">
        /// The alias this type will go under as. Leave null to use the type's name.
        /// However it must be set when the type has generic type parameters.
        /// </param>
        /// <param name="type">The type to make available to JavaScript.</param>
        public void RegisterHostType(Type type, string name = null) => RegisterHostType(type, name, true);

        /// <summary>
        /// Performs garbage collection forcibly.
        /// </summary>
        public void GarbageCollect() => JSCore.JSGarbageCoillect(Handle);

        /// <summary>
        /// Evaluates a given script. Similar to JavaScript's eval().
        /// </summary>
        /// <param name="script">The script to evaluate.</param>
        /// <param name="exception">The captured Javascript Error object.</param>
        /// <returns>Anything the evaluation may return or <see cref="Undefined"/>.</returns>
        /// <exception cref="JavaScriptException">Thrown when the script provided is invalid.</exception>
        public object Evaluate(string script)
        {
            if (!JSCore.JSCheckScriptSyntax(Handle, script, null, 1, out var error))
                throw new JavaScriptException((JSObject)Converter.ConvertJSValue(error));

            var value = JSCore.JSEvaluateScript(Handle, script, ((JSObject)Global).Handle, null, 1, out error);
            var converted = Converter.ConvertJSValue(value);

            if (error != IntPtr.Zero)
                throw new JavaScriptException((JSObject)Converter.ConvertJSValue(error));

            return converted;
        }

        internal IntPtr RegisterHostType(Type type, string name = null, bool makeConstructor = true)
        {
            var klass = Proxy.GetJSClassFor(type, out var data);

            if (makeConstructor)
            {
                if (type.GenericTypeArguments.Length > 0 && string.IsNullOrEmpty(name))
                    throw new ArgumentNullException(nameof(name));

                makeTypeAvailable(name ?? type.Name, klass, data);
            }

            return klass;
        }

        private void makeTypeAvailable(string name, IntPtr klass, IntPtr privateData)
        {
            JSObjectCallAsConstructorCallback callback;

            var constructor = JSCore.JSObjectMakeConstructor(Handle, klass, callback = Proxy.HandleConstructor);
            var prototype = JSCore.JSObjectGetProperty(Handle, constructor, "prototype", out _);

            JSCore.JSObjectSetPrivate(prototype, privateData);

            Global[name] = new JSObject(this, constructor);
            constructors.Add(callback);
        }

        protected override void DisposeManaged()
        {
            Proxy.Dispose();
            constructors.Clear();
        }

        protected override void DisposeUnmanaged() => JSCore.JSGlobalContextRelease(Handle);
    }
}
