// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Runtime.InteropServices;
using Oxide.Interop;

namespace Oxide.Platforms
{
    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool FileSystemFileExistsCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path
    );

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool FileSystemGetFileSizeCallback(uint handle, out long result);

    [return: MarshalAs(UnmanagedType.I1)]
    internal delegate bool FileSystemGetFileMimeTypeCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path,
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] out string result
    );

    internal delegate uint FileSystemOpenFileCallback(
        [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string path,
        [MarshalAs(UnmanagedType.I1)] bool openForWriting
    );

    internal unsafe delegate long FileSystemReadFromFileCallback(uint handle, byte* data, long length);
    internal delegate void FileSystemCloseFileCallback(uint handle);

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileSystem
    {
        public FileSystemFileExistsCallback FileExists;
        public FileSystemGetFileSizeCallback GetFileSize;
        public FileSystemGetFileMimeTypeCallback GetMimeType;
        public FileSystemOpenFileCallback OpenFile;
        public FileSystemCloseFileCallback CloseFile;
        public FileSystemReadFromFileCallback ReadFromFile;
    }
}
