// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using Oxide.Javascript.Objects;

namespace Oxide.Javascript
{
    public class JavascriptException : Exception
    {
        public JavascriptException(JSObject error)
            : base(format(error))
        {
        }

        public JavascriptException(JSObject error, Exception innerException)
            : base(format(error), innerException)
        {
        }

        private static string format(dynamic error) => $"{error.name}: {error.message}";
    }
}
