using System;
using System.Diagnostics;
using System.Numerics;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;
using BindingFlags = System.Reflection.BindingFlags;

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

#pragma warning disable CS1591

public interface IChannel {
    public int BitOffset { get; }
    public int BitCount { get; }
}

public interface IChannel<T> : IChannel
    where T : unmanaged, INumber<T> {
    public T ReadValue(ReadOnlySpan<byte> span, int shift);
    public void WriteValue(Span<byte> span, int shift, T value);

    public sealed uint ReadRawUInt32(ReadOnlySpan<byte> span, int shift) {
        shift += BitOffset;
        span = span[(shift / 8)..];
        shift %= 8;

        var tmp = 0ul;
        for (var i = 0; i < shift + BitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        return (uint) (tmp >> shift) & ((1u << BitCount) - 1);
    }

    public sealed void WriteRawUInt32(Span<byte> span, int shift, uint value) {
        shift += BitOffset;
        span = span[(shift / 8)..];
        shift %= 8;

        var tmp = 0ul;
        for (var i = 0; i < shift + BitCount; i += 8)
            tmp |= (ulong) span[i] << (i * 8);

        tmp &= ~(((1ul << BitCount) - 1) << shift);
        tmp |= value << shift;
        for (var i = 0; i < shift + BitCount; i += 8)
            span[i] = (byte) (tmp >> (i * 8));
    }
}

public interface ISIntChannel<T> : IChannel<T>
    where T : unmanaged, INumber<T>, ISignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
    T IChannel<T>.ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ReadRawUInt32(span, shift);
        return (n & (1 << (BitCount - 1))) == 0
            ? T.CreateSaturating(n)
            : -T.CreateSaturating((~n + 1) & ((1u << BitCount) - 1));
    }

    void IChannel<T>.WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            WriteRawUInt32(span, shift, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = -int.CreateSaturating(value);
            Debug.Assert(n > 0);
            WriteRawUInt32(span, shift, (uint) n | (1u << (BitCount - 1)));
        }
    }
}

public interface IUIntChannel<T> : IChannel<T>
    where T : unmanaged, INumber<T>, IUnsignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
    T IChannel<T>.ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ReadRawUInt32(span, shift);
        var bitmask = (1u << BitCount) - 1;
        n &= bitmask;

        return (n & (1 << (BitCount - 1))) == 0
            ? T.CreateSaturating(n)
            : -T.CreateSaturating((~n + 1) & bitmask);
    }

    void IChannel<T>.WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            WriteRawUInt32(span, shift, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = Math.Min(uint.CreateSaturating(~value + T.One), 1u << (BitCount - 1));
            WriteRawUInt32(span, shift, (~n + 1) & ((1u << BitCount) - 1u));
        }
    }
}

public interface IFloatChannel : IChannel<float> {
    public bool HasSignBit { get; }
    public int ExponentBitCount { get; }
    public int MantissaBitCount { get; }
}

public interface INormalizedChannel<T> : IChannel<T>
    where T : unmanaged, INumber<T> {
    public float ToNormalizedValue(T value);
    public T FromNormalizedValue(float value);
}

internal struct ChannelBitsDefinition {
    public int Offset;
    public int Count;

    public ChannelBitsDefinition(int offset, int count) {
        Offset = offset;
        Count = count;
    }
}

public readonly struct TypelessChannel<T> : IChannel<T>
    where T : unmanaged, IBinaryInteger<T>, IMinMaxValue<T> {
    public TypelessChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    T IChannel<T>.ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ((IChannel<T>) this).ReadRawUInt32(span, shift);
        var bitmask = (1u << BitCount) - 1;
        n &= bitmask;

        return (n & (1 << (BitCount - 1))) == 0
            ? T.CreateSaturating(n)
            : -T.CreateSaturating((~n + 1) & bitmask);
    }

    void IChannel<T>.WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            ((IChannel<T>) this).WriteRawUInt32(span, shift, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = Math.Min(uint.CreateSaturating(~value + T.One), 1u << (BitCount - 1));
            ((IChannel<T>) this).WriteRawUInt32(span, shift, (~n + 1) & ((1u << BitCount) - 1u));
        }
    }
}

public readonly struct UIntChannel<T> : IUIntChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public UIntChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }
}

public readonly struct SIntChannel<T> : ISIntChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    private readonly ChannelBitsDefinition _definition;

    public SIntChannel(int bitOffset, int bitCount) {
        _definition = new(bitOffset, bitCount);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;
}

