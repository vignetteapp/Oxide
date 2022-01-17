// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.JavaScript.Objects
{
    [Flags]
    public enum JSClassAttributes
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
}
