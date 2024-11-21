#pragma warning disable CA1416 // Validate platform compatibility

using System.Drawing;
using CompanyName.Core.Messages;
using CompanyName.Core.Devices;
using CompanyName.Core.Logging;

namespace CompanyName.UI.Devices;

public abstract class CameraDevice : BaseDevice
{
    public string CameraName { get; }

    protected CameraDevice(string cameraName)
    {
        CameraName = cameraName;
        Name += " - " + CameraName;
    }

    public async Task<Bitmap> GrabImage()
    {
        using var tw = new TraceWatch(this);

        await Task.Delay(2000);

        Executions++;

        this.CreateMessage("Image grabbed", MessageType.Information);

        if (Executions == 3)
            Online = false;

        return new Bitmap(1, 1);
    }
}