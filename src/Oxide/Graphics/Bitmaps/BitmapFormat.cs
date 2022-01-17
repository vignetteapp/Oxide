// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide.Graphics.Bitmaps
{
    public enum BitmapFormat
    {
        /// <summary>
        /// Alpha channel only, 8-bits per pixel.
        /// <br/>
        /// Encoding: 8-bits per channel, unsigned normalized.
        /// <br/>
        /// Color-space: Linear (no gamma), alpha-coverage only.
        /// </summary>
        A8_UNORM,

        /// <summary>
        /// Blue Green Red Alpha channels, 32-bits per pixel.
        /// <br/>
        /// Encoding: 8-bits per channel, unsigned normalized.
        /// <br/>
        /// Color-space: sRGB gamma with premultiplied linear alpha channel.
        /// </summary>
        BGRA8_UNORM_SRGB,
    }
}
