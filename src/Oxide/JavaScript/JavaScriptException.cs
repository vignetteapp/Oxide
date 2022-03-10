// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;

namespace Oxide.Javascript
{
    internal class JavascriptException : Exception
    {
        public static Exception Throw(JSContext context, IntPtr handle)
        {
            var jsObject = new JSObject(context, handle, false);

            if (jsObject.IsHostObject)
            {
                var hostObject = jsObject.GetHostObject();

                if (hostObject is Exception e)
                    return new JavascriptException("An exception has occurred from the host.", e);

                return new JavascriptException(hostObject.ToString());
            }

            return new JavascriptException(format(jsObject));
        }

        private static string format(dynamic error) => $"{error.name}: {error.message}";

        private JavascriptException(string message)
            : base(message)
        {
        }

        private JavascriptException(string message, Exception underlyingException)
            : base(message, underlyingException)
        {
        }
    }
}
