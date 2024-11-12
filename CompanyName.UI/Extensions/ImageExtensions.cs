﻿#pragma warning disable CA1416 // Validate platform compatibility

namespace CompanyName.UI.Extensions;

public static class ImageExtensions
{
    public static Avalonia.Media.Imaging.Bitmap? ToBitmap(this System.Drawing.Bitmap source)
    {
        if (source == null)
            return null;

        using (var stream = new MemoryStream())
        {
            source?.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

            stream.Position = 0;

            return new Avalonia.Media.Imaging.Bitmap(stream);
        }
    }

    public static System.Drawing.Bitmap? ToBitmap(this Avalonia.Media.Imaging.Bitmap source)
    {
        if (source == null)
            return null;

        using (var stream = new MemoryStream())
        {
            source?.Save(stream);

            stream.Position = 0;

            return new System.Drawing.Bitmap(stream);
        }
    }
}