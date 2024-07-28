using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace hiro.Helpers
{
    internal class Hiro_Bluetooth
    {
        public class BluetoothConnection
        {
            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern int BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams, out IntPtr deviceInfo);

            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern bool BluetoothFindNextDevice(IntPtr hRadio, out IntPtr deviceInfo);

            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern bool BluetoothFindDeviceClose(IntPtr hRadio);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
            {
                public uint dwSize;
                public uint uFlags;
                public uint cTimeoutMultiplier;
                public IntPtr hRadio;
                public bool fReturnAuthenticated;
                public bool fReturnRemembered;
                public bool fReturnUnknown;
                public bool fReturnConnected;
                public bool fReturnAllDevices;
                public bool fReturnNewDevices;
                public bool fReturnRawDevices;
                public uint fReturnUninitialized;
            }

            // You should define BLUETOOTH_DEVICE_INFO and other structures as needed

            public static void ConnectToDevice(string deviceAddress)
            {
                // Initialize search parameters and device info
                var searchParams = new BLUETOOTH_DEVICE_SEARCH_PARAMS
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(BLUETOOTH_DEVICE_SEARCH_PARAMS)),
                    fReturnAuthenticated = true,
                    fReturnRemembered = true,
                    fReturnUnknown = true,
                    fReturnConnected = true,
                    fReturnAllDevices = true,
                    fReturnNewDevices = true,
                };

                IntPtr deviceInfo = IntPtr.Zero;

                if (BluetoothFindFirstDevice(ref searchParams, out deviceInfo) > 0)
                {
                    // Iterate through found devices and match with deviceAddress
                    do
                    {
                        // Check device info and compare with deviceAddress
                    } while (BluetoothFindNextDevice(deviceInfo, out deviceInfo));

                    BluetoothFindDeviceClose(deviceInfo);
                }
            }
        }
        public class BluetoothDisconnection
        {
            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern int BluetoothFindFirstDevice(ref BLUETOOTH_DEVICE_SEARCH_PARAMS searchParams, out IntPtr deviceInfo);

            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern bool BluetoothFindNextDevice(IntPtr hRadio, out IntPtr deviceInfo);

            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern bool BluetoothFindDeviceClose(IntPtr hRadio);

            [DllImport("bthprops.cpl", CharSet = CharSet.Auto)]
            private static extern int BluetoothRemoveDevice(ref BLUETOOTH_ADDRESS address);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct BLUETOOTH_DEVICE_SEARCH_PARAMS
            {
                public uint dwSize;
                public uint uFlags;
                public uint cTimeoutMultiplier;
                public IntPtr hRadio;
                public bool fReturnAuthenticated;
                public bool fReturnRemembered;
                public bool fReturnUnknown;
                public bool fReturnConnected;
                public bool fReturnAllDevices;
                public bool fReturnNewDevices;
                public bool fReturnRawDevices;
                public uint fReturnUninitialized;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct BLUETOOTH_ADDRESS
            {
                public ulong ullLong;
            }

            public static void DisconnectBluetoothDevice(string deviceAddress)
            {
                var searchParams = new BLUETOOTH_DEVICE_SEARCH_PARAMS
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(BLUETOOTH_DEVICE_SEARCH_PARAMS)),
                    fReturnAuthenticated = true,
                    fReturnRemembered = true,
                    fReturnUnknown = true,
                    fReturnConnected = true,
                    fReturnAllDevices = true,
                    fReturnNewDevices = true
                };

                IntPtr deviceInfo = IntPtr.Zero;

                // Find the device
                if (BluetoothFindFirstDevice(ref searchParams, out deviceInfo) > 0)
                {
                    do
                    {
                        // Assuming deviceInfo contains information, you need to extract and compare it
                        // You need to define BLUETOOTH_DEVICE_INFO and extract the BluetoothAddress
                        var device = (BLUETOOTH_DEVICE_INFO)Marshal.PtrToStructure(deviceInfo, typeof(BLUETOOTH_DEVICE_INFO));
                        var deviceAddr = device.Address.ullLong.ToString("X");

                        if (deviceAddr.Equals(deviceAddress, StringComparison.OrdinalIgnoreCase))
                        {
                            // Create BluetoothAddress object
                            var addr = new BLUETOOTH_ADDRESS
                            {
                                ullLong = device.Address.ullLong
                            };

                            // Remove device
                            int result = BluetoothRemoveDevice(ref addr);
                            if (result == 0) // Success
                            {
                                Console.WriteLine($"Successfully disconnected from device: {deviceAddress}");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to disconnect from device: {deviceAddress}");
                            }
                        }

                    } while (BluetoothFindNextDevice(deviceInfo, out deviceInfo));

                    BluetoothFindDeviceClose(deviceInfo);
                }
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
            public struct BLUETOOTH_DEVICE_INFO
            {
                public uint dwSize;
                public BLUETOOTH_ADDRESS Address;
                public uint dwServiceMask;
                public uint uCategory;
                public uint szName;
                public uint szNameLen;
                public uint szServiceName;
                public uint szServiceNameLen;
            }
        }
    }
}
