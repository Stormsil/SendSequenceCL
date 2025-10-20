using System;

namespace SendSequenceCL
{
    /// <summary>
    /// Represents virtual key codes based on HID Usage Table for keyboard input.
    /// Values correspond to USB HID Usage IDs for keyboard/keypad page (0x07).
    /// </summary>
    public enum VirtualKey : byte
    {
        /// <summary>No key pressed.</summary>
        None = 0,

        // Letters (A-Z): Usage IDs 0x04-0x1D
        /// <summary>Letter A key.</summary>
        A = 0x04,
        /// <summary>Letter B key.</summary>
        B = 0x05,
        /// <summary>Letter C key.</summary>
        C = 0x06,
        /// <summary>Letter D key.</summary>
        D = 0x07,
        /// <summary>Letter E key.</summary>
        E = 0x08,
        /// <summary>Letter F key.</summary>
        F = 0x09,
        /// <summary>Letter G key.</summary>
        G = 0x0A,
        /// <summary>Letter H key.</summary>
        H = 0x0B,
        /// <summary>Letter I key.</summary>
        I = 0x0C,
        /// <summary>Letter J key.</summary>
        J = 0x0D,
        /// <summary>Letter K key.</summary>
        K = 0x0E,
        /// <summary>Letter L key.</summary>
        L = 0x0F,
        /// <summary>Letter M key.</summary>
        M = 0x10,
        /// <summary>Letter N key.</summary>
        N = 0x11,
        /// <summary>Letter O key.</summary>
        O = 0x12,
        /// <summary>Letter P key.</summary>
        P = 0x13,
        /// <summary>Letter Q key.</summary>
        Q = 0x14,
        /// <summary>Letter R key.</summary>
        R = 0x15,
        /// <summary>Letter S key.</summary>
        S = 0x16,
        /// <summary>Letter T key.</summary>
        T = 0x17,
        /// <summary>Letter U key.</summary>
        U = 0x18,
        /// <summary>Letter V key.</summary>
        V = 0x19,
        /// <summary>Letter W key.</summary>
        W = 0x1A,
        /// <summary>Letter X key.</summary>
        X = 0x1B,
        /// <summary>Letter Y key.</summary>
        Y = 0x1C,
        /// <summary>Letter Z key.</summary>
        Z = 0x1D,

        // Numbers (1-0): Usage IDs 0x1E-0x27
        /// <summary>Number 1 key (main keyboard).</summary>
        D1 = 0x1E,
        /// <summary>Number 2 key (main keyboard).</summary>
        D2 = 0x1F,
        /// <summary>Number 3 key (main keyboard).</summary>
        D3 = 0x20,
        /// <summary>Number 4 key (main keyboard).</summary>
        D4 = 0x21,
        /// <summary>Number 5 key (main keyboard).</summary>
        D5 = 0x22,
        /// <summary>Number 6 key (main keyboard).</summary>
        D6 = 0x23,
        /// <summary>Number 7 key (main keyboard).</summary>
        D7 = 0x24,
        /// <summary>Number 8 key (main keyboard).</summary>
        D8 = 0x25,
        /// <summary>Number 9 key (main keyboard).</summary>
        D9 = 0x26,
        /// <summary>Number 0 key (main keyboard).</summary>
        D0 = 0x27,

        // Control keys
        /// <summary>Enter/Return key.</summary>
        Enter = 0x28,
        /// <summary>Escape key.</summary>
        Escape = 0x29,
        /// <summary>Backspace key (delete character to left).</summary>
        Backspace = 0x2A,
        /// <summary>Tab key.</summary>
        Tab = 0x2B,
        /// <summary>Spacebar key.</summary>
        Space = 0x2C,

        // Symbols
        /// <summary>Minus/Underscore key (- _).</summary>
        Minus = 0x2D,
        /// <summary>Equals/Plus key (= +).</summary>
        Equals = 0x2E,
        /// <summary>Left bracket key ([ {).</summary>
        LeftBracket = 0x2F,
        /// <summary>Right bracket key (] }).</summary>
        RightBracket = 0x30,
        /// <summary>Backslash/Pipe key (\ |).</summary>
        Backslash = 0x31,
        /// <summary>Semicolon/Colon key (; :).</summary>
        Semicolon = 0x33,
        /// <summary>Apostrophe/Quote key (' ").</summary>
        Apostrophe = 0x34,
        /// <summary>Grave/Tilde key (` ~).</summary>
        Grave = 0x35,
        /// <summary>Comma/Less-than key (, &lt;).</summary>
        Comma = 0x36,
        /// <summary>Period/Greater-than key (. &gt;).</summary>
        Period = 0x37,
        /// <summary>Slash/Question key (/ ?).</summary>
        Slash = 0x38,

        // Lock keys
        /// <summary>Caps Lock key.</summary>
        CapsLock = 0x39,

