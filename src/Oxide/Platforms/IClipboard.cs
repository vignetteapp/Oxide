// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide.Platforms
{
    public interface IClipboard
    {
        /// <summary>
        /// Called when the clipboard has to be cleared.
        /// </summary>
        void Clear();

        /// <summary>
        /// Called when the clipboard text is requested.
        /// </summary>
        /// <returns>The text.</returns>
        string GetText();

        /// <summary>
        /// Called when the clipboard text has to be set.
        /// </summary>
        /// <param name="text">The text to be set.</param>
        void SetText(string text);
    }
}
