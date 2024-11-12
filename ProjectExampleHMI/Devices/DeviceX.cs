using System;

using CompanyName.Core.Devices;
using CompanyName.Core.Messages;

namespace ProjectExampleHMI.Devices;

public abstract class DeviceX(int id, string name, double value) : IDevice
{
    readonly Random _random = new();

    double _lastValue = value;

    public int Id { get; set; } = id;

    public string Name { get; set; } = name;

    public void Execute(string methodName, params object[] methodParameter)
    {
        switch (methodName)
        {
            case "DoStuff": this.CreateMessage(Name + " did stuff", MessageType.Information); break;
            case "DoThis": this.CreateMessage(Name + " did this", MessageType.Information); break;
            case "DoThat": this.CreateMessage(Name + " did that", MessageType.Information); break;
        }
    }

    public object GetParameterValue(string parameterName)
    {
        var newValue = _random.NextDouble() - 0.5 + _lastValue;

        _lastValue = newValue;

        return newValue;
    }

    public void SetParameterValue(string parameterName, object value)
    {
        if (value is double x)
            _lastValue = x;
    }
}