        // Function keys (F1-F12)
        /// <summary>F1 function key.</summary>
        F1 = 0x3A,
        /// <summary>F2 function key.</summary>
        F2 = 0x3B,
        /// <summary>F3 function key.</summary>
        F3 = 0x3C,
        /// <summary>F4 function key.</summary>
        F4 = 0x3D,
        /// <summary>F5 function key.</summary>
        F5 = 0x3E,
        /// <summary>F6 function key.</summary>
        F6 = 0x3F,
        /// <summary>F7 function key.</summary>
        F7 = 0x40,
        /// <summary>F8 function key.</summary>
        F8 = 0x41,
        /// <summary>F9 function key.</summary>
        F9 = 0x42,
        /// <summary>F10 function key.</summary>
        F10 = 0x43,
        /// <summary>F11 function key.</summary>
        F11 = 0x44,
        /// <summary>F12 function key.</summary>
        F12 = 0x45,

        // System keys
        /// <summary>Print Screen key.</summary>
        PrintScreen = 0x46,
        /// <summary>Scroll Lock key.</summary>
        ScrollLock = 0x47,
        /// <summary>Pause/Break key.</summary>
        Pause = 0x48,
        /// <summary>Insert key.</summary>
        Insert = 0x49,
        /// <summary>Home key.</summary>
        Home = 0x4A,
        /// <summary>Page Up key.</summary>
        PageUp = 0x4B,
        /// <summary>Delete key (delete character to right).</summary>
        Delete = 0x4C,
        /// <summary>End key.</summary>
        End = 0x4D,
        /// <summary>Page Down key.</summary>
        PageDown = 0x4E,

        // Arrow keys
        /// <summary>Right arrow key.</summary>
        Right = 0x4F,
        /// <summary>Left arrow key.</summary>
        Left = 0x50,
        /// <summary>Down arrow key.</summary>
        Down = 0x51,
        /// <summary>Up arrow key.</summary>
        Up = 0x52,

        // Numpad keys
        /// <summary>Num Lock key.</summary>
        NumLock = 0x53,
        /// <summary>Numpad divide key (/).</summary>
        NumpadDivide = 0x54,
        /// <summary>Numpad multiply key (*).</summary>
        NumpadMultiply = 0x55,
        /// <summary>Numpad minus key (-).</summary>
        NumpadMinus = 0x56,
        /// <summary>Numpad plus key (+).</summary>
        NumpadPlus = 0x57,
        /// <summary>Numpad Enter key.</summary>
        NumpadEnter = 0x58,
        /// <summary>Numpad 1 key.</summary>
        Numpad1 = 0x59,
        /// <summary>Numpad 2 key.</summary>
        Numpad2 = 0x5A,
        /// <summary>Numpad 3 key.</summary>
        Numpad3 = 0x5B,
        /// <summary>Numpad 4 key.</summary>
        Numpad4 = 0x5C,
        /// <summary>Numpad 5 key.</summary>
        Numpad5 = 0x5D,
        /// <summary>Numpad 6 key.</summary>
        Numpad6 = 0x5E,
        /// <summary>Numpad 7 key.</summary>
        Numpad7 = 0x5F,
        /// <summary>Numpad 8 key.</summary>
        Numpad8 = 0x60,
        /// <summary>Numpad 9 key.</summary>
        Numpad9 = 0x61,
        /// <summary>Numpad 0 key.</summary>
        Numpad0 = 0x62,
        /// <summary>Numpad decimal/period key (.).</summary>
        NumpadPeriod = 0x63,

        // Extended function keys (F13-F24)
        /// <summary>F13 function key.</summary>
        F13 = 0x68,
        /// <summary>F14 function key.</summary>
        F14 = 0x69,
        /// <summary>F15 function key.</summary>
        F15 = 0x6A,
        /// <summary>F16 function key.</summary>
        F16 = 0x6B,
        /// <summary>F17 function key.</summary>
        F17 = 0x6C,
        /// <summary>F18 function key.</summary>
        F18 = 0x6D,
        /// <summary>F19 function key.</summary>
        F19 = 0x6E,
        /// <summary>F20 function key.</summary>
        F20 = 0x6F,
        /// <summary>F21 function key.</summary>
        F21 = 0x70,
        /// <summary>F22 function key.</summary>
        F22 = 0x71,
        /// <summary>F23 function key.</summary>
        F23 = 0x72,
        /// <summary>F24 function key.</summary>
        F24 = 0x73,

        // Modifier keys (used in modifier byte, not in key slots)
        /// <summary>Left Control modifier key.</summary>
        LeftControl = 0xE0,
        /// <summary>Left Shift modifier key.</summary>
        LeftShift = 0xE1,
        /// <summary>Left Alt modifier key.</summary>
        LeftAlt = 0xE2,
        /// <summary>Left Windows/GUI modifier key.</summary>
        Windows = 0xE3,
        /// <summary>Right Control modifier key.</summary>
        RightControl = 0xE4,
        /// <summary>Right Shift modifier key.</summary>
        RightShift = 0xE5,
        /// <summary>Right Alt (AltGr) modifier key.</summary>
        RightAlt = 0xE6
    }
}
