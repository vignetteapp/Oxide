// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Input
{
    [Flags]
    public enum KeyModifiers : uint
    {
        /// <summary>
        /// Alt Key
        /// </summary>
        Alt = 1 << 0,

        /// <summary>
        /// Control key
        /// </summary>
        Control = 1 << 1,

        /// <summary>
        /// Meta key (Command key on Mac, Windows key on Windows)
        /// </summary>
        Meta = 1 << 2,

        /// <summary>
        /// Shift Key
        /// </summary>
        Shift = 1 << 3
    }
}
