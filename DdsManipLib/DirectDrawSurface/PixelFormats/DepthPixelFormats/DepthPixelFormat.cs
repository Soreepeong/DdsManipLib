using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.DepthPixelFormats;

public class DepthPixelFormat<T> : RawPixelFormat
    where T : unmanaged, IChannel {
    public DepthPixelFormat(int depth) {
        Depth = (IChannel) Activator.CreateInstance(typeof(T), 0, depth)!;
    }

    public IChannel Depth { get; }
    public override int Bpp => Depth.BitCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is DepthPixelFormat<T> r
        && Depth.Equals(r.Depth);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Depth.GetHashCode());
}