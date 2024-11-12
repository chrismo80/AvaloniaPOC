using CompanyName.Core.Devices;

namespace ProjectExampleHMI.Devices;

public abstract class DeviceY(int id, string name, string file) : IDevice
{
    string _file = file;

    public int Id { get; set; } = id;

    public string Name { get; set; } = name;

    public object GetParameterValue(string parameterName)
    {
        return _file;
    }

    public void SetParameterValue(string parameterName, object value)
    { }
}