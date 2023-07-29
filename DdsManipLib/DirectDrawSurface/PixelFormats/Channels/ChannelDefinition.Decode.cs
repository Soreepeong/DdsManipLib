using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly partial struct ChannelDefinition {
    /// <summary>
    /// Decode the channel value as a raw value.
    /// </summary>
    public uint DecodeRaw(ReadOnlySpan<byte> data, int bitOffset) {
        var shift = bitOffset % 8;
        data = data[(bitOffset / 8)..];

        var first = 0;
        if (shift != 0) {
            first = data[0];
            data = data[1..];
        }

        var n = BitConverter.IsLittleEndian
            ? Bits switch {
                <= 8 => data[0],
                <= 16 => data[0] | (data[1] << 8),
                <= 24 => data[0] | (data[1] << 8) | (data[2] << 16),
                <= 32 => data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24),
                _ => throw new NotSupportedException(),
            }
            : Bits switch {
                <= 8 => data[0],
                <= 16 => data[1] | (data[0] << 8),
                <= 24 => data[2] | (data[1] << 8) | (data[0] << 16),
                <= 32 => data[3] | (data[2] << 8) | (data[1] << 16) | (data[0] << 24),
                _ => throw new NotSupportedException(),
            };

        if (shift != 0)
            n = (n << (8 - shift)) | (first >> shift);

        var bitmask = (1 << Bits) - 1;
        n &= bitmask;

        return (uint)n;
    }

    /// <summary>
    /// Decode the channel value as a byte. 
    /// </summary>
    public byte DecodeByte(ReadOnlySpan<byte> data, int bitOffset) {
        const int maxValue = byte.MaxValue;
        const int rawMinValue = 0;

        if (Bits == 0)
            return default;

        var n = DecodeRaw(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return (byte) (Bits switch {
                    1 => n == 0 ? 0u : 255u,
                    2 => n * 0b01010101u,
                    3 => (n << 6) | (n << 3) | n,
                    < 8 => (n << Bits) | n,
                    8 => n,
                    _ => n >>> (Bits - 8),
                });
            case ChannelType.Snorm:
                return n >= 1 << (Bits - 1) // is it negative?
                    ? (byte) 0 // clamp to zero
                    : (byte) (Bits switch {
                        1 => 0u,
                        2 => n == 0 ? 0u : 127u,
                        3 => n * 0b01010101u,
                        4 => (n << 6) | (n << 3) | n,
                        < 7 => (n << (Bits - 1)) | n,
                        7 => n,
                        8 => n,
                        _ => n >>> (Bits - 8),
                    });
            case ChannelType.Uint:
                return (byte) Math.Clamp(n, rawMinValue, maxValue);
            case ChannelType.Sint:
                return n >= 1 << (Bits - 1) // is it negative?
                    ? (byte) 0 // clamp to zero
                    : (byte) Math.Clamp(n, rawMinValue, maxValue);
            case ChannelType.F32:
            case ChannelType.Sf16:
            case ChannelType.Uf16:
                throw new InvalidOperationException("Use DecodeFloat for float values.");
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Decode the channel value as a signed byte. 
    /// </summary>
    public sbyte DecodeSByte(ReadOnlySpan<byte> data, int bitOffset) {
        const int maxValue = sbyte.MaxValue;
        const int rawMinValue = sbyte.MinValue;
        const int normalizedMinValue = -sbyte.MaxValue;

        if (Bits == 0)
            return default;

        var n = DecodeRaw(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return (sbyte) (Bits switch {
                    1 => n == 0 ? 0 : 255,
                    2 => n * 0b01010101,
                    3 => (n << 6) | (n << 3) | n,
                    < 8 => (n << Bits) | n,
                    8 => n,
                    _ => n >>> (Bits - 8),
                } >> 1);
            case ChannelType.Snorm: {
                return (sbyte) (Bits switch {
                    1 => n,
                    2 => (n & 2) << 6 | ((n & 1) * 0b01111111),
                    3 => (n & 4) << 5 | (((n & 3) * 0b01010101) >> 1),
                    4 => (n & 8) << 4 | (((n & 7) * 0b01001001) >> 1),
                    5 => (n & 16) << 3 | (((n & 15) * 0b00010001) >> 1),
                    6 => (n & 32) << 2 | (((n & 31) * 0b00100001) >> 1),
                    7 => (n & 64) << 1 | (((n & 63) * 0b01000001) >> 1),
                    8 => n,
                    _ => n >>> (Bits - 8),
                });
            }
            case ChannelType.Uint:
                return (sbyte) Math.Clamp(n, rawMinValue, maxValue);
            case ChannelType.Sint: {
                var halfmask = (1u << (Bits - 1)) - 1u;
                var negative = n >= halfmask;
                if (negative) {
                    n = (~n + 1u) & halfmask;
                    if (n > halfmask)
                        n = halfmask;
                }

                return (sbyte) Math.Clamp(negative ? -n : n, normalizedMinValue, maxValue);
            }
            case ChannelType.F32:
            case ChannelType.Sf16:
            case ChannelType.Uf16:
                throw new InvalidOperationException("Use DecodeFloat for float values.");
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Decode the channel value as a float. 
    /// </summary>
    public float DecodeFloat(ReadOnlySpan<byte> data, int bitOffset) {
        if (Bits == 0)
            return 0f;

        var n = DecodeRaw(data, bitOffset);
        switch (Type) {
            case ChannelType.F32:
            case ChannelType.Typeless when Bits == 32:
                return BitConverter.UInt32BitsToSingle(n);
            case ChannelType.Sf16:
            case ChannelType.Typeless when Bits == 16:
                return (float) BitConverter.UInt16BitsToHalf((ushort) n);
            case ChannelType.Uf16:
                return UInt16UHalfToSingle((ushort) n);
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return 1f * n / ((1 << Bits) - 1);
            case ChannelType.Snorm: {
                var halfmask = (1u << (Bits - 1)) - 1u;
                var negative = n >= halfmask;
                if (negative) {
                    n = (~n + 1u) & halfmask;
                    if (n > halfmask)
                        n = halfmask;
                }

                return (negative ? -1f : 1f) * n / halfmask;
            }
            case ChannelType.Uint:
                return n;
            case ChannelType.Sint:
                return Math.Clamp(n, -((1 << (Bits - 1)) - 1), (1 << (Bits - 1)) - 1);
            default:
                throw new NotSupportedException();
        }
    }
}
