// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

using System.Collections.Generic;

namespace Oxide.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            return dictionary[key] = value;
        }
    }
}
