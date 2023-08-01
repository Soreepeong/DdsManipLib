using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

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

    public bool Equals(TypelessChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is TypelessChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is TypelessChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(TypelessChannel<T> left, TypelessChannel<T> right) => left.Equals(right);

    public static bool operator !=(TypelessChannel<T> left, TypelessChannel<T> right) => !(left == right);
}