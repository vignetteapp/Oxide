using System;
using System.Runtime.InteropServices;

namespace Mikomi
{
    public partial class Ultralight
    {
        internal const string LIB_ULTRALIGHT = @"Ultralight";
        internal const string LIB_JSCORE = @"Javascript";
        internal const string LIB_APPCORE = @"AppCore";

        public static readonly Version Version = new Version((int)ulVersionMajor(), (int)ulVersionMinor(), (int)ulVersionPatch());

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMajor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionMinor();

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        private static extern uint ulVersionPatch();
    }
}