public readonly struct UNormChannel<T> : IUIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    private readonly ChannelBitsDefinition _definition;

    public UNormChannel(int bitOffset, int bitCount) {
        _definition = new(bitOffset, bitCount);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;
    public float ToNormalizedValue(T value) => float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, 0f, 1f) * float.CreateSaturating(T.MaxValue));
}

public readonly struct UNormSrgbChannel<T> : IUIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    private readonly ChannelBitsDefinition _definition;

    public UNormSrgbChannel(int bitOffset, int bitCount) {
        _definition = new(bitOffset, bitCount);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;
    public float ToNormalizedValue(T value) => float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, 0f, 1f) * float.CreateSaturating(T.MaxValue));
}

public readonly struct SNormChannel<T> : ISIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    private readonly ChannelBitsDefinition _definition;

    public SNormChannel(int bitOffset, int bitCount) {
        _definition = new(bitOffset, bitCount);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;
    public float ToNormalizedValue(T value) => value == T.MinValue ? -1f : float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, -1f, 1f) * float.CreateSaturating(T.MaxValue));
}

public readonly struct F32Channel : IFloatChannel, INormalizedChannel<float> {
    private readonly ChannelBitsDefinition _definition;

    public F32Channel(int bitOffset) {
        _definition = new(bitOffset, 32);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;

    public bool HasSignBit => true;
    public int ExponentBitCount => 8;
    public int MantissaBitCount => 23;

    public float ReadValue(ReadOnlySpan<byte> span, int shift) =>
        BitConverter.UInt32BitsToSingle(((IChannel<float>) this).ReadRawUInt32(span, shift));

    public void WriteValue(Span<byte> span, int shift, float value) =>
        ((IChannel<float>) this).WriteRawUInt32(span, shift, BitConverter.SingleToUInt32Bits(value));

    public float ToNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
    public float FromNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
}

public readonly struct F16Channel : IFloatChannel, INormalizedChannel<float> {
    private readonly ChannelBitsDefinition _definition;

    public F16Channel(int bitOffset) {
        _definition = new(bitOffset, 16);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;

    public bool HasSignBit => true;
    public int ExponentBitCount => 5;
    public int MantissaBitCount => 10;

    public float ReadValue(ReadOnlySpan<byte> span, int shift) =>
        (float) BitConverter.UInt16BitsToHalf((ushort) ((IChannel<float>) this).ReadRawUInt32(span, shift));

    public void WriteValue(Span<byte> span, int shift, float value) =>
        ((IChannel<float>) this).WriteRawUInt32(span, shift, BitConverter.HalfToUInt16Bits((Half) value));

    public float ToNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
    public float FromNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
}

public readonly struct F16UnsignedChannel : IFloatChannel, INormalizedChannel<float> {
    private readonly ChannelBitsDefinition _definition;

    public F16UnsignedChannel(int bitOffset) {
        _definition = new(bitOffset, 16);
    }

    public int BitOffset => _definition.Offset;
    public int BitCount => _definition.Count;

    public bool HasSignBit => false;
    public int ExponentBitCount => 5;
    public int MantissaBitCount => 11;

    public float ReadValue(ReadOnlySpan<byte> span, int shift) {
        var v = (ushort) ((IChannel<float>) this).ReadRawUInt32(span, shift);
        var exponentU16 = (v & 0xF800u) >> 11; // 5 bits
        var exponent = exponentU16 + 15u; // [-16, 15]
        var exponent32 = exponent - 127u;

        var mantissaU16 = v & 0x7FFu; // 11 bits
        var mantissa32 = (mantissaU16 << 13) | (mantissaU16 << 2) | (mantissaU16 >> 9); // 24 bits

        return BitConverter.UInt32BitsToSingle((exponent32 << 24) | mantissa32);
    }

    public void WriteValue(Span<byte> span, int shift, float value) {
        var fvalue = BitConverter.SingleToUInt32Bits(value);
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

        ((IChannel<float>) this).WriteRawUInt32(span, shift, ((uint) (exponent + 15) << 11) | mantissaU16);
    }

    public float ToNormalizedValue(float value) => float.Clamp(value, 0f, 1f);
    public float FromNormalizedValue(float value) => float.Clamp(value, 0f, 1f);
}

public interface IPixelFormat {
    public int Bpp { get; }
}

public interface IChannelSet : IPixelFormat { }

public interface IRedChannelSet : IChannelSet {
    public IChannel Red { get; }
}

public interface IGreenChannelSet : IChannelSet {
    public IChannel Green { get; }
}

public interface IRedGreenChannelSet : IRedChannelSet, IGreenChannelSet { }

public interface IRgbChannelSet : IRedGreenChannelSet {
    public IChannel Blue { get; }
}

public interface IAlphaChannelSet : IChannelSet {
    public IChannel Alpha { get; }
    public AlphaType AlphaType { get; }
}

public interface ILuminanceChannelSet : IChannelSet {
    public IChannel Luminance { get; }
}

public interface IYuvChannelSet : ILuminanceChannelSet {
    public IChannel ChromaBlue { get; }
    public IChannel ChromaRed { get; }
}

public interface IDepthChannelSet : IChannelSet {
    public IChannel Depth { get; }
}

public interface IDepthStencilChannelSet : IDepthChannelSet {
    public IChannel Stencil { get; }
}

public interface IBlockCompressionPixelFormat : IPixelFormat {
    public AlphaType AlphaType { get; }
    public int Version { get; }
    public int BlockByteCount { get; }
    public IChannelSet DecompressedPixelFormat { get; }
}

public abstract class PixelFormatAttribute : Attribute, IPixelFormat {
    public abstract int Bpp { get; }
}

public class AlphaPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IAlphaChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe AlphaPixelFormatAttribute(int alpha, AlphaType alphaType) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, alpha);
        Alpha = tmp;
        AlphaType = alphaType;
    }

    public IChannel Alpha { get; }
    public AlphaType AlphaType { get; }
    public override int Bpp => Alpha.BitCount;
}

