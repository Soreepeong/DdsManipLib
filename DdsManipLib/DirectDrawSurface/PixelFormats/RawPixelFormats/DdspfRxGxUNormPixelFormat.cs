using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxGxUNormPixelFormat<T> : DdspfPixelFormat, IRawRgPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    internal DdspfRxGxUNormPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits) : base(nbits, rshift, rbits, gshift, gbits, 0, 0, 0, 0) { }

    public float GetRed(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetRed(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public float GetGreen(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, GreenShift, GreenBits);
    public void SetGreen(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, GreenShift, GreenBits, value);
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, GreenShift, GreenBits);
    public void SetGreen(Span<byte> pixel, T value) => UpdateUNorm(pixel, GreenShift, GreenBits, value);

    public Vector2 GetRg(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var r = PixelFormatUtilities.RawToUNorm(raw >>> RedShift, RedBits) / float.CreateTruncating(T.MaxValue);
        var g = PixelFormatUtilities.RawToUNorm(raw >>> GreenShift, GreenBits) / float.CreateTruncating(T.MaxValue);
        return new(r, g);
    }

    public void SetRg(Span<byte> pixel, Vector2 rgb) {
        var r = PixelFormatUtilities.FloatToUNormRaw<uint>(rgb.X, RedBits);
        var g = PixelFormatUtilities.FloatToSNormRaw<uint>(rgb.Y, GreenBits);
        SetRaw(pixel, (r << RedShift) | (g << GreenShift));
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
        SetRaw(pixel, (r << RedShift) | (g << GreenShift));
    }

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, 0u);
}
