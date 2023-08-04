using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfLxAxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawLaPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfLxAxUNormPixelFormat(int nbits, int lshift, int lbits, int ashift, int abits) : base(nbits, lshift, lbits, 0, 0, 0, 0, ashift, abits) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromLuminance(BitsPerPixel, RedMax << RedShift, AlphaMax << AlphaShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetLuminanceTyped(pixel)) / RedMax;
    public void SetLuminance(Span<byte> pixel, float value) => SetLuminance(pixel, T.CreateTruncating(value * RedMax));
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);

    public void SetLuminance(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));

    public float GetAlpha(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetAlphaTyped(pixel)) / AlphaMax;
    public void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, T.CreateTruncating(value * AlphaMax));
    public T GetAlphaTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> AlphaShift) & AlphaMax);

    public void SetAlpha(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(AlphaMax << AlphaShift) | (Math.Clamp(uint.CreateTruncating(value), 0, AlphaMax) << AlphaShift));
}