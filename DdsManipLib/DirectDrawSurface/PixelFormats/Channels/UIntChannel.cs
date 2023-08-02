using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct UIntChannel<T> : IUIntChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public UIntChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public T ReadValue(ReadOnlySpan<byte> span, int shift) =>
        T.CreateSaturating(ChannelUtilities.ReadRawUInt32(span, BitOffset + shift, BitCount));

    public void WriteValue(Span<byte> span, int shift, T value) =>
        ChannelUtilities.WriteRawUInt32(span, BitOffset + shift, BitCount, uint.CreateSaturating(value));

    public bool Equals(UIntChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is UIntChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is UIntChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(UIntChannel<T> left, UIntChannel<T> right) => left.Equals(right);

    public static bool operator !=(UIntChannel<T> left, UIntChannel<T> right) => !(left == right);
}