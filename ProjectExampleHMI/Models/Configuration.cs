using CompanyName.Core.Data;
using CompanyName.UI.Models;
using ProjectExampleHMI.Devices;

namespace ProjectExampleHMI.Models;

public class Inspection1(Cam110 cam, LedController1 ledController, IParameterManager parameterManager)
	: InspectionModel(cam, ledController, parameterManager, "DoCrop");

public class Inspection2(Cam110 cam, LedController2 ledController, IParameterManager parameterManager)
	: InspectionModel(cam, ledController, parameterManager, "ReadDmc");

public class Inspection3(Cam111 cam, LedController2 ledController, IParameterManager parameterManager)
	: InspectionModel(cam, ledController, parameterManager, "MeasureDiameter");