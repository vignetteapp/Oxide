using System;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Oxide.Input
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

        public KeyEvent(KeyEventType type, uint modifiers, int virtualKeyCode, int nativeKeyCode, string text, string unmodifiedText, bool isKeypad, bool isAutoRepeat, bool isSystemKey)
            : base(Ultralight.ulCreateKeyEvent(type, modifiers, virtualKeyCode, nativeKeyCode, text, unmodifiedText, isKeypad, isAutoRepeat, isSystemKey))
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
}
