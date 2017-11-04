using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

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
            public byte AcOnLine;

            public byte BatteryPresent;

            public byte Charging;

            public byte Discharging;

            public byte spare1;

            public byte spare2;

            public byte spare3;

            public byte spare4;

            public UInt32 MaxCapacity;

            public UInt32 RemainingCapacity;

            public Int32 Rate;

            public UInt32 EstimatedTime;

            public UInt32 DefaultAlert1;

            public UInt32 DefaultAlert2;
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

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("kernel32.dll")]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

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

            Console.WriteLine(@"Going to sleep in 3 secundos...");
            Thread.Sleep(3000);
            
            //Sleep
            SetSuspendState(false, false, false);
            SetWaitForWakeUpTime();

        }


        static void SetWaitForWakeUpTime()
        {
            DateTime time = DateTime.Now.AddMinutes(1);
            long duetime = time.ToFileTime();

            using (SafeWaitHandle handle = CreateWaitableTimer(IntPtr.Zero, true, "MyWaitabletimer"))
            {
                if (SetWaitableTimer(handle, ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
                {
                    using (EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset))
                    {
                        wh.SafeWaitHandle = handle;
                        wh.WaitOne();
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            // You could make it a recursive call here, setting it to 1 hours time or similar
            Console.WriteLine("Wake up call");
            Console.ReadLine();
        }
    }
}