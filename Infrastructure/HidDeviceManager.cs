using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SendSequenceCL.Infrastructure
{
    /// <summary>
    /// Manages HID device discovery and connection lifecycle.
    /// Responsible for finding Tetherscript HID virtual devices by VID/PID.
    /// </summary>
    internal class HidDeviceManager : IDisposable
    {
        private SafeFileHandle? _deviceHandle;
        private readonly ushort _vendorId;
        private readonly ushort _productId;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HidDeviceManager"/> class.
        /// </summary>
        /// <param name="vendorId">Vendor ID of target HID device.</param>
        /// <param name="productId">Product ID of target HID device.</param>
        public HidDeviceManager(ushort vendorId, ushort productId)
        {
            _vendorId = vendorId;
            _productId = productId;
        }

        /// <summary>
        /// Gets a value indicating whether device is currently connected.
        /// </summary>
        public bool IsConnected => _deviceHandle != null && !_deviceHandle.IsInvalid && !_deviceHandle.IsClosed;

        /// <summary>
        /// Gets the device handle for communication.
        /// </summary>
        public SafeFileHandle? DeviceHandle => _deviceHandle;

        /// <summary>
        /// Connects to the HID device by enumerating all HID devices and finding match by VID/PID.
        /// </summary>
        /// <exception cref="DriverNotFoundException">Thrown if no matching device found.</exception>
        /// <exception cref="DriverCommunicationException">Thrown if device enumeration fails.</exception>
        public void Connect()
        {
            if (IsConnected)
                return;

            try
            {
                // Get HID class GUID
                NativeMethods.HidD_GetHidGuid(out Guid hidGuid);

                // Get device information set for all HID devices
                IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(
                    ref hidGuid,
                    IntPtr.Zero,
                    IntPtr.Zero,
                    NativeMethods.DiGetClassFlags.DIGCF_PRESENT | NativeMethods.DiGetClassFlags.DIGCF_DEVICEINTERFACE);

                if (deviceInfoSet == NativeMethods.INVALID_HANDLE_VALUE)
                {
                    throw new DriverCommunicationException("Failed to get HID device information set.");
                }

                try
                {
                    // Enumerate devices
                    uint memberIndex = 0;
                    while (true)
                    {
                        NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
                        deviceInterfaceData.cbSize = (uint)Marshal.SizeOf(deviceInterfaceData);

                        bool found = NativeMethods.SetupDiEnumDeviceInterfaces(
                            deviceInfoSet,
                            IntPtr.Zero,
                            ref hidGuid,
                            memberIndex,
                            ref deviceInterfaceData);

                        if (!found)
                            break; // No more devices

                        // Try to open this device and check VID/PID
                        if (TryOpenDevice(deviceInfoSet, ref deviceInterfaceData))
                            return; // Successfully connected

                        memberIndex++;
                    }

                    throw new DriverNotFoundException($"HID device with VID=0x{_vendorId:X4} PID=0x{_productId:X4} not found.");
                }
                finally
                {
                    NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
                }
            }
            catch (SendSequenceException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new DriverCommunicationException("Failed to connect to HID device.", ex);
            }
        }

        /// <summary>
        /// Attempts to open a device and verify its VID/PID.
        /// </summary>
        private bool TryOpenDevice(IntPtr deviceInfoSet, ref NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInterfaceData)
        {
            NativeMethods.SP_DEVINFO_DATA deviceInfoData = new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize = (uint)Marshal.SizeOf(deviceInfoData);

            // Get required size for detail data
            NativeMethods.SetupDiGetDeviceInterfaceDetail(
                deviceInfoSet,
                ref deviceInterfaceData,
                IntPtr.Zero,
                0,
                out uint requiredSize,
                ref deviceInfoData);

            if (Marshal.GetLastWin32Error() != 122) // ERROR_INSUFFICIENT_BUFFER
                return false;

            // Allocate buffer for detail data
            IntPtr detailDataBuffer = Marshal.AllocHGlobal((int)requiredSize);
            try
            {
                // Set size field (platform dependent: 8 for x64, 6 for x86 includes size + padding)
                Marshal.WriteInt32(detailDataBuffer, IntPtr.Size == 8 ? 8 : 6);

                // Get device path
                if (!NativeMethods.SetupDiGetDeviceInterfaceDetail(
                    deviceInfoSet,
                    ref deviceInterfaceData,
                    detailDataBuffer,
                    requiredSize,
                    out _,
                    ref deviceInfoData))
                {
                    return false;
                }

                // Extract device path (skip size field)
                IntPtr pathPtr = new IntPtr(detailDataBuffer.ToInt64() + 4);
                string? devicePath = Marshal.PtrToStringAuto(pathPtr);
                if (devicePath == null)
                    return false;

                // Try to open device with ReadWrite access first
                SafeFileHandle handle = NativeMethods.CreateFile(
                    devicePath,
                    System.IO.FileAccess.ReadWrite,
                    System.IO.FileShare.ReadWrite,
                    IntPtr.Zero,
                    System.IO.FileMode.Open,
                    0,
                    IntPtr.Zero);

                // If ReadWrite failed, try opening with no access (only for HidD_SetFeature)
                // This is needed when device is already opened by another process (e.g., ControlMyJoystick)
                if (handle.IsInvalid)
                {
                    handle.Dispose();
                    handle = NativeMethods.CreateFile(
                        devicePath,
                        0, // No read/write access - only for sending feature reports
                        System.IO.FileShare.ReadWrite,
                        IntPtr.Zero,
                        System.IO.FileMode.Open,
                        0,
                        IntPtr.Zero);

                    if (handle.IsInvalid)
                    {
                        handle.Dispose();
                        return false;
                    }
                }

                // Check VID/PID
                NativeMethods.HIDD_ATTRIBUTES attributes = new NativeMethods.HIDD_ATTRIBUTES();
                attributes.Size = Marshal.SizeOf(attributes);

                if (NativeMethods.HidD_GetAttributes(handle, ref attributes) &&
                    attributes.VendorID == _vendorId &&
                    attributes.ProductID == _productId)
                {
                    _deviceHandle = handle;
                    return true;
                }

                handle.Dispose();
                return false;
            }
            finally
            {
                Marshal.FreeHGlobal(detailDataBuffer);
            }
        }

        /// <summary>
        /// Disconnects from the HID device and releases resources.
        /// </summary>
        public void Disconnect()
        {
            _deviceHandle?.Dispose();
            _deviceHandle = null;
        }

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                Disconnect();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }
    }
}
