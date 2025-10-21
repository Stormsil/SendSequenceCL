using System;
using System.Collections.Generic;

namespace SendSequenceCL
{
    /// <summary>
    /// Defines contract for virtual keyboard automation operations.
    /// </summary>
    public interface IVirtualKeyboard
    {
        /// <summary>
        /// Types arbitrary text string with human-like delays between keystrokes.
        /// Automatically handles shift for uppercase letters and symbols.
        /// </summary>
        /// <param name="text">Text to type. Supports letters, numbers, symbols, spaces, newlines.</param>
        /// <exception cref="ArgumentNullException">Thrown if text is null.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void TypeText(string text);

        /// <summary>
        /// Presses and releases a single key (atomic operation).
        /// </summary>
        /// <param name="key">Key to press.</param>
        /// <exception cref="InvalidKeyCodeException">Thrown if key enum value not supported.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void KeyPress(VirtualKey key);

        /// <summary>
        /// Executes keyboard shortcut with one modifier key and one regular key.
        /// Example: Ctrl+C, Alt+F4, Win+R
        /// </summary>
        /// <param name="modifier">Modifier key (Ctrl, Shift, Alt, Windows).</param>
        /// <param name="key">Regular key to press while modifier held.</param>
        /// <exception cref="InvalidKeyCodeException">Thrown if key enum value not supported.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Chord(VirtualKey modifier, VirtualKey key);

        /// <summary>
        /// Executes keyboard shortcut with multiple modifier keys and one regular key.
        /// Example: Ctrl+Shift+Esc, Shift+Win+S
        /// </summary>
        /// <param name="modifiers">Collection of modifier keys (Ctrl, Shift, Alt, Windows).</param>
        /// <param name="key">Regular key to press while modifiers held.</param>
        /// <exception cref="ArgumentNullException">Thrown if modifiers is null.</exception>
        /// <exception cref="ArgumentException">Thrown if modifiers is empty.</exception>
        /// <exception cref="InvalidKeyCodeException">Thrown if any key enum value not supported.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key);

        /// <summary>
        /// Presses key down without releasing (manual control).
        /// Must be paired with <see cref="KeyUp"/> to avoid stuck keys.
        /// </summary>
        /// <param name="key">Key to press down.</param>
        /// <exception cref="InvalidKeyCodeException">Thrown if key enum value not supported.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void KeyDown(VirtualKey key);

        /// <summary>
        /// Releases previously pressed key.
        /// Safe to call even if key not currently pressed.
        /// </summary>
        /// <param name="key">Key to release.</param>
        /// <exception cref="InvalidKeyCodeException">Thrown if key enum value not supported.</exception>
        /// <exception cref="DriverNotFoundException">Thrown if HID driver not found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if communication with driver fails.</exception>
        void KeyUp(VirtualKey key);

        /// <summary>
        /// Checks if the specified key is currently pressed down.
        /// Uses Windows API to query the physical keyboard state.
        /// </summary>
        /// <param name="key">The key to check.</param>
        /// <returns>True if the key is currently pressed down, false otherwise.</returns>
        bool IsKeyDown(VirtualKey key);

        /// <summary>
        /// Types text with maximum humanization: random delays, typo simulation, word pauses.
        /// Uses Configuration ranges for delays and Configuration.TypoChance for typos.
        /// </summary>
        /// <param name="text">Text to type.</param>
        /// <exception cref="ArgumentNullException">Thrown if text is null.</exception>
        void TypeTextHuman(string text);
    }
}
