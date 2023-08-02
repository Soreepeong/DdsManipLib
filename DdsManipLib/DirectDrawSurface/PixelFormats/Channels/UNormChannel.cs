using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct UNormChannel<T> : IUIntChannel<T>, INormalizedChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public UNormChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }
    public float ToNormalizedValue(T value) => float.CreateSaturating(value) / float.CreateSaturating(T.MaxValue);
    public T FromNormalizedValue(float value) => T.CreateSaturating(float.Clamp(value, 0f, 1f) * float.CreateSaturating(T.MaxValue));

    public T ReadValue(ReadOnlySpan<byte> span, int shift) =>
        T.CreateSaturating(ChannelUtilities.ReadRawUInt32(span, BitOffset + shift, BitCount));

    public void WriteValue(Span<byte> span, int shift, T value) =>
        ChannelUtilities.WriteRawUInt32(span, BitOffset + shift, BitCount, uint.CreateSaturating(value));

    public bool Equals(UNormChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is UNormChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is UNormChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(UNormChannel<T> left, UNormChannel<T> right) => left.Equals(right);

    public static bool operator !=(UNormChannel<T> left, UNormChannel<T> right) => !(left == right);
}

public static class UNormChannel {
    public static IChannel? FromMask(uint mask) {
        if (mask == 0)
            return null;
        
        var shift = BitOperations.TrailingZeroCount(mask);
        var bits = BitOperations.PopCount(mask);
        var mask2 = ((1u << bits) - 1u) << shift;
        if (mask != mask2)
            throw new NotSupportedException("Mask with a hole in the middle is not supported.");
        return bits switch {
            <= 8 => new UNormChannel<byte>(shift, bits),
            <= 16 => new UNormChannel<ushort>(shift, bits),
            <= 32 => new UNormChannel<uint>(shift, bits),
            _ => throw new InvalidOperationException(),
        };
    }
}
