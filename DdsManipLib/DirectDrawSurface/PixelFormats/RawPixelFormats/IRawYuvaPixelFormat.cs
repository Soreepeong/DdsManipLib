using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawYuvaPixelFormat : IRawYuvPixelFormat, IRawLaPixelFormat {
    public Vector4 GetYuva(ReadOnlySpan<byte> pixel) => new(GetLuminance(pixel), GetChromaBlue(pixel), GetChromaRed(pixel), GetAlpha(pixel));

    public void SetYuva(Span<byte> pixel, Vector4 yuva) {
        SetLuminance(pixel, yuva.X);
        SetChromaBlue(pixel, yuva.Y);
        SetChromaRed(pixel, yuva.Z);
        SetAlpha(pixel, yuva.W);
    }

    public Vector4 GetRgbaBt601(ReadOnlySpan<byte> pixel) => Vector4.Transform(GetYuva(pixel), PixelFormatUtilities.YuvToRgbBt601);
    public void SetRgbaBt601(Span<byte> pixel, Vector4 rgba) => SetYuva(pixel, Vector4.Transform(rgba, PixelFormatUtilities.RgbToYuvBt601));
    public Vector4 GetRgbaBt709(ReadOnlySpan<byte> pixel) => Vector4.Transform(GetYuva(pixel), PixelFormatUtilities.YuvToRgbBt709);
    public void SetRgbaBt709(Span<byte> pixel, Vector4 rgba) => SetYuva(pixel, Vector4.Transform(rgba, PixelFormatUtilities.RgbToYuvBt709));
}

public interface IRawYuvaPixelFormat<TLuminance> : IRawYuvaPixelFormat, IRawYuvPixelFormat<TLuminance>, IRawLaPixelFormat<TLuminance>
    where TLuminance : unmanaged, IBinaryNumber<TLuminance> { }
