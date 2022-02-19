// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide.Javascript
{
    public enum JSType
    {
        /// <summary>
        /// The unique undefined value.
        /// </summary>
        Undefined,

        /// <summary>
        /// The unique null value.
        /// </summary>
        Null,

        /// <summary>
        /// A primiteve boolean value, one of true or false.
        /// </summary>
        Boolean,

        /// <summary>
        /// A primitive number value.
        /// </summary>
        Number,

        /// <summary>
        /// A primitive string value.
        /// </summary>
        String,

        /// <summary>
        /// An object value (meaning that this <see cref="JSObject"/> is a <see cref="JSObject"/>)
        /// </summary>
        Object,

        /// <summary>
        /// A primitive symbol value.
        /// </summary>
        Symbol,
    }
}
