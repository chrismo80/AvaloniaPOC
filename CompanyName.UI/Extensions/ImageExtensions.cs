using System.Drawing.Imaging;
using Avalonia.Media.Imaging;

#pragma warning disable CA1416 // Validate platform compatibility

namespace CompanyName.UI.Extensions;

public static class ImageExtensions
{
	public static Bitmap? ToBitmap(this System.Drawing.Bitmap? source)
	{
		if (source == null)
			return null;

		using var stream = new MemoryStream();

		source?.Save(stream, ImageFormat.Png);

		stream.Position = 0;

		return new Bitmap(stream);
	}

	public static System.Drawing.Bitmap? ToBitmap(this Bitmap? source)
	{
		if (source == null)
			return null;

		using var stream = new MemoryStream();

		source?.Save(stream);

		stream.Position = 0;

		return new System.Drawing.Bitmap(stream);
	}
}