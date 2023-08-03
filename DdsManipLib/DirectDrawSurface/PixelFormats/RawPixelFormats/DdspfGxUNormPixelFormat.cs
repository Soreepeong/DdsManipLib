using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfGxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawGPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T> {
    internal DdspfGxUNormPixelFormat(int nbits, int gshift, int gbits) : base(nbits, 0, 0, gshift, gbits, 0, 0, 0, 0) { }

    public float GetGreen(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetGreenTyped(pixel)) / GreenMax;
    public void SetGreen(Span<byte> pixel, float value) => SetGreen(pixel, T.CreateTruncating(value * GreenMax));
    public T GetGreenTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> GreenShift) & GreenMax);
    public void SetGreen(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(GreenMax << GreenShift) | (Math.Clamp(uint.CreateTruncating(value), 0, GreenMax) << GreenShift));
}