using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfLxUNormPixelFormat<T> : DdspfPixelFormat, IRawLPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfLxUNormPixelFormat(int nbits, int lshift, int lbits) : base(nbits, lshift, lbits, 0, 0, 0, 0, 0, 0) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromLuminance(BitsPerPixel, RedMax << RedShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => GetFloatFromUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, float value) => UpdateUNorm<T>(pixel, RedShift, RedBits, value);
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => GetUNorm<T>(pixel, RedShift, RedBits);
    public void SetLuminance(Span<byte> pixel, T value) => UpdateUNorm(pixel, RedShift, RedBits, value);

    public override void ClearPixel(Span<byte> pixel) => SetRaw(pixel, 0u);
}
