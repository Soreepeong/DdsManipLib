using System;
using System.Diagnostics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

/// <summary>
/// Defines a channel.
/// </summary>
public readonly partial struct ChannelDefinition : IEquatable<ChannelDefinition> {
    /// <summary>
    /// How the channel value should be interpreted.
    /// </summary>
    public readonly ChannelType Type;

    /// <summary>
    /// Number of bits to shift to retrieve value of this channel.
    /// </summary>
    public readonly byte Shift;

    /// <summary>
    /// Number of bits used to represent this channel.
    /// </summary>
    public readonly byte Bits;

    /// <summary>
    /// Bitmask for extracting the channel value.
    /// </summary>
    public readonly uint Mask;

    /// <summary>
    /// Construct a new empty channel defintiion.
    /// </summary>
    public ChannelDefinition() {
        Type = ChannelType.Typeless;
        Mask = Shift = Bits = 0;
    }

    /// <summary>
    /// Construct a channel definition with the given parameters.
    /// </summary>
    public ChannelDefinition(ChannelType type, int shift, int bits, uint? mask = default) {
        if (bits != 0) {
            if (type == ChannelType.F32 && bits != 32)
                throw new ArgumentException($"When {nameof(shift)} is F32, {nameof(bits)} must be 32.");
            if (type == ChannelType.F16 && bits != 16)
                throw new ArgumentException($"When {nameof(shift)} is F16, {nameof(bits)} must be 16.");
            if (type == ChannelType.Uf16 && bits != 16)
                throw new ArgumentException($"When {nameof(shift)} is Uf16, {nameof(bits)} must be 16.");
            if (bits is < 0 or > 32)
                throw new ArgumentOutOfRangeException(nameof(Bits), Bits, "Bits must be contained within [0, 32].");
        }

        mask ??= bits switch {
            32 => uint.MaxValue,
            _ => (1u << bits) - 1u,
        };
        switch (bits) {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(bits), bits, null);
            case 0:
                Debug.Assert(mask == 0);
                Type = ChannelType.Typeless;
                Mask = Shift = Bits = 0;
                break;
            default:
                Debug.Assert(mask != 0);
                Type = type;
                Shift = (byte) shift;
                Bits = (byte) bits;
                Mask = mask.Value;
                break;
        }
    }

    /// <summary>
    /// Get whether the channel has no stored value.
    /// </summary>
    public bool IsEmpty => Bits == 0;

    /// <inheritdoc/>
    public override string ToString() => $"{(ulong) Mask << Shift:X} ({Type})";

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine((int) Type, Shift, Bits, Mask);

    /// <inheritdoc/>
    public bool Equals(ChannelDefinition other) =>
        Type == other.Type && Shift == other.Shift && Bits == other.Bits && Mask == other.Mask;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ChannelDefinition definition && Equals(definition);

    /// <summary>
    /// Check for equality.
    /// </summary>
    public static bool operator ==(ChannelDefinition left, ChannelDefinition right) => left.Equals(right);

    /// <summary>
    /// Check for inequality.
    /// </summary>
    public static bool operator !=(ChannelDefinition left, ChannelDefinition right) => !(left == right);

    // TODO: verify if correct
    private static float UInt16UHalfToSingle(ushort v) {
        var exponentU16 = (v & 0xF800u) >> 11; // 5 bits
        var exponent = exponentU16 + 15u; // [-16, 15]
        var exponent32 = exponent - 127u;

        var mantissaU16 = v & 0x7FFu; // 11 bits
        var mantissa32 = (mantissaU16 << 13) | (mantissaU16 << 2) | (mantissaU16 >> 9); // 24 bits

        return BitConverter.UInt32BitsToSingle((exponent32 << 24) | mantissa32);
    }

    // TODO: verify if correct
    private static ushort SingleToUInt16UHalf(float v) {
        var fvalue = BitConverter.SingleToUInt32Bits(v);
        var exponent32 = (fvalue >> 23) & 0xFF;
        var exponent = (int) exponent32 - 127;
        var mantissaU16 = (fvalue & 0x7FFFFF) >> 12;
        switch (exponent) {
            case < -32:
                exponent = 0;
                mantissaU16 = 0;
                break;
            case > 31:
                exponent = 31;
                mantissaU16 = 0x7FFF;
                break;
        }

        return (ushort) (((uint) (exponent + 15) << 11) | mantissaU16);
    }

    /// <summary>
    /// Create a channel definition from bitmask.
    /// </summary>
    public static ChannelDefinition FromMask(ChannelType channelType, uint mask) {
        if (mask == 0)
            return default;

        var shift = 0;
        var bits = 0;

        while (mask != 0 && (mask & 1) == 0) {
            shift++;
            mask >>= 1;
        }

        while (mask != 0) {
            bits++;
            mask >>= 1;
        }

        return new(channelType, shift, bits);
    }
}
