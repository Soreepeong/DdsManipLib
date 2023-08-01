using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct F16Channel : IFloatChannel, INormalizedChannel<float> {
    public F16Channel(int bitOffset, int bitCount) {
        if (bitCount != 16)
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, null);
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public bool HasSignBit => true;
    public int ExponentBitCount => 5;
    public int MantissaBitCount => 10;

    public float ReadValue(ReadOnlySpan<byte> span, int shift) =>
        (float) BitConverter.UInt16BitsToHalf((ushort) ((IChannel<float>) this).ReadRawUInt32(span, shift));

    public void WriteValue(Span<byte> span, int shift, float value) =>
        ((IChannel<float>) this).WriteRawUInt32(span, shift, BitConverter.HalfToUInt16Bits((Half) value));

    public float ToNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
    public float FromNormalizedValue(float value) => float.Clamp(value, -1f, 1f);

    public bool Equals(F16Channel other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is F16Channel r && Equals(r);

    public override bool Equals(object? obj) => obj is F16Channel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(F16Channel left, F16Channel right) => left.Equals(right);

    public static bool operator !=(F16Channel left, F16Channel right) => !(left == right);
}