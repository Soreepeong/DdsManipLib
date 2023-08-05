using System;
using System.Numerics;
using DdsManipLib.Utilities;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfLxAxUNormPixelFormat<T> : DdspfPixelFormat, IRawLaPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfLxAxUNormPixelFormat(int nbits, int lshift, int lbits, int ashift, int abits) : base(nbits, lshift, lbits, 0, 0, 0, 0, ashift, abits) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromLuminance(BitsPerPixel, RedMax << RedShift, AlphaMax << AlphaShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public float GetAlpha(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, AlphaShift, AlphaBits, value);
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, AlphaShift, AlphaBits);
    public void SetAlpha(Span<byte> pixel, T value) => UpdateUNorm(pixel, AlphaShift, AlphaBits, value);

    public Vector2 GetLa(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var l = PixelFormatUtilities.RawToUNorm(raw >>> RedShift, RedBits) / float.CreateTruncating(T.MaxValue);
        var a = PixelFormatUtilities.RawToUNorm(raw >>> AlphaShift, AlphaBits) / float.CreateTruncating(T.MaxValue);
        return new(l, a);
    }

    public Vector2<T> GetLaTyped(ReadOnlySpan<byte> pixel) {
        var raw = (int) GetRaw(pixel);
        var y = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> RedShift), RedBits);
        var a = PixelFormatUtilities.RawToUNorm(T.CreateTruncating(raw >>> AlphaShift), AlphaBits);
        return new(y, a);
    }

    public void SetLa(Span<byte> span, Vector2 la) {
        var l = PixelFormatUtilities.FloatToUNormRaw<uint>(la.X, RedBits);
        var a = PixelFormatUtilities.FloatToUNormRaw<uint>(la.Y, AlphaBits);
        SetRaw(span, (l << RedShift) | (a << AlphaShift));
    }

    public void SetLa(Span<byte> pixel, Vector2<T> la) {
        var l = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(la.X, RedBits));
        var a = uint.CreateTruncating(PixelFormatUtilities.UNormToRaw(la.Y, AlphaBits));
        SetRaw(pixel, (l << RedShift) | (a << AlphaShift));
    }

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, AlphaMax << AlphaShift);
}
