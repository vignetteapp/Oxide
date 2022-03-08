// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using Oxide.Javascript.Interop;
using Oxide.Javascript.Objects;

namespace Oxide.Javascript
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

        internal JSContext(IntPtr handle, bool owned = true)
            : base(handle, owned)
        {
            Global = new JSObject(handle, JSCore.JSContextGetGlobalObject(Handle), false);
        }

        /// <summary>
        /// Registers a host type making it available for use in Javascript.
        /// </summary>
        /// <param name="name">
        /// The alias this type will go under as. Leave null to use the type's name.
        /// However it must be set when the type has generic type parameters.
        /// </param>
        /// <typeparam name="T">The type to make available to Javascript.</typeparam>
        public void RegisterHostType<T>(string name = null) => RegisterHostType(typeof(T), name);

        /// <summary>
        /// Registers a host type making it available for use in Javascript.
        /// </summary>
        /// <param name="type">The type to make available to Javascript.</param>
        /// <param name="name">
        /// The alias this type will go under as. Leave null to use the type's name.
        /// However it must be set when the type has generic type parameters.
        /// </param>
        public void RegisterHostType(Type type, string name = null)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            // if (type.IsGenericType)
            //     throw new ArgumentException($"{type.Name} contains generic arguments.", nameof(type));

            Global[name ?? type.Name] = type;
        }

        /// <summary>
        /// Performs garbage collection forcibly.
        /// </summary>
        public void GarbageCollect() => JSCore.JSGarbageCoillect(Handle);

        /// <summary>
        /// Evaluates a given script. Similar to Javascript's eval().
        /// </summary>
        /// <param name="script">The script to evaluate.</param>
        /// <param name="exception">The captured Javascript Error object.</param>
        /// <returns>Anything the evaluation may return or <see cref="Undefined"/>.</returns>
        /// <exception cref="JavascriptException">Thrown when the script provided is invalid.</exception>
        public object Evaluate(string script)
        {
            IntPtr value = IntPtr.Zero;

            if (JSCore.JSCheckScriptSyntax(Handle, script, null, 1, out var error))
                value = JSCore.JSEvaluateScript(Handle, script, ((JSObject)Global).Handle, null, 1, out error);

            if (error != IntPtr.Zero)
                throw JavascriptException.GetException(Handle, error);

            return JSValueRefConverter.ConvertJSValue(Handle, value);
        }

        protected override void DisposeUnmanaged() => JSCore.JSGlobalContextRelease(Handle);
    }
}
