using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Oxide.Graphics.Drivers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct GPUCommandList
    {
        private readonly uint count;
        private readonly IntPtr commands;

        public IReadOnlyList<GPUCommand> Commands
        {
            get
            {
                int size = Marshal.SizeOf<GPUCommand>();
                var array = new GPUCommand[count];

                for (int i = 0; i < count; i++)
                {
                    var item = new IntPtr(commands.ToInt64() + i * size);
                    array[i] = Marshal.PtrToStructure<GPUCommand>(item);
                }

                return array;
            }
        }
    }
}
