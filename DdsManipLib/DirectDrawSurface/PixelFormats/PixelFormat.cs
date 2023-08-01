using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public abstract class PixelFormat : Attribute, IPixelFormat, IEquatable<PixelFormat> {
    public abstract int Bpp { get; }
    public abstract int CalculatePitch(int width);
    public abstract int CalculateLinearSize(int width, int height);

    public abstract bool Equals(PixelFormat? other);

    public override bool Equals(object? obj) => obj is PixelFormat pfa && Equals(pfa);

    public override int GetHashCode() => HashCode.Combine(base.GetHashCode(), Bpp);
}