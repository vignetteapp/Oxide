using System.Runtime.InteropServices;

namespace Oxide.JavaScript
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TypeRegistry
    {
        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string AssemblyName;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string FullName;
    }
}
