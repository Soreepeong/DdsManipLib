using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old;

/// <summary>
/// Represent a pixel format that is not supported by us.
/// </summary>
public class UnknownPixelFormat : PixelFormat, IEquatable<UnknownPixelFormat> {
    /// <summary>
    /// The sole instance of UnknownPixelFormat.
    /// </summary>
    public static readonly UnknownPixelFormat Instance = new();

    private UnknownPixelFormat() {
        Alpha = AlphaType.None;
        Bpp = 0;
    }

    /// <inheritdoc />
    public override DxgiFormat DxgiFormat => DxgiFormat.Unknown;

    /// <inheritdoc />
    public override DdsFourCc FourCc => DdsFourCc.Unknown;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => ReferenceEquals(obj, this);

    /// <inheritdoc/>
    public bool Equals(UnknownPixelFormat? other) => ReferenceEquals(other, this);

    /// <inheritdoc/>
    public override int GetHashCode() => 0x4df85ea8;
}
