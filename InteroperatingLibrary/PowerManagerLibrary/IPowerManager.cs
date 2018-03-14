using System.Runtime.InteropServices;

namespace PowerManagerLibrary
{
    [ComVisible(true)]
    [Guid("69E39A4B-7106-41A6-B5CF-3A6FA0B4E6D5")]
    [InterfaceType(ComInterfaceType.InterfaceIsDual)]
    public interface IPowerManager
    {
        string GetPowerInfo_LastSleepTime();
        string GetPowerInfo_LastWakeTime();
        PowerManager.SystemPowerInformationBuffer GetPowerInfo_SystemPowerInformation();
        PowerManager.SystemBatteryStateBuffer GetPowerInfo_SystemBatteryState();
        bool SystemReserveHiberFileReserve();
        void Sleep();
        void ShutDown();
        void SetWaitForWakeUpTime();
    }
}
