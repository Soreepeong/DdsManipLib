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

    public bool Equals(SNormChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is SNormChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is SNormChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(SNormChannel<T> left, SNormChannel<T> right) => left.Equals(right);

    public static bool operator !=(SNormChannel<T> left, SNormChannel<T> right) => !(left == right);
}