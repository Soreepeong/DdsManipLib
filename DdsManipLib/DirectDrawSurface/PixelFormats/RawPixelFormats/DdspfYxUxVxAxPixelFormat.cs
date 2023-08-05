using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfYxUxVxAxPixelFormat<T> : DdspfPixelFormat, IRawYuvaPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfYxUxVxAxPixelFormat(int nbits, int yshift, int ybits, int ushift, int ubits, int vshift, int vbits, int ashift, int abits)
        : base(nbits, yshift, ybits, ushift, ubits, vshift, vbits, ashift, abits) { }

    public override DdsPixelFormat DdsPixelFormat =>
        DdsPixelFormat.FromYuva(BitsPerPixel, RedMax << RedShift, GreenMax << GreenShift, BlueMax << BlueShift, AlphaMax << AlphaShift);

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

    public float GetAlpha(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, AlphaShift, AlphaBits, value);
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, T value) => UpdateUNorm(pixel, AlphaShift, AlphaBits, value);

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
        SetRaw(pixel, (y << RedShift) | (u << GreenShift) | (v << BlueShift) | (GetRaw(pixel) & (AlphaMax << AlphaShift)));
    }

    public Vector4 GetYuva(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var y = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits));
        var u = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> GreenShift), GreenBits));
        var v = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> BlueShift), BlueBits));
        var a = float.CreateTruncating(PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> AlphaShift), AlphaBits));
        return new Vector4(y, u, v, a) / float.CreateTruncating(T.MaxValue) - new Vector4(0, 0.5f, 0.5f, 0);
    }

    public void SetYuva(Span<byte> pixel, Vector4 yuva) {
        var y = PixelFormatUtilities.FloatToUNormRaw<uint>(yuva.X, RedBits);
        var u = PixelFormatUtilities.FloatToUNormRaw<uint>(yuva.Y + 0.5f, GreenBits);
        var v = PixelFormatUtilities.FloatToUNormRaw<uint>(yuva.Z + 0.5f, BlueBits);
        var a = PixelFormatUtilities.FloatToUNormRaw<uint>(yuva.W, AlphaBits);
        SetRaw(pixel, (y << RedShift) | (u << GreenShift) | (v << BlueShift) | (a << AlphaShift));
    }

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, AlphaMax << AlphaShift);
}
