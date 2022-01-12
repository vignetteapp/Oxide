using System;
using System.Runtime.InteropServices;

namespace Oxide
{
    public partial class Ultralight
    {
        internal const string LIB_ULTRALIGHT = @"Ultralight";

        public static readonly Version Version = new Version((int)ulVersionMajor(), (int)ulVersionMinor(), (int)ulVersionPatch());

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMajor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMinor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionPatch();
    }
}
