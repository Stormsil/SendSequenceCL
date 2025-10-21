using System;
using System.Collections.Generic;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Maps characters and virtual keys to HID keyboard codes and modifier flags.
    /// </summary>
    internal static class KeyboardMapper
    {
        /// <summary>
        /// Converts a VirtualKey to HID Usage ID.
        /// </summary>
        /// <param name="key">Virtual key to convert.</param>
        /// <returns>HID Usage ID byte.</returns>
        public static byte VirtualKeyToHidCode(VirtualKey key)
        {
            // VirtualKey enum values are already HID Usage IDs
            return (byte)key;
        }

        /// <summary>
        /// Converts a VirtualKey to modifier bit flag.
        /// Only works for modifier keys (Control, Shift, Alt, Windows).
        /// </summary>
        /// <param name="key">Modifier key.</param>
        /// <returns>Modifier bit flag (1, 2, 4, 8, 16, 32, 64, 128).</returns>
        public static byte VirtualKeyToModifierFlag(VirtualKey key)
        {
            return key switch
            {
                VirtualKey.LeftControl => 1,    // Bit 0
                VirtualKey.LeftShift => 2,      // Bit 1
                VirtualKey.LeftAlt => 4,        // Bit 2
                VirtualKey.Windows => 8,        // Bit 3
                VirtualKey.RightControl => 16,  // Bit 4
                VirtualKey.RightShift => 32,    // Bit 5
                VirtualKey.RightAlt => 64,      // Bit 6
                _ => 0
            };
        }

        /// <summary>
        /// Checks if a VirtualKey is a modifier key.
        /// </summary>
        public static bool IsModifierKey(VirtualKey key)
        {
            return key >= VirtualKey.LeftControl; // 0xE0 and above
        }

        /// <summary>
        /// Converts a VirtualKey (HID Usage ID) to Windows Virtual-Key code.
        /// Used for GetAsyncKeyState and other Windows API calls.
        /// </summary>
        /// <param name="key">VirtualKey to convert.</param>
        /// <returns>Windows VK code (0-255).</returns>
        public static int VirtualKeyToWindowsVK(VirtualKey key)
        {
            // Letters A-Z: HID 0x04-0x1D → Windows 0x41-0x5A
            if (key >= VirtualKey.A && key <= VirtualKey.Z)
            {
                return 0x41 + ((byte)key - 0x04);
            }

            // Numbers 1-9: HID 0x1E-0x26 → Windows 0x31-0x39
            if (key >= VirtualKey.D1 && key <= VirtualKey.D9)
            {
                return 0x31 + ((byte)key - 0x1E);
            }

            // Number 0: HID 0x27 → Windows 0x30
            if (key == VirtualKey.D0)
            {
                return 0x30;
            }

            // Function keys F1-F12: HID 0x3A-0x45 → Windows 0x70-0x7B
            if (key >= VirtualKey.F1 && key <= VirtualKey.F12)
            {
                return 0x70 + ((byte)key - 0x3A);
            }

            // Arrow keys: HID Right/Left/Down/Up → Windows Right/Left/Down/Up
            if (key == VirtualKey.Right) return 0x27; // VK_RIGHT
            if (key == VirtualKey.Left) return 0x25;  // VK_LEFT
            if (key == VirtualKey.Down) return 0x28;  // VK_DOWN
            if (key == VirtualKey.Up) return 0x26;    // VK_UP

            // Modifier keys
            if (key == VirtualKey.LeftControl) return 0xA2;  // VK_LCONTROL
            if (key == VirtualKey.RightControl) return 0xA3; // VK_RCONTROL
            if (key == VirtualKey.LeftShift) return 0xA0;    // VK_LSHIFT
            if (key == VirtualKey.RightShift) return 0xA1;   // VK_RSHIFT
            if (key == VirtualKey.LeftAlt) return 0xA4;      // VK_LMENU
            if (key == VirtualKey.RightAlt) return 0xA5;     // VK_RMENU
            if (key == VirtualKey.Windows) return 0x5B;      // VK_LWIN

            // Common keys
            return key switch
            {
                VirtualKey.Enter => 0x0D,      // VK_RETURN
                VirtualKey.Escape => 0x1B,     // VK_ESCAPE
                VirtualKey.Backspace => 0x08,  // VK_BACK
                VirtualKey.Tab => 0x09,        // VK_TAB
                VirtualKey.Space => 0x20,      // VK_SPACE
                VirtualKey.CapsLock => 0x14,   // VK_CAPITAL
                VirtualKey.PrintScreen => 0x2C,// VK_SNAPSHOT
                VirtualKey.ScrollLock => 0x91, // VK_SCROLL
                VirtualKey.Pause => 0x13,      // VK_PAUSE
                VirtualKey.Insert => 0x2D,     // VK_INSERT
                VirtualKey.Home => 0x24,       // VK_HOME
                VirtualKey.PageUp => 0x21,     // VK_PRIOR
                VirtualKey.Delete => 0x2E,     // VK_DELETE
                VirtualKey.End => 0x23,        // VK_END
                VirtualKey.PageDown => 0x22,   // VK_NEXT
                VirtualKey.NumLock => 0x90,    // VK_NUMLOCK

                // Numpad
                VirtualKey.NumpadDivide => 0x6F,    // VK_DIVIDE
                VirtualKey.NumpadMultiply => 0x6A,  // VK_MULTIPLY
                VirtualKey.NumpadMinus => 0x6D,     // VK_SUBTRACT
                VirtualKey.NumpadPlus => 0x6B,      // VK_ADD
                VirtualKey.NumpadEnter => 0x0D,     // VK_RETURN (same as Enter)
                VirtualKey.Numpad1 => 0x61,         // VK_NUMPAD1
                VirtualKey.Numpad2 => 0x62,         // VK_NUMPAD2
                VirtualKey.Numpad3 => 0x63,         // VK_NUMPAD3
                VirtualKey.Numpad4 => 0x64,         // VK_NUMPAD4
                VirtualKey.Numpad5 => 0x65,         // VK_NUMPAD5
                VirtualKey.Numpad6 => 0x66,         // VK_NUMPAD6
                VirtualKey.Numpad7 => 0x67,         // VK_NUMPAD7
                VirtualKey.Numpad8 => 0x68,         // VK_NUMPAD8
                VirtualKey.Numpad9 => 0x69,         // VK_NUMPAD9
                VirtualKey.Numpad0 => 0x60,         // VK_NUMPAD0
                VirtualKey.NumpadPeriod => 0x6E,    // VK_DECIMAL

                // Symbols (approximate - these might not work perfectly with GetAsyncKeyState)
                VirtualKey.Minus => 0xBD,           // VK_OEM_MINUS
                VirtualKey.Equals => 0xBB,          // VK_OEM_PLUS
                VirtualKey.LeftBracket => 0xDB,     // VK_OEM_4
                VirtualKey.RightBracket => 0xDD,    // VK_OEM_6
                VirtualKey.Backslash => 0xDC,       // VK_OEM_5
                VirtualKey.Semicolon => 0xBA,       // VK_OEM_1
                VirtualKey.Apostrophe => 0xDE,      // VK_OEM_7
                VirtualKey.Grave => 0xC0,           // VK_OEM_3
                VirtualKey.Comma => 0xBC,           // VK_OEM_COMMA
                VirtualKey.Period => 0xBE,          // VK_OEM_PERIOD
                VirtualKey.Slash => 0xBF,           // VK_OEM_2

                _ => 0 // Unknown key
            };
        }

        /// <summary>
        /// Converts a character to VirtualKey and determines if Shift is needed.
        /// </summary>
        /// <param name="c">Character to convert.</param>
        /// <returns>Tuple of (VirtualKey, needsShift). Returns (None, false) if character not supported.</returns>
        public static (VirtualKey key, bool needsShift) CharToVirtualKey(char c)
        {
            // Lowercase letters (a-z)
            if (c >= 'a' && c <= 'z')
            {
                return ((VirtualKey)((byte)VirtualKey.A + (c - 'a')), false);
            }

            // Uppercase letters (A-Z) - same keys but need Shift
            if (c >= 'A' && c <= 'Z')
            {
                return ((VirtualKey)((byte)VirtualKey.A + (c - 'A')), true);
            }

            // Numbers and symbols
            return c switch
            {
                // Numbers (top row) - no shift
                '1' => (VirtualKey.D1, false),
                '2' => (VirtualKey.D2, false),
                '3' => (VirtualKey.D3, false),
                '4' => (VirtualKey.D4, false),
                '5' => (VirtualKey.D5, false),
                '6' => (VirtualKey.D6, false),
                '7' => (VirtualKey.D7, false),
                '8' => (VirtualKey.D8, false),
                '9' => (VirtualKey.D9, false),
                '0' => (VirtualKey.D0, false),

                // Shifted number symbols
                '!' => (VirtualKey.D1, true),  // Shift+1
                '@' => (VirtualKey.D2, true),  // Shift+2
                '#' => (VirtualKey.D3, true),  // Shift+3
                '$' => (VirtualKey.D4, true),  // Shift+4
                '%' => (VirtualKey.D5, true),  // Shift+5
                '^' => (VirtualKey.D6, true),  // Shift+6
                '&' => (VirtualKey.D7, true),  // Shift+7
                '*' => (VirtualKey.D8, true),  // Shift+8
                '(' => (VirtualKey.D9, true),  // Shift+9
                ')' => (VirtualKey.D0, true),  // Shift+0

                // Punctuation - no shift
                '-' => (VirtualKey.Minus, false),
                '=' => (VirtualKey.Equals, false),
                '[' => (VirtualKey.LeftBracket, false),
                ']' => (VirtualKey.RightBracket, false),
                '\\' => (VirtualKey.Backslash, false),
                ';' => (VirtualKey.Semicolon, false),
                '\'' => (VirtualKey.Apostrophe, false),
                '`' => (VirtualKey.Grave, false),
                ',' => (VirtualKey.Comma, false),
                '.' => (VirtualKey.Period, false),
                '/' => (VirtualKey.Slash, false),

                // Punctuation - with shift
                '_' => (VirtualKey.Minus, true),       // Shift+-
                '+' => (VirtualKey.Equals, true),      // Shift+=
                '{' => (VirtualKey.LeftBracket, true), // Shift+[
                '}' => (VirtualKey.RightBracket, true),// Shift+]
                '|' => (VirtualKey.Backslash, true),   // Shift+\
                ':' => (VirtualKey.Semicolon, true),   // Shift+;
                '"' => (VirtualKey.Apostrophe, true),  // Shift+'
                '~' => (VirtualKey.Grave, true),       // Shift+`
                '<' => (VirtualKey.Comma, true),       // Shift+,
                '>' => (VirtualKey.Period, true),      // Shift+.
                '?' => (VirtualKey.Slash, true),       // Shift+/

                // Whitespace
                ' ' => (VirtualKey.Space, false),
                '\t' => (VirtualKey.Tab, false),
                '\n' => (VirtualKey.Enter, false),
                '\r' => (VirtualKey.Enter, false),

                // Unsupported character
                _ => (VirtualKey.None, false)
            };
        }
    }
}
