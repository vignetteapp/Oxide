// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide.Javascript.Interop
{
    internal class HostObject
    {
        public readonly HostType Type;
        public readonly object Target;

        public HostObject(object target)
        {
            Type = HostType.Get(target.GetType());
            Target = target;
        }
    }
}
