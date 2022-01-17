using System;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript
{
    public class JavaScriptException : Exception
    {
        public JavaScriptException(JSObject error)
            : base(format(error))
        {
        }

        public JavaScriptException(JSObject error, Exception innerException)
            : base(format(error), innerException)
        {
        }

        private static string format(dynamic error) => $"{error.name}: {error.message}";
    }
}
