using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfRxGxBxAxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawRgbaPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    internal DdspfRxGxBxAxUNormPixelFormat(int nbits, int rshift, int rbits, int gshift, int gbits, int bshift, int bbits, int ashift, int abits) : base(nbits,
        rshift,
        rbits,
        gshift,
        gbits,
        bshift,
        bbits,
        ashift,
        abits) { }

    public AlphaType AlphaType => AlphaType.Straight;

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

    public float GetAlpha(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetAlphaTyped(pixel)) / AlphaMax;
    public void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, T.CreateTruncating(value * AlphaMax));
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> AlphaShift) & AlphaMax);

    public void SetAlpha(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(AlphaMax << AlphaShift) | (Math.Clamp(uint.CreateTruncating(value), 0, AlphaMax) << AlphaShift));
}
