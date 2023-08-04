using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfLxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawLPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfLxUNormPixelFormat(int nbits, int lshift, int lbits) : base(nbits, lshift, lbits, 0, 0, 0, 0, 0, 0) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromLuminance(BitsPerPixel, RedMax << RedShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetLuminanceTyped(pixel)) / RedMax;
    public void SetLuminance(Span<byte> pixel, float value) => SetLuminance(pixel, T.CreateTruncating(value * RedMax));
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);

    public void SetLuminance(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));
}
