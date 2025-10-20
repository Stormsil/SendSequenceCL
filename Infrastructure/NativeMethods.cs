using System;
using System.Runtime.InteropServices;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace SendSequenceCL.Infrastructure
{
    /// <summary>
    /// P/Invoke declarations for Windows API calls (SetupAPI, HID, Kernel32).
    /// </summary>
    internal static class NativeMethods
    {
        // Constants
        public const uint FILE_FLAG_OVERLAPPED = 0x40000000;
        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        /// <summary>
        /// Flags for SetupDiGetClassDevs function.
        /// </summary>
        [Flags]
        public enum DiGetClassFlags : uint
        {
            /// <summary>Return only devices that are currently present in the system.</summary>
            DIGCF_PRESENT = 0x00000002,

            /// <summary>Return devices that support device interfaces for the specified device interface classes.</summary>
            DIGCF_DEVICEINTERFACE = 0x00000010
        }

        /// <summary>
        /// Device interface data structure for SetupAPI.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVICE_INTERFACE_DATA
        {
            public uint cbSize;
            public Guid interfaceClassGuid;
            public int flags;
            private IntPtr reserved;
        }

        /// <summary>
        /// Device interface detail data structure for SetupAPI (variable length).
        /// Only the size field is used; actual path retrieved via pointer manipulation.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        public struct SP_DEVICE_INTERFACE_DETAIL_DATA
        {
            public uint cbSize;
            public char devicePath;
        }

        /// <summary>
        /// Device info data structure for SetupAPI.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA
        {
            public uint cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        /// <summary>
        /// HID device attributes structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct HIDD_ATTRIBUTES
        {
            public int Size;
            public ushort VendorID;
            public ushort ProductID;
            public ushort VersionNumber;
        }

        // ===== SetupAPI Functions (setupapi.dll) =====

        /// <summary>
        /// Returns a device information set containing all devices of a specified class.
        /// </summary>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr SetupDiGetClassDevs(
            ref Guid classGuid,
            IntPtr enumerator,
            IntPtr hwndParent,
            DiGetClassFlags flags);

        /// <summary>
        /// Enumerates device interfaces in a device information set.
        /// </summary>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr deviceInfoSet,
            IntPtr deviceInfoData,
            ref Guid interfaceClassGuid,
            uint memberIndex,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData);

        /// <summary>
        /// Retrieves details about a device interface.
        /// </summary>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet,
            ref SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            uint deviceInterfaceDetailDataSize,
            out uint requiredSize,
            ref SP_DEVINFO_DATA deviceInfoData);

        /// <summary>
        /// Destroys a device information set and frees associated memory.
        /// </summary>
        [DllImport("setupapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetupDiDestroyDeviceInfoList(IntPtr deviceInfoSet);

        // ===== HID Functions (hid.dll) =====

        /// <summary>
        /// Returns the HID class GUID.
        /// </summary>
        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern void HidD_GetHidGuid(out Guid hidGuid);

        /// <summary>
        /// Retrieves attributes of a HID device (vendor ID, product ID, version).
        /// </summary>
        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_GetAttributes(
            SafeFileHandle hidDeviceObject,
            ref HIDD_ATTRIBUTES attributes);

        /// <summary>
        /// Sends a feature report to a HID device.
        /// </summary>
        [DllImport("hid.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool HidD_SetFeature(
            SafeFileHandle hidDeviceObject,
            byte[] reportBuffer,
            uint reportBufferLength);

        // ===== Kernel32 Functions (kernel32.dll) =====

        /// <summary>
        /// Creates or opens a file or device.
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern SafeFileHandle CreateFile(
            string fileName,
            FileAccess fileAccess,
            FileShare fileShare,
            IntPtr securityAttributes,
            FileMode creationDisposition,
            uint flagsAndAttributes,
            IntPtr templateFile);

        // ===== User32 Functions (user32.dll) - for screen resolution =====

        /// <summary>
        /// Retrieves information about a display monitor.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromPoint(POINT pt, uint dwFlags);

        /// <summary>
        /// Retrieves information about a display monitor.
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        /// <summary>
        /// Retrieves the dimensions of the bounding rectangle of the specified window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        /// <summary>
        /// Retrieves a handle to the desktop window.
        /// </summary>
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// Retrieves the current position of the mouse cursor.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetCursorPos(out POINT lpPoint);

        /// <summary>
        /// Point structure for Win32 API.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        /// <summary>
        /// Rectangle structure for Win32 API.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        /// <summary>
        /// Monitor information structure.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct MONITORINFO
        {
            public uint cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        /// <summary>
        /// Flags for MonitorFromPoint.
        /// </summary>
        public const uint MONITOR_DEFAULTTOPRIMARY = 1;
    }
}
