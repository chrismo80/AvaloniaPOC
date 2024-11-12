using CompanyName.UI.Devices;

using ProjectExampleHMI.Models;

namespace ProjectExampleHMI.Devices;

public class Cam110() : CameraDevice("CAM_110");

public class Cam111() : CameraDevice("CAM_111");

public class LedController1() : IlluminationDevice("COM1");

public class LedController2() : IlluminationDevice("COM2");

public class SyncDevice1(Inspection1 inspection) : SyncDevice("GVL.ExtSync1", inspection);

public class SyncDevice2(Inspection2 inspection) : SyncDevice("GVL.ExtSync2", inspection);

public class SyncDevice3(Inspection3 inspection) : SyncDevice("GVL.ExtSync3", inspection);

public class DeviceX1() : DeviceX(1, "Device X1", 6.5);

public class DeviceY1() : DeviceY(2, "Device Y1", "pathToFile");

public class DeviceX2() : DeviceX(3, "Device X2", 15.6);