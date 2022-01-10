using System;

namespace Mikomi.Platform
{
    public interface IFileSystem
    {
        /// <summary>
        /// Called when a file is requested to be opened.
        /// </summary>
        /// <param name="path">The file's path.</param>
        /// <returns>The file's unique id.</returns>
        uint OpenFile(string path);

        /// <summary>
        /// Called when a file is being read.
        /// </summary>
        /// <param name="handle">The file's id.</param>
        /// <param name="data">The data to be read.</param>
        /// <returns>The length of bytes read or -1 on failure.</returns>
        long ReadFile(uint handle, ReadOnlySpan<byte> data);

        /// <summary>
        /// Called when a file is requested to be closed.
        /// </summary>
        /// <param name="handle">The file's id to be closed.</param>
        void CloseFile(uint handle);

        /// <summary>
        /// Called when checking a file exists in a given path.
        /// </summary>
        /// <param name="path">The file's path.</param>
        /// <returns>Whether the file exists or not.</returns>
        bool FileExists(string path);

        /// <summary>
        /// Called when a file's size is to be checked.
        /// </summary>
        /// <param name="handle">The file's id.</param>
        /// <param name="result">The file's size.</param>
        /// <returns>Whether this operation was a success or not.</returns>
        bool GetFileSize(uint handle, out long result);

        /// <summary>
        /// Called when a file's mime type is to be checked.
        /// </summary>
        /// <param name="path">The file's path.</param>
        /// <param name="type">The file's mime type.</param>
        /// <returns>Whether this operation was a success or not.</returns>
        bool GetMimeType(string path, out string type);
    }
}
