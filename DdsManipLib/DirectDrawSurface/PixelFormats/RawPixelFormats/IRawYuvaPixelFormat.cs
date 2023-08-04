using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawYuvaPixelFormat : IRawYuvPixelFormat, IRawLaPixelFormat {
    public Vector4 GetYuva(ReadOnlySpan<byte> pixel) => new(GetLuminance(pixel), GetChromaBlue(pixel), GetChromaRed(pixel), GetAlpha(pixel));

    public void SetYuva(Span<byte> pixel, Vector4 yuva) {
        SetLuminance(pixel, yuva.X);
        SetChromaBlue(pixel, yuva.Y);
        SetChromaRed(pixel, yuva.Z);
        SetAlpha(pixel, yuva.W);
    }

    public Vector4 GetRgbaBt601(ReadOnlySpan<byte> pixel) =>
        Vector4.Transform(GetYuva(pixel) - new Vector4(0, 0.5f, 0.5f, 0), PixelFormatUtilities.YuvToRgbBt601);

    public void SetRgbaBt601(Span<byte> pixel, Vector4 rgba) =>
        SetYuva(pixel, Vector4.Transform(rgba, PixelFormatUtilities.RgbToYuvBt601) + new Vector4(0, 0.5f, 0.5f, 0));

    public Vector4 GetRgbaBt709(ReadOnlySpan<byte> pixel) =>
        Vector4.Transform(GetYuva(pixel) - new Vector4(0, 0.5f, 0.5f, 0), PixelFormatUtilities.YuvToRgbBt709);

    public void SetRgbaBt709(Span<byte> pixel, Vector4 rgba) =>
        SetYuva(pixel, Vector4.Transform(rgba, PixelFormatUtilities.RgbToYuvBt709) + new Vector4(0, 0.5f, 0.5f, 0));
}

public interface IRawYuvaPixelFormat<T> : IRawYuvaPixelFormat, IRawYuvPixelFormat<T>, IRawLaPixelFormat<T> where T : unmanaged, IBinaryNumber<T> {
    public Vector4<T> GetYuvTyped(ReadOnlySpan<byte> pixel) =>
        new(GetLuminanceTyped(pixel), GetChromaBlueTyped(pixel), GetChromaRedTyped(pixel), GetAlphaTyped(pixel));

    public void SetYuva(Span<byte> pixel, Vector4<T> yuv) {
        SetLuminance(pixel, yuv.X);
        SetChromaBlue(pixel, yuv.Y);
        SetChromaRed(pixel, yuv.Z);
        SetAlpha(pixel, yuv.W);
    }
}
