using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SendSequenceCL.Infrastructure;

namespace SendSequenceCL.Core
{
    /// <summary>
    /// Implements virtual keyboard automation operations.
    /// </summary>
    internal class KeyboardController : IVirtualKeyboard
    {
        private readonly HidDeviceManager _deviceManager;
        private readonly HidCommunicator _communicator;
        private readonly HashSet<VirtualKey> _pressedKeys = new HashSet<VirtualKey>();
        private byte _currentModifiers = 0;

        public KeyboardController(HidDeviceManager deviceManager, HidCommunicator communicator)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
            _communicator = communicator ?? throw new ArgumentNullException(nameof(communicator));
        }

        /// <inheritdoc/>
        public void TypeText(string text)
        {
            if (text == null)
                throw new ArgumentNullException(nameof(text));

            foreach (char c in text)
            {
                var (key, needsShift) = KeyboardMapper.CharToVirtualKey(c);

                if (key == VirtualKey.None)
                {
                    // Skip unsupported characters
                    continue;
                }

                // Press key with Shift if needed
                if (needsShift)
                {
                    // Hold Shift
                    byte shiftModifier = KeyboardMapper.VirtualKeyToModifierFlag(VirtualKey.LeftShift);
                    byte hidCode = KeyboardMapper.VirtualKeyToHidCode(key);
                    _communicator.SendKeyboard((byte)(shiftModifier), hidCode);
                }
                else
                {
                    // Press without modifiers
                    byte hidCode = KeyboardMapper.VirtualKeyToHidCode(key);
                    _communicator.SendKeyboard(0, hidCode);
                }

                Thread.Sleep(Configuration.KeystrokeDelay);

                // Release key
                _communicator.SendKeyboard(0, 0);
                Thread.Sleep(Configuration.KeystrokeDelay / 2);
            }
        }

        /// <inheritdoc/>
        public void KeyPress(VirtualKey key)
        {
            if (KeyboardMapper.IsModifierKey(key))
            {
                throw new InvalidKeyCodeException($"Cannot use KeyPress with modifier key {key}. Use Chord() instead.");
            }

            byte hidCode = KeyboardMapper.VirtualKeyToHidCode(key);

            // Key down
            _communicator.SendKeyboard(_currentModifiers, hidCode);
            Thread.Sleep(Configuration.KeystrokeDelay);

            // Key up
            _communicator.SendKeyboard(_currentModifiers, 0);
        }

        /// <inheritdoc/>
        public void Chord(VirtualKey modifier, VirtualKey key)
        {
            Chord(new[] { modifier }, key);
        }

        /// <inheritdoc/>
        public void Chord(IEnumerable<VirtualKey> modifiers, VirtualKey key)
        {
            if (modifiers == null)
                throw new ArgumentNullException(nameof(modifiers));

            var modifierList = modifiers.ToList();
            if (modifierList.Count == 0)
                throw new ArgumentException("At least one modifier key required.", nameof(modifiers));

            // Calculate combined modifier flags
            byte modifierFlags = 0;
            foreach (var mod in modifierList)
            {
                if (!KeyboardMapper.IsModifierKey(mod))
                {
                    throw new InvalidKeyCodeException($"Key {mod} is not a modifier key.");
                }
                modifierFlags |= KeyboardMapper.VirtualKeyToModifierFlag(mod);
            }

            byte hidCode = KeyboardMapper.VirtualKeyToHidCode(key);

            // Press modifiers down
            _communicator.SendKeyboard(modifierFlags, 0);
            Thread.Sleep(Configuration.ModifierHoldDuration);

            // Press main key
            _communicator.SendKeyboard(modifierFlags, hidCode);
            Thread.Sleep(Configuration.KeystrokeDelay);

            // Release main key
            _communicator.SendKeyboard(modifierFlags, 0);
            Thread.Sleep(Configuration.KeystrokeDelay / 2);

            // Release modifiers
            _communicator.SendKeyboard(0, 0);
        }

        /// <inheritdoc/>
        public void KeyDown(VirtualKey key)
        {
            if (KeyboardMapper.IsModifierKey(key))
            {
                // Set modifier flag
                byte flag = KeyboardMapper.VirtualKeyToModifierFlag(key);
                _currentModifiers |= flag;
                _communicator.SendKeyboard(_currentModifiers, 0);
            }
            else
            {
                // Add to pressed keys (support up to 6 simultaneous keys)
                if (_pressedKeys.Count >= 6)
                {
                    throw new InvalidOperationException("Maximum 6 keys can be pressed simultaneously.");
                }

                _pressedKeys.Add(key);
                SendCurrentKeyState();
            }
        }

        /// <inheritdoc/>
        public void KeyUp(VirtualKey key)
        {
            if (KeyboardMapper.IsModifierKey(key))
            {
                // Clear modifier flag
                byte flag = KeyboardMapper.VirtualKeyToModifierFlag(key);
                _currentModifiers &= (byte)~flag;
                _communicator.SendKeyboard(_currentModifiers, 0);
            }
            else
            {
                // Remove from pressed keys
                _pressedKeys.Remove(key);
                SendCurrentKeyState();
            }
        }

        /// <summary>
        /// Sends current state of all pressed keys to driver.
        /// </summary>
        private void SendCurrentKeyState()
        {
            byte[] keys = new byte[6];
            int index = 0;

            foreach (var key in _pressedKeys)
            {
                if (index >= 6)
                    break;
                keys[index++] = KeyboardMapper.VirtualKeyToHidCode(key);
            }

            _communicator.SendKeyboard(_currentModifiers, keys[0], keys[1], keys[2], keys[3], keys[4], keys[5]);
        }
    }
}
