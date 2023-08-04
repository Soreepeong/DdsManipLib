using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public interface IRawYuvPixelFormat : IRawLPixelFormat {
    public float GetChromaBlue(ReadOnlySpan<byte> pixel);
    public void SetChromaBlue(Span<byte> pixel, float value);
    public float GetChromaRed(ReadOnlySpan<byte> pixel);
    public void SetChromaRed(Span<byte> pixel, float value);

    public Vector3 GetYuv(ReadOnlySpan<byte> pixel) => new(GetLuminance(pixel), GetChromaBlue(pixel), GetChromaRed(pixel));

    public void SetYuv(Span<byte> pixel, Vector3 yuv) {
        SetLuminance(pixel, yuv.X);
        SetChromaBlue(pixel, yuv.Y);
        SetChromaRed(pixel, yuv.Z);
    }

    public Vector3 GetRgbBt601(ReadOnlySpan<byte> pixel) => Vector3.Transform(GetYuv(pixel) - new Vector3(0, 0.5f, 0.5f), PixelFormatUtilities.YuvToRgbBt601);

    public void SetRgbBt601(Span<byte> pixel, Vector3 rgb) =>
        SetYuv(pixel, Vector3.Transform(rgb, PixelFormatUtilities.RgbToYuvBt601) + new Vector3(0, 0.5f, 0.5f));

    public Vector3 GetRgbBt709(ReadOnlySpan<byte> pixel) => Vector3.Transform(GetYuv(pixel) - new Vector3(0, 0.5f, 0.5f), PixelFormatUtilities.YuvToRgbBt709);

    public void SetRgbBt709(Span<byte> pixel, Vector3 rgb) =>
        SetYuv(pixel, Vector3.Transform(rgb, PixelFormatUtilities.RgbToYuvBt709) + new Vector3(0, 0.5f, 0.5f));
}

public interface IRawYuvPixelFormat<T> : IRawYuvPixelFormat, IRawLPixelFormat<T> where T : unmanaged, IBinaryNumber<T> {
    public T GetChromaBlueTyped(ReadOnlySpan<byte> pixel);
    public void SetChromaBlue(Span<byte> pixel, T value);
    public T GetChromaRedTyped(ReadOnlySpan<byte> pixel);
    public void SetChromaRed(Span<byte> pixel, T value);

    public Vector3<T> GetYuvTyped(ReadOnlySpan<byte> pixel) => new(GetLuminanceTyped(pixel), GetChromaBlueTyped(pixel), GetChromaRedTyped(pixel));

    public void SetYuv(Span<byte> pixel, Vector3<T> yuv) {
        SetLuminance(pixel, yuv.X);
        SetChromaBlue(pixel, yuv.Y);
        SetChromaRed(pixel, yuv.Z);
    }
}
