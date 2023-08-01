using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

public readonly partial struct ChannelDefinition {
    /// <summary>
    /// Decode the channel value as a raw value.
    /// </summary>
    public uint ExtractBits(ReadOnlySpan<byte> data, int bitOffset) {
        bitOffset += Shift;
        
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T FitBitsFromUNorm<T>(uint value, int numSourceBits)
        where T : unmanaged, INumber<T>, IBinaryInteger<T>, IBinaryNumber<T>{
        var numTargetBits = T.Zero.GetByteCount() * 8;
        
        // Are we truncating?
        if (numTargetBits <= numSourceBits)
            return T.CreateSaturating(value >> (numSourceBits - numTargetBits));

        var result = T.CreateSaturating(value) << (numTargetBits - numSourceBits);
        for (var numValidMsb = numSourceBits; numValidMsb < numTargetBits; numValidMsb *= 2)
            result |= result >> numValidMsb;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T FitBitsFromSNorm<T>(uint value, int numSourceBits)
        where T : unmanaged, INumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, ISignedNumber<T> {
        if (numSourceBits <= 1)
            return default;

        numSourceBits--;
        var signMask = 1u << numSourceBits;
        var sign = (value & signMask) != 0;
        value &= ~signMask;
        
        var numTargetBits = T.Zero.GetByteCount() * 8 - 1;

        T result;
        // Are we truncating?
        if (numTargetBits <= numSourceBits) {
            result = T.CreateSaturating(value >> (numSourceBits - numTargetBits));
        } else {
            result = T.CreateSaturating(value) << (numTargetBits - numSourceBits);
            for (var numValidMsb = numSourceBits; numValidMsb < numTargetBits; numValidMsb *= 2)
                result |= result >> numValidMsb;
        }

        if (sign)
            result |= T.One << numTargetBits;

        return result;
    }

    public T DecodeNormalizedSigned<T>(ReadOnlySpan<byte> data, int bitOffset, T defaultValue = default) 
        where T : unmanaged, INumber<T>, ISignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
        if (Bits == 0)
            return defaultValue;

        var n = ExtractBits(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return FitBitsFromUNorm<T>(n, Bits) >>> 1;
            case ChannelType.Snorm:
                return FitBitsFromSNorm<T>(n, Bits);
            case ChannelType.F32:
            case ChannelType.Sf16:
            case ChannelType.Uf16:
                return T.Clamp(DecodeNumber(data, bitOffset, defaultValue), T.NegativeOne, T.One);
            case ChannelType.Uint:
            case ChannelType.Sint:
                throw new InvalidOperationException("Uint/Sint types are not normalized.");
            default:
                throw new NotSupportedException();
        }
    }

    public T DecodeNormalizedUnsigned<T>(ReadOnlySpan<byte> data, int bitOffset, T defaultValue = default) 
        where T : unmanaged, INumber<T>, IUnsignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
        if (Bits == 0)
            return defaultValue;

        var n = ExtractBits(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return FitBitsFromUNorm<T>(n, Bits);
            case ChannelType.Snorm:
                return T.CreateTruncating(FitBitsFromSNorm<int>(n, Bits));
            case ChannelType.F32:
            case ChannelType.Sf16:
            case ChannelType.Uf16:
                return T.Clamp(DecodeNumber(data, bitOffset, defaultValue), T.Zero, T.One);
            case ChannelType.Uint:
            case ChannelType.Sint:
                throw new InvalidOperationException("Uint/Sint types are not normalized.");
            default:
                throw new NotSupportedException();
        }
    }
    
    public float DecodeNormalizedFloat(ReadOnlySpan<byte> data, int bitOffset, float defaultValue = default)  {
        if (Bits == 0)
            return defaultValue;

        var n = ExtractBits(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                return 1f * n / ((1u << Bits) - 1);
            case ChannelType.Snorm: {
                var halfmask = (1u << (Bits - 1)) - 1u;
                var negative = n >= halfmask;
                if (negative) {
                    n = (~n + 1u) & halfmask;
                    if (n > halfmask)
                        return -1f;
                    return -1f * n / halfmask;
                }

                return 1f * n / halfmask;
            }
            case ChannelType.F32:
            case ChannelType.Sf16:
            case ChannelType.Uf16:
                return float.Clamp(DecodeNumber(data, bitOffset, defaultValue), -1f, 1f);
            case ChannelType.Uint:
            case ChannelType.Sint:
                throw new InvalidOperationException("Uint/Sint types are not normalized.");
            default:
                throw new NotSupportedException();
        }
    }
    
    public T DecodeNumber<T>(ReadOnlySpan<byte> data, int bitOffset, T defaultValue = default) 
        where T : unmanaged, INumber<T> {
        if (Bits == 0)
            return defaultValue;

        var n = ExtractBits(data, bitOffset);
        switch (Type) {
            case ChannelType.Typeless:
                return T.CreateSaturating(n);
            case ChannelType.F32:
                return T.CreateTruncating(BitConverter.UInt32BitsToSingle(n));
            case ChannelType.Sf16:
                return T.CreateTruncating(BitConverter.UInt16BitsToHalf((ushort) n));
            case ChannelType.Uf16:
                return T.CreateTruncating(UInt16UHalfToSingle((ushort) n));
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
            case ChannelType.Uint:
                return T.CreateTruncating(n);
            case ChannelType.Snorm:
            case ChannelType.Sint:
                var halfmask = (1u << (Bits - 1)) - 1u;
                var negative = n >= halfmask;
                if (negative) {
                    n = (~n + 1u) & halfmask;
                    if (n > halfmask)
                        n = halfmask;
                    return T.CreateTruncating(-(int)n);
                }

                return T.CreateTruncating(n);
            default:
                throw new NotSupportedException();
        }
    }
}
