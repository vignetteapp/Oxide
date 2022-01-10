using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Mikomi.Input;

namespace Mikomi.Input
{
    public class KeyEvent : DisposableObject
    {
        public readonly KeyEventType Type;
        public readonly uint Modifiers;
        public readonly int VirtualKeyCode;
        public readonly int NativeKeyCode;
        public readonly string Text;
        public readonly string UnmodifiedText;
        public readonly bool IsKeypad;
        public readonly bool IsAutoRepeat;
        public readonly bool IsSystemKey;

        protected KeyEvent(IntPtr handle)
            : base(handle)
        {
        }

        public KeyEvent(KeyEventType type, uint modifiers, int virtualKeyCode, int nativeKeyCode,
            string text, string unmodifiedText, bool isKeypad, bool isAutoRepeat, bool isSystemKey)
            : base(createKeyEvent(type, modifiers, virtualKeyCode, nativeKeyCode, text, unmodifiedText, isKeypad, isAutoRepeat, isSystemKey))
        {
            Type = type;
            Modifiers = modifiers;
            VirtualKeyCode = virtualKeyCode;
            NativeKeyCode = nativeKeyCode;
            Text = text;
            IsKeypad = isKeypad;
            IsAutoRepeat = isAutoRepeat;
            IsSystemKey = isSystemKey;
        }

        private static IntPtr createKeyEvent(KeyEventType type, uint modifiers, int virtualKeyCode, int nativeKeyCode,
            string text, string unmodifiedText, bool isKeypad, bool isAutoRepeat, bool isSystemKey)
        {
            return Ultralight.ulCreateKeyEvent(type, modifiers, virtualKeyCode, nativeKeyCode,
                text, unmodifiedText, isKeypad, isAutoRepeat, isSystemKey);
        }

        /// <summary>
        /// Create a key event from a Windows key event.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"/>
        [SupportedOSPlatform("Windows")]
        public static KeyEvent CreateKeyEventFromWindowsEvent(KeyEventType type, UIntPtr wparam, UIntPtr lparam, bool isSystemKey)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new PlatformNotSupportedException();

            return new KeyEvent(Ultralight.ulCreateKeyEventWindows(type, wparam, lparam, isSystemKey));
        }

        /// <summary>
        /// Create a key event from an NSEvent.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException"/>
        [SupportedOSPlatform("OSX")]
        public static KeyEvent CreateKeyEventFromNSEvent(IntPtr evt)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                throw new PlatformNotSupportedException();

            return new KeyEvent(Ultralight.ulCreateKeyEventMacOS(evt));
        }
    }

    public enum KeyEventType
    {
        /// <summary>
        /// Key-Down event type. (Does not trigger accelerator commands in WebCore)
        /// </summary>
        /// <remarks>
        /// You should probably use <see cref="RawKeyDown"/> instead when a physical key
        /// is pressed. This member is only here for historic compatibility
        /// with WebCore's key event types.
        /// </remarks>
        KeyDown,

        /// <summary>
        /// Key-Up event type. Use this when a physical key is released.
        /// </summary>
        KeyUp,

        /// <summary>
        /// Raw Key-Down type. Use this when a physical key is pressed.
        /// </summary>
        /// <remarks>
        /// You should use <see cref="RawKeyDown"/> for physical key presses since it
        /// allows WebCore to do additional command translation.
        /// </remarks>
        RawKeyDown,

        /// <summary>
        /// Character input event type. Use this when the OS generates text from
        /// a physical key being pressed (eg, WM_CHAR on Windows).
        /// </summary>
        Char,
    }
}

namespace Mikomi
{
    public partial class Ultralight
    {
        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEvent(
            KeyEventType type,
            uint modifiers,
            int virtualKeyCode,
            int nativeKeyCode,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string text,
            [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ULStringMarshaler))] string unmodifiedText,
            [MarshalAs(UnmanagedType.I1)] bool isKeypad,
            [MarshalAs(UnmanagedType.I1)] bool isAutoRepeat,
            [MarshalAs(UnmanagedType.I1)] bool isSystemKey);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEventWindows(
            KeyEventType type,
            UIntPtr wparam,
            UIntPtr lparam,
            [MarshalAs(UnmanagedType.I1)] bool isSystemKey);

        [DllImport(LIB_ULTRALIGHT, ExactSpelling = true)]
        internal static extern IntPtr ulCreateKeyEventMacOS(IntPtr evt);
    }
}
