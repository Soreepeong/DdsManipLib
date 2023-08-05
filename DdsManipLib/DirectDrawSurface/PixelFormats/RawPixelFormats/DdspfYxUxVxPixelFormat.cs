using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfYxUxVxPixelFormat<T> : DdspfPixelFormat, IRawYuvPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfYxUxVxPixelFormat(int nbits, int yshift, int ybits, int ushift, int ubits, int vshift, int vbits)
        : base(nbits, yshift, ybits, ushift, ubits, vshift, vbits, 0, 0) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromYuva(BitsPerPixel, RedMax << RedShift, GreenMax << GreenShift, BlueMax << BlueShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public float GetChromaBlue(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, GreenShift, GreenBits) - 0.5f;
    public void SetChromaBlue(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, GreenShift, GreenBits, value + 0.5f);
    public T GetChromaBlueTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, GreenShift, GreenBits);
    public void SetChromaBlue(Span<byte> pixel, T value) => UpdateUNorm(pixel, GreenShift, GreenBits, value);

    public float GetChromaRed(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, BlueShift, BlueBits) - 0.5f;
    public void SetChromaRed(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, BlueShift, BlueBits, value + 0.5f);
    public T GetChromaRedTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, BlueShift, BlueBits);
    public void SetChromaRed(Span<byte> pixel, T value) => UpdateUNorm(pixel, BlueShift, BlueBits, value);

    public Vector3 GetYuv(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var y = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits));
        var u = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> GreenShift), GreenBits));
        var v = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> BlueShift), BlueBits));
        return new Vector3(y, u, v) / float.CreateTruncating(T.MaxValue) - new Vector3(0, 0.5f, 0.5f);
    }

    public void SetYuv(Span<byte> pixel, Vector3 yuv) {
        var y = PixelFormatUtilities.FloatToUNormRaw<uint>(yuv.X, RedBits);
        var u = PixelFormatUtilities.FloatToUNormRaw<uint>(yuv.Y + 0.5f, GreenBits);
        var v = PixelFormatUtilities.FloatToUNormRaw<uint>(yuv.Z + 0.5f, BlueBits);
        SetRaw(pixel, (y << RedShift) | (u << GreenShift) | (v << BlueShift));
    }

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, 0u);
}
