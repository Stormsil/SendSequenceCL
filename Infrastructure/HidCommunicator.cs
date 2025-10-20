using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SendSequenceCL.Infrastructure
{
    /// <summary>
    /// Handles low-level communication with HID driver via feature reports.
    /// </summary>
    internal class HidCommunicator
    {
        private readonly HidDeviceManager _deviceManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidCommunicator"/> class.
        /// </summary>
        /// <param name="deviceManager">Device manager providing access to HID device handle.</param>
        public HidCommunicator(HidDeviceManager deviceManager)
        {
            _deviceManager = deviceManager ?? throw new ArgumentNullException(nameof(deviceManager));
        }

        /// <summary>
        /// Sends mouse absolute positioning data to HID driver.
        /// </summary>
        /// <param name="x">Absolute X coordinate (0-32767).</param>
        /// <param name="y">Absolute Y coordinate (0-32767).</param>
        /// <param name="buttons">Button state flags (bit 0=Left, bit 1=Right, bit 2=Middle).</param>
        /// <exception cref="DriverNotFoundException">Thrown if device not connected.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if send operation fails.</exception>
        public void SendMouseAbsolute(ushort x, ushort y, byte buttons)
        {
            EnsureConnected();

            SetFeatureMouseAbs data = new SetFeatureMouseAbs
            {
                ReportID = DriverConstants.ReportId,
                CommandCode = DriverConstants.CommandCodeSend,
                Buttons = buttons,
                X = x,
                Y = y
            };

            SendFeatureReport(data);
        }

        /// <summary>
        /// Sends keyboard key data to HID driver.
        /// </summary>
        /// <param name="modifier">Modifier key flags (Ctrl/Shift/Alt/Win bit flags).</param>
        /// <param name="key0">Key slot 0 (HID Usage ID).</param>
        /// <param name="key1">Key slot 1 (HID Usage ID).</param>
        /// <param name="key2">Key slot 2 (HID Usage ID).</param>
        /// <param name="key3">Key slot 3 (HID Usage ID).</param>
        /// <param name="key4">Key slot 4 (HID Usage ID).</param>
        /// <param name="key5">Key slot 5 (HID Usage ID).</param>
        /// <exception cref="DriverNotFoundException">Thrown if device not connected.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if send operation fails.</exception>
        public void SendKeyboard(byte modifier, byte key0, byte key1 = 0, byte key2 = 0, byte key3 = 0, byte key4 = 0, byte key5 = 0)
        {
            EnsureConnected();

            SetFeatureKeyboard data = new SetFeatureKeyboard
            {
                ReportID = DriverConstants.ReportId,
                CommandCode = DriverConstants.CommandCodeSend,
                Timeout = DriverConstants.KeyboardPingTimeout / 5,
                Modifier = modifier,
                Padding = 0,
                Key0 = key0,
                Key1 = key1,
                Key2 = key2,
                Key3 = key3,
                Key4 = key4,
                Key5 = key5
            };

            SendFeatureReport(data);
        }

        /// <summary>
        /// Sends ping (keep-alive) to keyboard driver to prevent stuck keys.
        /// </summary>
        /// <exception cref="DriverNotFoundException">Thrown if device not connected.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if send operation fails.</exception>
        public void SendKeyboardPing()
        {
            EnsureConnected();

            SetFeatureKeyboard data = new SetFeatureKeyboard
            {
                ReportID = DriverConstants.ReportId,
                CommandCode = DriverConstants.CommandCodePing,
                Timeout = DriverConstants.KeyboardPingTimeout / 5,
                Modifier = 0,
                Padding = 0,
                Key0 = 0,
                Key1 = 0,
                Key2 = 0,
                Key3 = 0,
                Key4 = 0,
                Key5 = 0
            };

            SendFeatureReport(data);
        }

        /// <summary>
        /// Ensures device is connected, throws if not.
        /// </summary>
        private void EnsureConnected()
        {
            if (!_deviceManager.IsConnected)
            {
                throw new DriverNotFoundException("HID device not connected. Call Connect() first.");
            }
        }

        /// <summary>
        /// Sends a feature report structure to the HID device.
        /// </summary>
        private void SendFeatureReport<T>(T report) where T : struct
        {
            SafeFileHandle? handle = _deviceManager.DeviceHandle;
            if (handle == null || handle.IsInvalid || handle.IsClosed)
            {
                throw new DriverNotFoundException("Invalid device handle.");
            }

            int size = Marshal.SizeOf(report);
            byte[] buffer = new byte[size];

            // Convert struct to byte array
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(report, ptr, false);
                Marshal.Copy(ptr, buffer, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            // Send feature report
            bool success = NativeMethods.HidD_SetFeature(handle, buffer, (uint)buffer.Length);
            if (!success)
            {
                int error = Marshal.GetLastWin32Error();
                throw new DriverCommunicationException($"Failed to send HID feature report. Win32 Error: {error}");
            }
        }
    }
}
