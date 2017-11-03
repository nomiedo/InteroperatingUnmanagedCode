using System;
using System.Runtime.InteropServices;

namespace ConsoleApplication1
{
    class Program
    {
        const int LastSleepTimeLevel = 15;
        const int LastWakeTimeLevel = 14;
        const int SystemPowerInformationLevel = 12;
        const int SystemBatteryStateLevel = 5;
        const int SystemReserveHiberFileLevel = 10;
        const uint STATUS_SUCCESS = 0;

        #region Structures
        struct LastSleepTimeBuffer
        {
            public ulong LastSleepTime;
        }

        struct LastWakeTimeBuffer
        {
            public ulong LastWakeTime;
        }

        struct SystemReserveHiberFileBuffer
        {
            public bool SystemReserveHiberFile;
        }

        struct SystemPowerInformationBuffer
        {
            public uint MaxIdlenessAllowed;
            public uint Idleness;
            public uint TimeRemaining;
            public byte CoolingMode;
        }

        struct SystemBatteryStateBuffer
        {
            public bool AcOnLine;
            public bool BatteryPresent;
            public bool Charging;
            public bool Discharging;
            public string MaxCapacity;
            public string RemainingCapacity;
            public string Rate;
            public string EstimatedTime;
            public string DefaultAlert1;
            public string DefaultAlert2;
        }

        struct Buffer
        {
            public ulong LastSleepTime;
            public ulong LastWakeTime;
            public SystemPowerInformationBuffer systemPowerInformationBuffer;
            public SystemBatteryStateBuffer systemBatteryStateBuffer;
        }
        #endregion
   
        #region DllImport

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out LastSleepTimeBuffer lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out LastWakeTimeBuffer lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out SystemPowerInformationBuffer lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            out SystemBatteryStateBuffer lpOutputBuffer,
            int nOutputBufferSize
        );

        #endregion

        static void Main(string[] args)
        {
            // get Last Sleep Time
            LastSleepTimeBuffer outputBuffer;
            uint retval = CallNtPowerInformation(
                LastSleepTimeLevel,
                IntPtr.Zero,
                0,
                out outputBuffer,
                Marshal.SizeOf(typeof(LastSleepTimeBuffer))
            );
            if (retval == STATUS_SUCCESS)
            {
                Console.WriteLine($"Last Sleep Time: {outputBuffer.LastSleepTime}");
            }

            LastWakeTimeBuffer outputBuffer2;
            uint retval2 = CallNtPowerInformation(
                LastWakeTimeLevel,
                IntPtr.Zero,
                0,
                out outputBuffer2,
                Marshal.SizeOf(typeof(LastWakeTimeBuffer))
            );
            if (retval2 == STATUS_SUCCESS)
            {
                Console.WriteLine($"Last Wake Time: {outputBuffer2.LastWakeTime}");
            }

            SystemPowerInformationBuffer outputBuffer3;
            uint retval3 = CallNtPowerInformation(
                SystemPowerInformationLevel,
                IntPtr.Zero,
                0,
                out outputBuffer3,
                Marshal.SizeOf(typeof(SystemPowerInformationBuffer))
            );
            if (retval3 == STATUS_SUCCESS)
            {
                Console.WriteLine($"SPI : Cooling Mode: {outputBuffer3.CoolingMode}");
                Console.WriteLine($"SPI : Time Remaining: {outputBuffer3.TimeRemaining}");
                Console.WriteLine($"SPI : Idleness: {outputBuffer3.Idleness}");
                Console.WriteLine($"SPI : Max Idleness Allowed: {outputBuffer3.MaxIdlenessAllowed}");
            }

            SystemBatteryStateBuffer outputBuffer4;
            uint retval4 = CallNtPowerInformation(
                SystemBatteryStateLevel,
                IntPtr.Zero,
                0,
                out outputBuffer4,
                Marshal.SizeOf(typeof(SystemBatteryStateBuffer))
            );
            if (retval4 == STATUS_SUCCESS)
            {
                Console.WriteLine($"Battery : Ac On Line: {outputBuffer4.AcOnLine}");
                Console.WriteLine($"Battery : Battery Present: {outputBuffer4.BatteryPresent}");
                Console.WriteLine($"Battery : Charging: {outputBuffer4.Charging}");
                Console.WriteLine($"Battery : Discharging: {outputBuffer4.Discharging}");
            }

            Console.ReadLine();
        }
    }
}