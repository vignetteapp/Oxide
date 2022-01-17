// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Apps
{
    [Flags]
    public enum WindowFlags
    {
        Borderless = 1 << 0,
        Titled = 1 << 1,
        Resizable = 1 << 2,
        Maximisable = 1 << 3,
        Hidden = 1 << 4,
    }
}
