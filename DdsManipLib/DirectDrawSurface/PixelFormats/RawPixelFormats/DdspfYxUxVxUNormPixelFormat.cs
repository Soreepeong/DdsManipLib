using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class DdspfYxUxVxUNormPixelFormat<T> : DdspfUNormPixelFormat, IRawYuvPixelFormat<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public DdspfYxUxVxUNormPixelFormat(int nbits, int yshift, int ybits, int ushift, int ubits, int vshift, int vbits) : base(nbits,
        yshift,
        ybits,
        ushift,
        ubits,
        vshift,
        vbits,
        0,
        0) { }

    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromYuva(BitsPerPixel, RedMax << RedShift, GreenMax << GreenShift, BlueMax << BlueShift);

    public float GetLuminance(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetLuminanceTyped(pixel)) / RedMax;
    public void SetLuminance(Span<byte> pixel, float value) => SetLuminance(pixel, T.CreateTruncating(value * RedMax));
    public T GetLuminanceTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> RedShift) & RedMax);

    public void SetLuminance(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(RedMax << RedShift) | (Math.Clamp(uint.CreateTruncating(value), 0, RedMax) << RedShift));

    public float GetChromaBlue(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetChromaBlueTyped(pixel)) / GreenMax;
    public void SetChromaBlue(Span<byte> pixel, float value) => SetChromaBlue(pixel, T.CreateTruncating(value * GreenMax));
    public T GetChromaBlueTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> GreenShift) & GreenMax);

    public void SetChromaBlue(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(GreenMax << GreenShift) | (Math.Clamp(uint.CreateTruncating(value), 0, GreenMax) << GreenShift));

    public float GetChromaRed(ReadOnlySpan<byte> pixel) => float.CreateTruncating(GetChromaRedTyped(pixel)) / BlueMax;
    public void SetChromaRed(Span<byte> pixel, float value) => SetChromaRed(pixel, T.CreateTruncating(value * BlueMax));
    public T GetChromaRedTyped(ReadOnlySpan<byte> pixel) => T.CreateTruncating((GetRaw(pixel) >> BlueShift) & BlueMax);

    public void SetChromaRed(Span<byte> pixel, T value) =>
        SetRaw(pixel, GetRaw(pixel) & ~(BlueMax << BlueShift) | (Math.Clamp(uint.CreateTruncating(value), 0, BlueMax) << BlueShift));
}
