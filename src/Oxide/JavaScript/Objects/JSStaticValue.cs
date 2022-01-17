using System.Runtime.InteropServices;

namespace Oxide.JavaScript.Objects
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct JSStaticValue
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        internal string Name;

        internal JSObjectGetPropertyCallbackEx GetProperty;
        internal JSObjectSetPropertyCallback SetProperty;
        internal JSPropertyAttribute Attributes;
    }
}
