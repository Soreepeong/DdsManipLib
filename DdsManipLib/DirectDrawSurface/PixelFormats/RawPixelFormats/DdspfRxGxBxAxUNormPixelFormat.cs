using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxGxBxAxUNormPixelFormat<T> : DdspfPixelFormat, IRawRgbaPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    internal DdspfRxGxBxAxUNormPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits, int bshift, int bbits, int ashift, int abits)
        : base(nbits, rshift, rbits, gshift, gbits, bshift, bbits, ashift, abits) { }

    public AlphaType AlphaType => AlphaType.Straight;

    public float GetRed(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public float GetGreen(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, GreenShift, GreenBits);
    public void SetGreen(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, GreenShift, GreenBits, value);
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, GreenShift, GreenBits);
    public void SetGreen(Span<byte> pixel, T value) => UpdateUNorm(pixel, GreenShift, GreenBits, value);

    public float GetBlue(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, BlueShift, BlueBits);
    public void SetBlue(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, BlueShift, BlueBits, value);
    public T GetBlueTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, BlueShift, BlueBits);
    public void SetBlue(Span<byte> pixel, T value) => UpdateUNorm(pixel, BlueShift, BlueBits, value);

    public float GetAlpha(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, AlphaShift, AlphaBits, value);
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, T value) => UpdateUNorm(pixel, AlphaShift, AlphaBits, value);

    public Vector2 GetRg(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(raw >>> RedShift, RedBits) / float.CreateTruncating(T.MaxValue);
        var g = PixelFormatUtilities.RawToUNorm(raw >>> GreenShift, GreenBits) / float.CreateTruncating(T.MaxValue);
        return new(r, g);
    }

    public void SetRg(Span<byte> pixel, Vector2 rgb) {
        var r = PixelFormatUtilities.FloatToUNormRaw<uint>(rgb.X, RedBits);
        var g = PixelFormatUtilities.FloatToSNormRaw<uint>(rgb.Y, GreenBits);
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (GetRaw(pixel) & ((BlueMax << BlueShift) | (AlphaMax << AlphaShift))));
    }

    public Vector3 GetRgb(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(raw >>> RedShift, RedBits) / float.CreateTruncating(T.MaxValue);
        var g = PixelFormatUtilities.RawToUNorm(raw >>> GreenShift, GreenBits) / float.CreateTruncating(T.MaxValue);
        var b = PixelFormatUtilities.RawToUNorm(raw >>> BlueShift, BlueBits) / float.CreateTruncating(T.MaxValue);
        return new(r, g, b);
    }

    public void SetRgb(Span<byte> pixel, Vector3 rgb) {
        var r = PixelFormatUtilities.FloatToUNormRaw<uint>(rgb.X, RedBits);
        var g = PixelFormatUtilities.FloatToSNormRaw<uint>(rgb.Y, GreenBits);
        var b = PixelFormatUtilities.FloatToSNormRaw<uint>(rgb.Z, BlueBits);
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (b << BlueShift) | (GetRaw(pixel) & (AlphaMax << AlphaShift)));
    }

    public Vector4 GetRgba(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(raw >>> RedShift, RedBits) / float.CreateTruncating(T.MaxValue);
        var g = PixelFormatUtilities.RawToUNorm(raw >>> GreenShift, GreenBits) / float.CreateTruncating(T.MaxValue);
        var b = PixelFormatUtilities.RawToUNorm(raw >>> BlueShift, BlueBits) / float.CreateTruncating(T.MaxValue);
        var a = PixelFormatUtilities.RawToUNorm(raw >>> AlphaShift, AlphaBits) / float.CreateTruncating(T.MaxValue);
        return new(r, g, b, a);
    }

    public void SetRgba(Span<byte> pixel, Vector4 rgba) {
        var r = PixelFormatUtilities.FloatToUNormRaw<uint>(rgba.X, RedBits);
        var g = PixelFormatUtilities.FloatToSNormRaw<uint>(rgba.Y, GreenBits);
        var b = PixelFormatUtilities.FloatToSNormRaw<uint>(rgba.Z, BlueBits);
        var a = PixelFormatUtilities.FloatToUNormRaw<uint>(rgba.W, AlphaBits);
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (b << BlueShift) | (a << AlphaShift));
    }

    public Vector2<T> GetRgTyped(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits);
        var g = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> GreenShift), GreenBits);
        return new(r, g);
    }

    public void SetRg(Span<byte> pixel, Vector2<T> rg) {
        var r = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(rg.X, RedBits));
        var g = uint.CreateTruncating(PixelFormatUtilities.SNormToRaw(rg.Y, GreenBits));
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (GetRaw(pixel) & ((BlueMax << BlueShift) | (AlphaMax << AlphaShift))));
    }

    public Vector3<T> GetRgbTyped(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits);
        var g = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> GreenShift), GreenBits);
        var b = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> BlueShift), BlueBits);
        return new(r, g, b);
    }

    public void SetRgb(Span<byte> pixel, Vector3<T> rgb) {
        var r = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(rgb.X, RedBits));
        var g = uint.CreateTruncating(PixelFormatUtilities.SNormToRaw(rgb.Y, GreenBits));
        var b = uint.CreateTruncating(PixelFormatUtilities.SNormToRaw(rgb.Z, BlueBits));
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (b << BlueShift) | (GetRaw(pixel) & (AlphaMax << AlphaShift)));
    }

    public Vector4<T> GetRgbaTyped(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits);
        var g = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> GreenShift), GreenBits);
        var b = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> BlueShift), BlueBits);
        var a = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> AlphaShift), AlphaBits);
        return new(r, g, b, a);
    }

    public void SetRgba(Span<byte> pixel, Vector4<T> rgba) {
        var r = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(rgba.X, RedBits));
        var g = uint.CreateTruncating(PixelFormatUtilities.SNormToRaw(rgba.Y, GreenBits));
        var b = uint.CreateTruncating(PixelFormatUtilities.SNormToRaw(rgba.Z, BlueBits));
        var a = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(rgba.W, AlphaBits));
        SetRaw(pixel, (r << RedShift) | (g << GreenShift) | (b << BlueShift) | (a << AlphaShift));
    }

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, AlphaMax << AlphaShift);
}
