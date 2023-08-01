using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;
using DdsManipLib.DirectDrawSurface.PixelFormats.PlainPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public class AlphaPixelFormat<T>
    : PlainPixelFormat, IAlphaPlainPixelFormat
    where T : unmanaged, IChannel {
    public AlphaPixelFormat(int alpha, AlphaType alphaType) {
        Alpha = (IChannel) Activator.CreateInstance(typeof(T), 0, alpha)!;
        AlphaType = alphaType;
    }

    public IChannel Alpha { get; }
    public AlphaType AlphaType { get; }
    public override int Bpp => Alpha.BitCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is AlphaPixelFormat<T> r
        && Alpha.Equals(r.Alpha)
        && AlphaType == r.AlphaType;

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Alpha.GetHashCode(), Bpp);
}