public class DepthPixelFormatAttribute<TDepth, TDepthChannel>
    : PixelFormatAttribute, IDepthChannelSet
    where TDepth : unmanaged, INumber<TDepth>
    where TDepthChannel : unmanaged, IChannel<TDepth> {
    public unsafe DepthPixelFormatAttribute(int depth) {
        var tmp = default(TDepthChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, depth);
        Depth = tmp;
    }

    public IChannel Depth { get; }
    public override int Bpp => Depth.BitCount;
}

public class DepthStencilPixelFormatAttribute<TDepth, TDepthChannel, TStencil, TStencilChannel>
    : DepthPixelFormatAttribute<TDepth, TDepthChannel>
    where TDepth : unmanaged, INumber<TDepth>
    where TStencil : unmanaged, INumber<TStencil>
    where TDepthChannel : unmanaged, IChannel<TDepth>
    where TStencilChannel : unmanaged, IChannel<TStencil> {
    public unsafe DepthStencilPixelFormatAttribute(int depth, int stencil) : base(depth) {
        var tmp = default(TStencilChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, stencil);
        Stencil = tmp;
    }

    public IChannel Stencil { get; }
    public override int Bpp => Depth.BitCount + Stencil.BitCount;
}

public class DsxPixelFormatAttribute<TDepth, TDepthChannel, TStencil, TStencilChannel, TX, TXChannel>
    : DepthStencilPixelFormatAttribute<TDepth, TDepthChannel, TStencil, TStencilChannel>
    where TDepth : unmanaged, INumber<TDepth>
    where TStencil : unmanaged, INumber<TStencil>
    where TX : unmanaged, INumber<TX>
    where TDepthChannel : unmanaged, IChannel<TDepth>
    where TStencilChannel : unmanaged, IChannel<TStencil>
    where TXChannel : unmanaged, IChannel<TX> {
    public unsafe DsxPixelFormatAttribute(int depth, int stencil, int x) : base(depth, stencil) {
        var tmp = default(TXChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, x);
        X = tmp;
    }

    public IChannel X { get; }
    public override int Bpp => Depth.BitCount + Stencil.BitCount + X.BitCount;
}

public class RedPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IRedChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe RedPixelFormatAttribute(int red) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, red);
        Red = tmp;
    }

    public IChannel Red { get; }
    public override int Bpp => Red.BitCount;
}

