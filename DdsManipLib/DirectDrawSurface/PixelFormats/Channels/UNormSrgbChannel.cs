using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct UNormSrgbChannel<T> : IUIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public UNormSrgbChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }
    public float ToNormalizedValue(T value) => float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, 0f, 1f) * float.CreateSaturating(T.MaxValue));

    public bool Equals(UNormSrgbChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is UNormSrgbChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is UNormSrgbChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(UNormSrgbChannel<T> left, UNormSrgbChannel<T> right) => left.Equals(right);

    public static bool operator !=(UNormSrgbChannel<T> left, UNormSrgbChannel<T> right) => !(left == right);
}