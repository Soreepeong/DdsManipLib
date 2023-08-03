using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxGxBxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawRgbPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    internal DdspfRxGxBxUNormPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits, int bshift, int bbits) : base(nbits, rshift, rbits, gshift, gbits, bshift, bbits, 0, 0) { }

    public float GetRed(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetRedTyped(pixel)) / RedMax;
    public void SetRed(Span<byte> pixel, float value) => SetRed(pixel, T.CreateTruncating(value * RedMax));
    public T GetRedTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);
    public void SetRed(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));
    
    public float GetGreen(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetGreenTyped(pixel)) / GreenMax;
    public void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, T.CreateTruncating(value * GreenMax));
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> GreenShift) & GreenMax);
    public void SetGreen(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(GreenMax << GreenShift) | (Math.Clamp(uint.CreateTruncating(value), 0, GreenMax) << GreenShift));
    
    public float GetBlue(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetBlueTyped(pixel)) / BlueMax;
    public void SetBlue(Span<byte> pixel, float value) => SetBlue(pixel, T.CreateTruncating(value * BlueMax));
    public T GetBlueTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> BlueShift) & BlueMax);
    public void SetBlue(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(BlueMax << BlueShift) | (Math.Clamp(uint.CreateTruncating(value), 0, BlueMax) << BlueShift));
}