public class RxPixelFormatAttribute<T, TChannel, TX>
    : RedPixelFormatAttribute<T, TChannel>
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T>
    where TX : unmanaged, IBinaryInteger<TX>, IMinMaxValue<TX> {
    public unsafe RxPixelFormatAttribute(int red, int x1) : base(red) {
        var tmp = default(TypelessChannel<TX>);
        *(ChannelBitsDefinition*) &tmp = new(x1, base.Bpp);
        X = tmp;
    }

    public IChannel X { get; }
    public override int Bpp => Red.BitCount + X.BitCount;
}

public class RxxPixelFormatAttribute<T, TChannel, TX1, TX2>
    : RxPixelFormatAttribute<T, TChannel, TX1>
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T>
    where TX1 : unmanaged, IBinaryInteger<TX1>, IMinMaxValue<TX1>
    where TX2 : unmanaged, IBinaryInteger<TX2>, IMinMaxValue<TX2> {
    public unsafe RxxPixelFormatAttribute(int red, int x1, int x2) : base(red, x1) {
        var tmp2 = default(TypelessChannel<TX2>);
        *(ChannelBitsDefinition*) &tmp2 = new(x2, base.Bpp + x1);
        X2 = tmp2;
    }

    public IChannel X2 { get; }
    public override int Bpp => Red.BitCount + X.BitCount + X2.BitCount;
}

public class XgPixelFormatAttribute<T, TChannel, TX>
    : PixelFormatAttribute, IGreenChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T>
    where TX : unmanaged, IBinaryInteger<TX>, IMinMaxValue<TX> {
    public unsafe XgxPixelFormatAttribute(int x, int green) {
        var tmp1 = default(TypelessChannel<TX>);
        *(ChannelBitsDefinition*) &tmp1 = new(x, 0);
        X = tmp1;

        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(green, x);
        Green = tmp;
    }

    public IChannel X { get; }
    public IChannel Green { get; }
    public override int Bpp => X.BitCount + Green.BitCount;
}

public class XgxPixelFormatAttribute<T, TChannel, TX1, TX2>
    : XgPixelFormatAttribute<T, TChannel, TX1>
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T>
    where TX1 : unmanaged, IBinaryInteger<TX1>, IMinMaxValue<TX1>
    where TX2 : unmanaged, IBinaryInteger<TX2>, IMinMaxValue<TX2> {
    public unsafe XgxPixelFormatAttribute(int x1, int green, int x2) : base(x1, green) {
        var tmp2 = default(TypelessChannel<TX2>);
        *(ChannelBitsDefinition*) &tmp2 = new(x2, x1 + green);
        X2 = tmp2;
    }

    public IChannel X2 { get; }
    public override int Bpp => X.BitCount + Green.BitCount + X2.BitCount;
}

public class RedGreenPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IRedGreenChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe RedGreenPixelFormatAttribute(int red, int green) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, red);
        Red = tmp;
        *(ChannelBitsDefinition*) &tmp = new(red, green);
        Green = tmp;
    }

    public IChannel Red { get; }
    public IChannel Green { get; }
    public override int Bpp => Red.BitCount + Green.BitCount;
}

public class RgxPixelFormatAttribute<T, TChannel, TX>
    : RedGreenPixelFormatAttribute<T, TChannel>
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T>
    where TX : unmanaged, IBinaryInteger<TX>, IMinMaxValue<TX> {
    public unsafe RgxPixelFormatAttribute(int red, int green, int x) : base(red, green) {
        var tmp = default(TypelessChannel<TX>);
        *(ChannelBitsDefinition*) &tmp = new(x, base.Bpp);
        X = tmp;
    }

    public IChannel X { get; }
    public override int Bpp => Red.BitCount + Green.BitCount + X.BitCount;
}

public class RgbPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IRgbChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe RgbPixelFormatAttribute(int red, int green, int blue) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, red);
        Red = tmp;
        *(ChannelBitsDefinition*) &tmp = new(red, green);
        Green = tmp;
        *(ChannelBitsDefinition*) &tmp = new(red + green, blue);
        Blue = tmp;
    }

    public IChannel Red { get; }
    public IChannel Green { get; }
    public IChannel Blue { get; }
    public override int Bpp => Red.BitCount + Green.BitCount + Blue.BitCount;
}

public class BgrPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IRgbChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe BgrPixelFormatAttribute(int blue, int green, int red) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(0, blue);
        Blue = tmp;
        *(ChannelBitsDefinition*) &tmp = new(blue, green);
        Green = tmp;
        *(ChannelBitsDefinition*) &tmp = new(blue + green, red);
        Red = tmp;
    }

    public IChannel Blue { get; }
    public IChannel Green { get; }
    public IChannel Red { get; }
    public override int Bpp => Blue.BitCount + Green.BitCount + Red.BitCount;
}

