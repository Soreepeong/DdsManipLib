using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.DepthPixelFormats;

public class DsxPixelFormat<TDepth, TStencil, TX>
    : DepthStencilPixelFormat<TDepth, TStencil>, IX1PixelFormat
    where TDepth : unmanaged, IChannel
    where TStencil : unmanaged, IChannel
    where TX : unmanaged, IChannel {
    public DsxPixelFormat(int depth, int stencil, int x) : base(depth, stencil) {
        X1 = (IChannel) Activator.CreateInstance(typeof(TX), base.Bpp, x)!;
    }

    public IChannel X1 { get; }
    public override int Bpp => Depth.BitCount + Stencil.BitCount + X1.BitCount;

    public override bool Equals(PixelFormat? other) =>
        GetType() == other?.GetType()
        && other is DsxPixelFormat<TDepth, TStencil, TX> r
        && Depth.Equals(r.Depth)
        && Stencil.Equals(r.Stencil)
        && X1.Equals(r.X1);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), X1.GetHashCode());
}
