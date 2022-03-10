// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Javascript
{
    internal class JavascriptException : Exception
    {
        public JavascriptException(string message, JSValue error = default)
            : base(message, getError(message, error))
        {
        }

        private static Exception getError(string message, JSValue value)
        {
            if (value.Type == JSType.Object)
            {
                var obj = value.GetValue();

                if (obj is Exception e)
                    return e;

                if (obj is JSObject jsObj)
                    return new JavascriptException(message, ((dynamic)jsObj).toString());
            }

            return null;
        }
    }
}
