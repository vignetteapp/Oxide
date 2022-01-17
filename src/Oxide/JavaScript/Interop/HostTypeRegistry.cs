// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Oxide.JavaScript.Objects;

namespace Oxide.JavaScript.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal class HostTypeRegistry
    {
        public JSClassDefinition Definition;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string AssemblyName;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string FullName;

        [MarshalAs(UnmanagedType.LPUTF8Str)]
        public string Name;
        private IntPtr instanceMembers;

        public IReadOnlyList<MemberInfo> InstanceMembers
        {
            get => (IReadOnlyList<MemberInfo>)GCHandle.FromIntPtr(instanceMembers).Target;
            set
            {
                if (instanceMembers != IntPtr.Zero)
                {
                    GCHandle.FromIntPtr(instanceMembers).Free();
                }

                if (value != null)
                {
                    var mInst = GCHandle.Alloc(value, GCHandleType.Normal);
                    instanceMembers = GCHandle.ToIntPtr(mInst);
                }
            }
        }

        public IntPtr ToPointer()
        {
            int size = Marshal.SizeOf<HostTypeRegistry>();
            var pointer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(this, pointer, false);
            return pointer;
        }
    }
}
