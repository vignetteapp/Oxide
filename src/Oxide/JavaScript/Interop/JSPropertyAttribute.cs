// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Javascript.Interop
{
    [Flags]
    internal enum JSPropertyAttribute : uint
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
}
