using System;
using System.Threading;
using SendSequenceCL.Core;
using SendSequenceCL.Infrastructure;

namespace SendSequenceCL
{
    /// <summary>
    /// Static facade providing access to virtual mouse and keyboard automation.
    /// Thread-safe singleton initialization with lazy loading.
    /// </summary>
    public static class Input
    {
        private static readonly Lazy<HidDeviceManager> _mouseDeviceManager = new Lazy<HidDeviceManager>(() =>
        {
            var manager = new HidDeviceManager(DriverConstants.VendorId, DriverConstants.ProductIdMouseAbsolute);
            manager.Connect();
            return manager;
        });

        private static readonly Lazy<HidDeviceManager> _keyboardDeviceManager = new Lazy<HidDeviceManager>(() =>
        {
            var manager = new HidDeviceManager(DriverConstants.VendorId, DriverConstants.ProductIdKeyboard);
            manager.Connect();
            return manager;
        });

        private static readonly Lazy<HidDeviceManager> _joystickDeviceManager = new Lazy<HidDeviceManager>(() =>
        {
            var manager = new HidDeviceManager(DriverConstants.VendorId, DriverConstants.ProductIdJoystick);
            manager.Connect();
            return manager;
        });

        private static readonly Lazy<HidDeviceManager> _mouseRelativeDeviceManager = new Lazy<HidDeviceManager>(() =>
        {
            var manager = new HidDeviceManager(DriverConstants.VendorId, DriverConstants.ProductIdMouseRelative);
            manager.Connect();
            return manager;
        });

        private static readonly Lazy<HidCommunicator> _mouseCommunicator = new Lazy<HidCommunicator>(() =>
            new HidCommunicator(_mouseDeviceManager.Value));

        private static readonly Lazy<HidCommunicator> _keyboardCommunicator = new Lazy<HidCommunicator>(() =>
            new HidCommunicator(_keyboardDeviceManager.Value));

        private static readonly Lazy<HidCommunicator> _joystickCommunicator = new Lazy<HidCommunicator>(() =>
            new HidCommunicator(_joystickDeviceManager.Value));

        private static readonly Lazy<HidCommunicator> _mouseRelativeCommunicator = new Lazy<HidCommunicator>(() =>
            new HidCommunicator(_mouseRelativeDeviceManager.Value));

        private static readonly Lazy<IVirtualKeyboard> _keyboard = new Lazy<IVirtualKeyboard>(() =>
            new KeyboardController(_keyboardDeviceManager.Value, _keyboardCommunicator.Value));

        private static readonly Lazy<IVirtualMouse> _mouse = new Lazy<IVirtualMouse>(() =>
            new MouseController(_mouseDeviceManager.Value, _mouseCommunicator.Value, _joystickDeviceManager.Value, _joystickCommunicator.Value, _mouseRelativeDeviceManager.Value, _mouseRelativeCommunicator.Value, _keyboard));

        /// <summary>
        /// Gets the virtual mouse automation interface.
        /// First access will initialize connection to HID mouse driver.
        /// </summary>
        /// <exception cref="DriverNotFoundException">Thrown if mouse HID driver not found on first access.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if connection to driver fails on first access.</exception>
        public static IVirtualMouse Mouse => _mouse.Value;

        /// <summary>
        /// Gets the virtual keyboard automation interface.
        /// First access will initialize connection to HID keyboard driver.
        /// </summary>
        /// <exception cref="DriverNotFoundException">Thrown if keyboard HID driver not found on first access.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if connection to driver fails on first access.</exception>
        public static IVirtualKeyboard Keyboard => _keyboard.Value;

        /// <summary>
        /// Utility method to pause execution for specified duration.
        /// Equivalent to Thread.Sleep() but with clearer intent in automation context.
        /// </summary>
        /// <param name="milliseconds">Duration to wait in milliseconds.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if milliseconds is negative.</exception>
        public static void Wait(int milliseconds)
        {
            if (milliseconds < 0)
                throw new ArgumentOutOfRangeException(nameof(milliseconds), "Wait duration cannot be negative.");

            Thread.Sleep(milliseconds);
        }

        /// <summary>
        /// Releases all resources used by Input class.
        /// Should be called when application is shutting down to properly close HID device handles.
        /// </summary>
        public static void Dispose()
        {
            if (_mouseDeviceManager.IsValueCreated)
                _mouseDeviceManager.Value.Dispose();

            if (_keyboardDeviceManager.IsValueCreated)
                _keyboardDeviceManager.Value.Dispose();

            if (_joystickDeviceManager.IsValueCreated)
                _joystickDeviceManager.Value.Dispose();

            if (_mouseRelativeDeviceManager.IsValueCreated)
                _mouseRelativeDeviceManager.Value.Dispose();
        }
    }
}
