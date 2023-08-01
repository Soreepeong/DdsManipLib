using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

public readonly partial struct ChannelDefinition {
    /// <summary>
    /// Encode the channel value as a raw value.
    /// </summary>
    public void EncodeRaw(Span<byte> data, int bitOffset, uint value) {
        bitOffset += Shift;
        
        var shift = bitOffset % 8;
        data = data[(bitOffset / 8)..];

        if (shift == 0) {
            switch (Bits) {
                case 8:
                    data[0] = (byte) value;
                    return;
                case 16:
                    data[0] = (byte) value;
                    data[1] = (byte) (value >> 8);
                    return;
                case 24:
                    data[0] = (byte) value;
                    data[1] = (byte) (value >> 8);
                    data[2] = (byte) (value >> 16);
                    return;
                case 32:
                    data[0] = (byte) value;
                    data[1] = (byte) (value >> 8);
                    data[2] = (byte) (value >> 16);
                    data[3] = (byte) (value >> 24);
                    return;
            }
        }

        var nb = (Bits + shift + 7) / 8;
        switch (nb) {
            case < 1:
                break;
            case 1:
                data[0] = (byte) ((data[0] & ~(((1u << Bits) - 1) << shift)) | (value << shift));
                break;
            default:
                data[0] &= (byte) ~(0xFFu << shift);
                data[nb - 1] &= (byte) (0xFFu << ((shift + Bits) % 8));
                if (nb > 2)
                    data[1..(nb - 1)].Clear();

                data[0] |= (byte) (value << shift);
                value >>>= shift;

                for (var i = 1; i < nb - 1; i++) {
                    data[i] = (byte) value;
                    value >>>= 8;
                }

                data[nb - 1] |= (byte) value;
                break;
        }
    }

    /// <summary>
    /// Encode the given value.
    /// </summary>
    public void EncodeByte(Span<byte> data, int bitOffset, byte value) {
        if (Bits == 0)
            return;

        switch (Type) {
            case ChannelType.Snorm:
                EncodeRaw(data, bitOffset, (uint) (value * ((1 << (Bits - 1)) - 1) / 255));
                break;
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                EncodeRaw(data, bitOffset, (uint) (value * ((1 << Bits) - 1) / 255));
                break;
            case ChannelType.Sint:
                EncodeRaw(data, bitOffset, (uint) Math.Clamp(value, 0, (1 << (Bits - 1)) - 1));
                break;
            case ChannelType.Uint:
                EncodeRaw(data, bitOffset, (uint) Math.Clamp(value, 0, (1 << Bits) - 1));
                break;
            case ChannelType.F32:
            case ChannelType.F16:
            case ChannelType.Uf16:
                throw new InvalidOperationException("Use EncodeFloat for float values.");
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Encode the given value.
    /// </summary>
    public void EncodeSByte(Span<byte> data, int bitOffset, sbyte value) {
        if (Bits == 0)
            return;

        switch (Type) {
            case ChannelType.Snorm:
                if (value == -128)
                    value = -127;
                EncodeRaw(data, bitOffset, (uint) Math.Abs(value * ((1u << (Bits - 1)) - 1u) / 127u) | (value < 0 ? 1u << (Bits - 1) : 0));
                break;
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                EncodeRaw(data, bitOffset, (uint) (value < 0 ? 0 : value * ((1 << Bits) - 1) / 127));
                break;
            case ChannelType.Sint:
                if (value == -128)
                    value = -127;
                EncodeRaw(data, bitOffset, Math.Min((uint) Math.Abs(value), (1u << (Bits - 1)) - 1u) | (value < 0 ? 1u << (Bits - 1) : 0));
                break;
            case ChannelType.Uint:
                EncodeRaw(data, bitOffset, (uint) Math.Clamp(value, 0, (1 << Bits) - 1));
                break;
            case ChannelType.F32:
            case ChannelType.F16:
            case ChannelType.Uf16:
                throw new InvalidOperationException("Use EncodeFloat for float values.");
            default:
                throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Encode the given value.
    /// </summary>
    public void EncodeFloat(Span<byte> data, int bitOffset, float value) {
        if (Bits == 0)
            return;

        switch (Type) {
            case ChannelType.Snorm: {
                value = MathF.Round(Math.Clamp(value, -1f, 1f) * ((1 << (Bits - 1)) - 1));
                var n = (uint) MathF.Abs(value);
                if (value < 0)
                    n |= 1u << (Bits - 1);
                EncodeRaw(data, bitOffset, n);
                break;
            }
            case ChannelType.Typeless:
            case ChannelType.Unorm:
            case ChannelType.UnormSrgb:
                EncodeRaw(data, bitOffset, (uint) MathF.Round(Math.Clamp(value, 0f, 1f) * ((1 << (Bits - 1)) - 1)));
                break;
            case ChannelType.Sint: {
                value = Math.Clamp(MathF.Round(value), -((1 << (Bits - 1)) - 1), (1 << (Bits - 1)) - 1);
                var n = (uint) MathF.Abs(value);
                if (value < 0)
                    n |= 1u << (Bits - 1);
                EncodeRaw(data, bitOffset, n);
                break;
            }
            case ChannelType.Uint:
                EncodeRaw(data, bitOffset, (uint) Math.Clamp(Math.Round(value), 0, (1 << Bits) - 1));
                break;
            case ChannelType.F32:
                EncodeRaw(data, bitOffset, BitConverter.SingleToUInt32Bits(value));
                break;
            case ChannelType.F16:
                EncodeRaw(data, bitOffset, BitConverter.HalfToUInt16Bits((Half) value));
                break;
            case ChannelType.Uf16:
                EncodeRaw(data, bitOffset, SingleToUInt16UHalf(value));
                break;
            default:
                throw new NotSupportedException();
        }
    }

    public void EncodeNumber<T>(Span<byte> data, int bitOffset, T value)
        where T : unmanaged, INumber<T> {
        if (Bits == 0)
            return;
    }
}
