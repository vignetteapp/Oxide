using System;
using System.Runtime.InteropServices;

namespace Mikomi.Platform
{
    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool FileSystemFileExistsCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path
    );

    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool FIleSystemFileSizeCallback(uint handle, out int result);

    [return: MarshalAs(UnmanagedType.I1)]
    public delegate bool FileSystemGetFileMimeTypeCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] out string result
    );

    public delegate uint FileSystemOpenFileCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path,
        [MarshalAs(UnmanagedType.I1)] bool openForWriting
    );

    public delegate ulong FileSystemReadFromFileCallback(uint handle, Span<byte> data, int length);

    [StructLayout(LayoutKind.Sequential)]
    public struct FileSystem : IDisposable
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
