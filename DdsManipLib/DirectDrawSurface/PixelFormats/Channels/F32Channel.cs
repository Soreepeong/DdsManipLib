using System;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct F32Channel : IFloatChannel, INormalizedChannel<float> {
    public F32Channel(int bitOffset, int bitCount) {
        if (bitCount != 32)
            throw new ArgumentOutOfRangeException(nameof(bitCount), bitCount, null);
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public bool HasSignBit => true;
    public int ExponentBitCount => 8;
    public int MantissaBitCount => 23;

    public float ReadValue(ReadOnlySpan<byte> span, int shift) =>
        BitConverter.UInt32BitsToSingle(((IChannel<float>) this).ReadRawUInt32(span, shift));

    public void WriteValue(Span<byte> span, int shift, float value) =>
        ((IChannel<float>) this).WriteRawUInt32(span, shift, BitConverter.SingleToUInt32Bits(value));

    public float ToNormalizedValue(float value) => float.Clamp(value, -1f, 1f);
    public float FromNormalizedValue(float value) => float.Clamp(value, -1f, 1f);

    public bool Equals(F32Channel other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is F32Channel r && Equals(r);

    public override bool Equals(object? obj) => obj is F32Channel other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(F32Channel left, F32Channel right) => left.Equals(right);

    public static bool operator !=(F32Channel left, F32Channel right) => !(left == right);
}