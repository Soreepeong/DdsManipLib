using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct SNormChannel<T> : ISIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public SNormChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }
    public float ToNormalizedValue(T value) => value == T.MinValue ? -1f : float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, -1f, 1f) * float.CreateSaturating(T.MaxValue));

    public T ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ChannelUtilities.ReadRawUInt32(span, BitOffset + shift, BitCount);
        var bm = 1 << (BitCount - 1);
        if (n < bm)
            return T.CreateSaturating(n);
        if (n == bm)
            return -T.CreateSaturating(bm - 1);
        return -T.CreateSaturating((~n + 1) & ((1u << BitCount) - 1));
    }

    public void WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            ChannelUtilities.WriteRawUInt32(span, BitOffset + shift, BitCount, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = -int.CreateSaturating(value);
            ChannelUtilities.WriteRawUInt32(span, BitOffset + shift, BitCount, (uint) n | (1u << (BitCount - 1)));
        }
    }

    public bool Equals(SNormChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is SNormChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is SNormChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(SNormChannel<T> left, SNormChannel<T> right) => left.Equals(right);

    public static bool operator !=(SNormChannel<T> left, SNormChannel<T> right) => !(left == right);
}