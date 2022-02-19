// Copyright (c) The Vignette Authors
// Licensed under BSD 3-Clause License. See LICENSE for details.

namespace Oxide.Javascript
{
    public class Undefined
    {
        public static readonly Undefined Value = new Undefined();

        private Undefined()
        {
        }

        public override string ToString() => @"[undefined]";
    }
}
