using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.DepthPixelFormats;

public class DepthStencilPixelFormat<TDepth, TStencil>
    : DepthPixelFormat<TDepth>
    where TDepth : unmanaged, IChannel
    where TStencil : unmanaged, IChannel {
    public DepthStencilPixelFormat(int depth, int stencil) : base(depth) {
        Stencil = (IChannel) Activator.CreateInstance(typeof(TStencil), base.Bpp, stencil)!;
    }

    public IChannel Stencil { get; }
    public override int Bpp => Depth.BitCount + Stencil.BitCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is DepthStencilPixelFormat<TDepth, TStencil> r
        && Depth.Equals(r.Depth)
        && Stencil.Equals(r.Stencil);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Stencil.GetHashCode());
}