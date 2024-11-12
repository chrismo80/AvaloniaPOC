using CompanyName.Core.Data;
using CompanyName.Core.Messages;
using CompanyName.Core.Models;
using CompanyName.UI.Devices;

namespace CompanyName.UI.Models;

public abstract class InspectionModel : BaseModel
{
	readonly IParameterManager _parameterManager;
	readonly string _imagingModuleName = "";

	public IlluminationDevice LedController { get; set; }

	public CameraDevice Camera { get; set; }

	public int Executions { get; set; }

	public string Name { get; set; }

	protected InspectionModel(CameraDevice camera, IlluminationDevice ledController,
		IParameterManager paramManager, string imagingModuleName)
	{
		Name = GetType().Name;
		Camera = camera;
		LedController = ledController;

		_parameterManager = paramManager;
		_imagingModuleName = imagingModuleName;
	}

	public override async Task Execute()
	{
		await LedController.ActivateIllumination();

		await Camera.GrabImage();

		_ = LedController.DeactivateIllumination();

		this.CreateMessage("Inspection done", MessageType.Information);

		Executions++;

		OnPropertyChanged(nameof(Executions));
	}
}