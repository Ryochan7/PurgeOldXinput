using System;
using System.Collections.Generic;
using Nefarius.Devcon;

namespace PurgeOldXInput
{
    class Program
    {
        static Guid xnaClassGuid = Guid.Parse("{d61ca365-5af4-4486-998b-9db4734c6ca3}");
        static Guid hidSetupClassGuid = Guid.Parse("{745a17a0-74d3-11d0-b6fe-00a0c90f57da}");

        static void Main(string[] args)
        {
            List<string> xnaInstanceIds = GetXnaDevices();
            List<string> hidFFInstanceIds = GetXboxHidFFDevices();
            List<string> hidJoystickInstanceIds = GetXboxHidJoystickDevices();

            int deletedDevices = 0;

            Console.WriteLine($"Found {xnaInstanceIds.Count} XnaComposite devices");
            foreach(string instanceId in xnaInstanceIds)
            {
                if (Devcon.Remove(xnaClassGuid, instanceId))
                {
                    deletedDevices++;
                }
            }

            Console.WriteLine($"Deleted {deletedDevices} XnaComposite devices");

            deletedDevices = 0;
            Console.WriteLine($"Found {hidJoystickInstanceIds.Count} Xbox 360 HID-compliant game controller devices");
            foreach (string instanceId in hidJoystickInstanceIds)
            {
                if (Devcon.Remove(hidSetupClassGuid, instanceId))
                {
                    deletedDevices++;
                }
            }

            Console.WriteLine($"Deleted {deletedDevices} Xbox 360 HID-compliant game controller devices");

            deletedDevices = 0;
            Console.WriteLine($"Found {hidFFInstanceIds.Count} Xbox 360 USB Input Device devices");
            foreach (string instanceId in hidFFInstanceIds)
            {
                if (Devcon.Remove(hidSetupClassGuid, instanceId))
                {
                    deletedDevices++;
                }
            }

            Console.WriteLine($"Deleted {deletedDevices} Xbox 360 USB Input Device devices");
        }

        static List<string> GetXnaDevices()
        {
            List<string> deviceInstanceIds = new List<string>();

            bool result = false;
            //Guid xnaGuid = Guid.Parse("{d61ca365-5af4-4486-998b-9db4734c6ca3}");
            Guid xnaGuid = xnaClassGuid;

            NativeMethods.SP_DEVINFO_DATA deviceInfoData =
                new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize =
                System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);

            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref xnaGuid, null, 0, 0);
            for (int i = 0; !result && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData,
                    ref NativeMethods.DEVPKEY_Device_InstanceId, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string instanceId = dataBuffer.ToUTF16String();
                    deviceInstanceIds.Add(instanceId);
                    //Console.WriteLine(instanceId);
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return deviceInstanceIds;
        }

        static List<string> GetXboxHidJoystickDevices()
        {
            List<string> deviceInstanceIds = new List<string>();

            bool result = false;

            NativeMethods.SP_DEVINFO_DATA deviceInfoData =
                new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize =
                System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);

            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref hidSetupClassGuid, null, 0, 0);

            //int match = 0;
            for (int i = 0; !result && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData,
                    ref NativeMethods.DEVPKEY_Device_InstanceId, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string instanceId = dataBuffer.ToUTF16String();
                    if (instanceId.StartsWith(@"HID\VID_045E&PID_028E&IG_"))
                    {
                        deviceInstanceIds.Add(instanceId);
                        //match++;
                        //Console.WriteLine(instanceId);
                    }

                    //Console.WriteLine(instanceId);
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return deviceInstanceIds;
        }

        static List<string> GetXboxHidFFDevices()
        {
            List<string> deviceInstanceIds = new List<string>();

            bool result = false;

            NativeMethods.SP_DEVINFO_DATA deviceInfoData =
                new NativeMethods.SP_DEVINFO_DATA();
            deviceInfoData.cbSize =
                System.Runtime.InteropServices.Marshal.SizeOf(deviceInfoData);

            var dataBuffer = new byte[4096];
            ulong propertyType = 0;
            var requiredSize = 0;
            IntPtr deviceInfoSet = NativeMethods.SetupDiGetClassDevs(ref hidSetupClassGuid, null, 0, 0);
            //int match = 0;
            for (int i = 0; !result && NativeMethods.SetupDiEnumDeviceInfo(deviceInfoSet, i, ref deviceInfoData); i++)
            {
                if (NativeMethods.SetupDiGetDeviceProperty(deviceInfoSet, ref deviceInfoData,
                    ref NativeMethods.DEVPKEY_Device_InstanceId, ref propertyType,
                    dataBuffer, dataBuffer.Length, ref requiredSize, 0))
                {
                    string instanceId = dataBuffer.ToUTF16String();
                    if (instanceId.StartsWith(@"USB\VID_045E&PID_028E&IG_"))
                    {
                        deviceInstanceIds.Add(instanceId);
                        //match++;
                        //Console.WriteLine(instanceId);
                    }

                    //Console.WriteLine(instanceId);
                }
            }

            if (deviceInfoSet.ToInt64() != NativeMethods.INVALID_HANDLE_VALUE)
            {
                NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoSet);
            }

            return deviceInstanceIds;
        }
    }
}
