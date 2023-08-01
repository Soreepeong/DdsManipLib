using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct F16UnsignedChannel : IFloatChannel, INormalizedChannel<float> {
    public F16UnsignedChannel(int bitOffset, int bitCount) {
        if (bitCount != 16)
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, null);
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

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

    public bool Equals(F16UnsignedChannel other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is F16UnsignedChannel r && Equals(r);

    public override bool Equals(object? obj) => obj is F16UnsignedChannel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(F16UnsignedChannel left, F16UnsignedChannel right) => left.Equals(right);

    public static bool operator !=(F16UnsignedChannel left, F16UnsignedChannel right) => !(left == right);
}