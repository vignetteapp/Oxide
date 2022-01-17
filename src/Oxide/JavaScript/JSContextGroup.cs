// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;

namespace Oxide.JavaScript
{
    public class JSContextGroup : DisposableObject
    {
        private readonly List<JSContext> contexts = new List<JSContext>();

        public JSContextGroup()
            : base(JSCore.JSContextGroupCreate())
        {
        }

        public JSContext CreateContext()
        {
            var context = new JSContext(JSCore.JSGlobalContextCreateInGroup(Handle, IntPtr.Zero));
            contexts.Add(context);
            return context;
        }

        protected override void DisposeManaged()
        {
            foreach (var context in contexts)
                context?.Dispose();
        }

        protected override void DisposeUnmanaged()
            => JSCore.JSContextGroupRelease(Handle);
    }
}
