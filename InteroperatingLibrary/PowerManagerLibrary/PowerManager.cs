using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace PowerManagerLibrary
{
    [ComVisible(true)]
    [Guid("8E2C74B2-8B52-4C12-8FCF-23F86DE02EE4")]
    [ClassInterface(ClassInterfaceType.None)]
    public class PowerManager: IPowerManager
    {
        const int LastSleepTimeLevel = 15;
        const int LastWakeTimeLevel = 14;
        const int SystemPowerInformationLevel = 12;
        const int SystemBatteryStateLevel = 5;
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

        public struct SystemPowerInformationBuffer
        {
            public uint MaxIdlenessAllowed;
            public uint Idleness;
            public uint TimeRemaining;
            public byte CoolingMode;
        }

        public struct SystemBatteryStateBuffer
        {
            public byte AcOnLine;

            public byte BatteryPresent;

            public byte Charging;

            public byte Discharging;

            public byte spare1;

            public byte spare2;

            public byte spare3;

            public byte spare4;

            public uint MaxCapacity;

            public uint RemainingCapacity;

            public uint Rate;

            public uint EstimatedTime;

            public uint DefaultAlert1;

            public uint DefaultAlert2;
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

        [DllImport("powrprof.dll")]
        static extern uint CallNtPowerInformation(
            int InformationLevel,
            IntPtr lpInputBuffer,
            int nInputBufferSize,
            IntPtr lpOutputBuffer,
            int nOutputBufferSize
        );

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("kernel32.dll")]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

        #endregion

        public string GetPowerInfo_LastSleepTime()
        {
            LastSleepTimeBuffer outputBuffer;
            uint retval = CallNtPowerInformation(
                LastSleepTimeLevel,
                IntPtr.Zero,
                0,
                out outputBuffer,
                Marshal.SizeOf(typeof(LastSleepTimeBuffer))
            );
            return retval == STATUS_SUCCESS ? outputBuffer.LastSleepTime.ToString() : null;
        }

        public string GetPowerInfo_LastWakeTime()
        {
            LastWakeTimeBuffer outputBuffer;
            uint retval = CallNtPowerInformation(
                LastWakeTimeLevel,
                IntPtr.Zero,
                0,
                out outputBuffer,
                Marshal.SizeOf(typeof(LastWakeTimeBuffer))
            );
            return retval == STATUS_SUCCESS ? outputBuffer.LastWakeTime.ToString() : null;
        }

        public SystemPowerInformationBuffer GetPowerInfo_SystemPowerInformation()
        {
            SystemPowerInformationBuffer outputBuffer;
            uint retval = CallNtPowerInformation(
                SystemPowerInformationLevel,
                IntPtr.Zero,
                0,
                out outputBuffer,
                Marshal.SizeOf(typeof(SystemPowerInformationBuffer))
            );

            return outputBuffer;
        }

        public SystemBatteryStateBuffer GetPowerInfo_SystemBatteryState()
        {
            SystemBatteryStateBuffer outputBuffer;
            uint retval = CallNtPowerInformation(
                SystemBatteryStateLevel,
                IntPtr.Zero,
                0,
                out outputBuffer,
                Marshal.SizeOf(typeof(SystemBatteryStateBuffer))
            );
            return outputBuffer;
        }

        public bool SystemReserveHiberFileReserve()
        {
            bool outputBuffer;
            uint retval = CallNtPowerInformation(
                10,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                0
            );
            return retval == STATUS_SUCCESS;
        }

        public void Sleep()
        {
            SetSuspendState(false, false, false);
        }

        public void SetWaitForWakeUpTime()
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
