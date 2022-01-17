using System.Runtime.InteropServices;

namespace Oxide.JavaScript.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct JSStaticFunction
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectCallAsFunctionCallbackEx Call;

        internal JSPropertyAttribute Attributes;
    }
}
