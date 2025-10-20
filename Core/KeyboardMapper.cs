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