public class BgrxPixelFormatAttribute<T, TChannel>
    : BgrPixelFormatAttribute<T, TChannel>
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe BgrxPixelFormatAttribute(int blue, int green, int red, int x) : base(blue, green, red) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(x, base.Bpp);
        X = tmp;
    }

    public IChannel X { get; }
    public override int Bpp => Blue.BitCount + Green.BitCount + Red.BitCount + X.BitCount;
}

public class RgbaPixelFormatAttribute<T, TChannel>
    : RgbPixelFormatAttribute<T, TChannel>, IAlphaChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe RgbaPixelFormatAttribute(int red, int green, int blue, int alpha, AlphaType alphaType)
        : base(red, green, blue) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(base.Bpp, alpha);
        Alpha = tmp;
        AlphaType = alphaType;
    }

    public IChannel Alpha { get; }
    public AlphaType AlphaType { get; }
    public override int Bpp => base.Bpp + Alpha.BitCount;
}

public class RedBcPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IBlockCompressionPixelFormat
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public RedBcPixelFormatAttribute(int version, int blockByteCount, int bpp, int red) {
        Version = version;
        BlockByteCount = blockByteCount;
        Bpp = bpp;
        DecompressedPixelFormat = new RedPixelFormatAttribute<T, TChannel>(red);
    }

    public AlphaType AlphaType => AlphaType.None;
    public int Version { get; }
    public int BlockByteCount { get; }
    public override int Bpp { get; }
    public IChannelSet DecompressedPixelFormat { get; }
}

public class RedGreenBcPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IBlockCompressionPixelFormat
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public RedGreenBcPixelFormatAttribute(int version, int blockByteCount, int bpp, int red, int green) {
        Version = version;
        BlockByteCount = blockByteCount;
        Bpp = bpp;
        DecompressedPixelFormat = new RedGreenPixelFormatAttribute<T, TChannel>(red, green);
    }

    public AlphaType AlphaType => AlphaType.None;
    public int Version { get; }
    public int BlockByteCount { get; }
    public override int Bpp { get; }
    public IChannelSet DecompressedPixelFormat { get; }
}

public class RgbBcPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IBlockCompressionPixelFormat
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public RgbBcPixelFormatAttribute(int version, int blockByteCount, int bpp, int red, int green, int blue) {
        Version = version;
        BlockByteCount = blockByteCount;
        Bpp = bpp;
        DecompressedPixelFormat = new RgbPixelFormatAttribute<T, TChannel>(red, green, blue);
    }

    public AlphaType AlphaType => AlphaType.None;
    public int Version { get; }
    public int BlockByteCount { get; }
    public override int Bpp { get; }
    public IChannelSet DecompressedPixelFormat { get; }
}

public class RgbaBcPixelFormatAttribute<T, TChannel>
    : PixelFormatAttribute, IBlockCompressionPixelFormat
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public RgbaBcPixelFormatAttribute(int version, int blockByteCount, int bpp, int red, int green, int blue, int alpha, AlphaType alphaType) {
        AlphaType = alphaType;
        Version = version;
        BlockByteCount = blockByteCount;
        Bpp = bpp;
        DecompressedPixelFormat = new RgbaPixelFormatAttribute<T, TChannel>(red, green, blue, alpha, alphaType);
    }

    public AlphaType AlphaType { get; }
    public int Version { get; }
    public int BlockByteCount { get; }
    public override int Bpp { get; }
    public IChannelSet DecompressedPixelFormat { get; }
}

public class BgraPixelFormatAttribute<T, TChannel>
    : RgbPixelFormatAttribute<T, TChannel>, IAlphaChannelSet
    where T : unmanaged, INumber<T>
    where TChannel : unmanaged, IChannel<T> {
    public unsafe BgraPixelFormatAttribute(int blue, int green, int red, int alpha, AlphaType alphaType)
        : base(blue, green, red) {
        var tmp = default(TChannel);
        *(ChannelBitsDefinition*) &tmp = new(base.Bpp, alpha);
        Alpha = tmp;
        AlphaType = alphaType;
    }

    public IChannel Alpha { get; }
    public AlphaType AlphaType { get; }
    public override int Bpp => base.Bpp + Alpha.BitCount;
}
