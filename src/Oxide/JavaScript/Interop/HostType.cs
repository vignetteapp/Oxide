// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Oxide.Javascript.Interop
{
    internal class HostType
    {
        private static readonly List<HostType> cache = new List<HostType>();

        public static HostType Get(Type type)
        {
            var result = cache.FirstOrDefault(h => h.Type == type);

            if (result == null)
            {
                result = new HostType(type);
                cache.Add(result);
            }

            return result;
        }

        public string Name => Type.Name;
        public readonly Type Type;
        public readonly IReadOnlyList<MemberInfo> Members;
        public readonly IReadOnlyList<HostMethod> Methods;

        private HostType(Type type)
        {
            Type = type;
            Members = type.GetMembers(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            Methods = Members.OfType<MethodBase>().Where(m => !m.IsGenericMethod).Select(m => new HostMethod(type, m)).ToArray();
        }
    }
}
