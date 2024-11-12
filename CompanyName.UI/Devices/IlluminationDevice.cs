using CompanyName.Core.Messages;
using CompanyName.Core.Devices;

namespace CompanyName.UI.Devices;

public abstract class IlluminationDevice : BaseDevice
{
    public string ComPort { get; }

    protected IlluminationDevice(string comPort)
    {
        ComPort = comPort;
        Name += " - " + ComPort;
    }

    public async Task ActivateIllumination()
    {
        await Task.Delay(1000);

        Executions++;

        this.CreateMessage("Illumination on", MessageType.Information);
    }

    public async Task DeactivateIllumination()
    {
        await Task.Delay(1000);

        Executions++;

        this.CreateMessage("Illumination off", MessageType.Information);
    }
}