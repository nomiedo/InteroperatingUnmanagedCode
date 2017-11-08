using System;
using System.Threading;
using PowerManagerLibrary;

namespace InteroperatingLibrary
{
    class Program
    {
        static void Main(string[] args)
        {
            IPowerManager powerManager = new PowerManager();

            var lastSleepTime = powerManager.GetPowerInfo_LastSleepTime();
            var dateS = new DateTime(1970, 1, 1).AddSeconds(double.Parse(lastSleepTime));
            Console.WriteLine($@"Last Sleep Time: {dateS}");

            var lastWakeTime = powerManager.GetPowerInfo_LastWakeTime();
            var dateW = new DateTime(1970, 1, 1).AddSeconds(double.Parse(lastWakeTime));
            Console.WriteLine($@"Last Wake Time: {dateW}");

            powerManager.SystemReserveHiberFileReserve();

            var systemPowerInformation = powerManager.GetPowerInfo_SystemPowerInformation();
            Console.WriteLine($@"SystemPowerInformation : CoolingMode: {systemPowerInformation.CoolingMode}");
            Console.WriteLine($@"SystemPowerInformation : Idleness: {systemPowerInformation.Idleness}");
            Console.WriteLine($@"SystemPowerInformation : MaxIdlenessAllowed: {systemPowerInformation.MaxIdlenessAllowed}");
            Console.WriteLine($@"SystemPowerInformation : TimeRemaining: {systemPowerInformation.TimeRemaining}");

            var systemBatteryState = powerManager.GetPowerInfo_SystemBatteryState();
            Console.WriteLine($@"SystemBatteryState : Charging: {systemBatteryState.Charging}");
            Console.WriteLine($@"SystemBatteryState : AcOnLine: {systemBatteryState.AcOnLine}");
            Console.WriteLine($@"SystemBatteryState : BatteryPresent: {systemBatteryState.BatteryPresent}");
            Console.WriteLine($@"SystemBatteryState : MaxCapacity: {systemBatteryState.MaxCapacity}");

            Console.WriteLine(@"Going to sleep in 3 secundos...");
            Thread.Sleep(3000);
            powerManager.Sleep();
            //Sleep
            powerManager.SetWaitForWakeUpTime();
        }     
    }
}