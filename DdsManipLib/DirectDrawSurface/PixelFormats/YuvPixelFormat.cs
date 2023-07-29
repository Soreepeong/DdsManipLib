using System;
using System.Linq;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

/// <summary>
/// Represent a pixel format containing YUV values.
/// </summary>
public class YuvPixelFormat : PixelFormat, IEquatable<YuvPixelFormat> {
    /// <summary>
    /// Number of bits for the luminousity channel. 
    /// </summary>
    public readonly ChannelDefinition Y;

    /// <summary>
    /// Number of bits for the blue projection channel. 
    /// </summary>
    public readonly ChannelDefinition U;

    /// <summary>
    /// Number of bits for the red projection channel. 
    /// </summary>
    public readonly ChannelDefinition V;

    /// <summary>
    /// Number of bits for the alpha channel.
    /// </summary>
    public readonly ChannelDefinition A;

    /// <summary>
    /// Number of bits for the extra channel.
    /// </summary>
    public readonly ChannelDefinition X;

    /// <summary>
    /// Construct a new instance of the class.
    /// </summary>
    /// <param name="alphaType"></param>
    /// <param name="y">Number of bits for the luminousity channel.</param>
    /// <param name="u">Number of bits for the blue projection channel.</param>
    /// <param name="v">Number of bits for the red projection channel.</param>
    /// <param name="a">Number of bits for the alpha channel.</param>
    /// <param name="x">Number of bits for the extra channel.</param>
    public YuvPixelFormat(
        AlphaType alphaType,
        ChannelDefinition? y = null,
        ChannelDefinition? u = null,
        ChannelDefinition? v = null,
        ChannelDefinition? a = null,
        ChannelDefinition? x = null) {
        Alpha = alphaType;
        Y = y ?? new();
        U = u ?? new();
        V = v ?? new();
        A = a ?? new();
        X = x ?? new();
        Bpp = new[] {
            Y.Bits + Y.Shift,
            U.Bits + U.Shift,
            V.Bits + V.Shift,
            A.Bits + A.Shift,
            X.Bits + X.Shift,
        }.Max();
    }

    /// <inheritdoc/>
    public bool Equals(YuvPixelFormat? other) {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Y.Equals(other.Y) && U.Equals(other.U) && V.Equals(other.V) && A.Equals(other.A) &&
            X.Equals(other.X) &&
            Alpha == other.Alpha;
    }

    /// <inheritdoc/>
    public override bool Equals(object? obj) => Equals(obj as YuvPixelFormat);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Y, U, V, A, X, (int) Alpha);
}
