using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct SIntChannel<T> : ISIntChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public SIntChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public bool Equals(SIntChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is SIntChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is SIntChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(SIntChannel<T> left, SIntChannel<T> right) => left.Equals(right);

    public static bool operator !=(SIntChannel<T> left, SIntChannel<T> right) => !(left == right);
}