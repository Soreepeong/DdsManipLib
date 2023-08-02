using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public readonly struct SIntChannel<T> : ISIntChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IMinMaxValue<T> {
    public SIntChannel(int bitOffset, int bitCount) {
        BitOffset = bitOffset;
        BitCount = bitCount;
    }

    public int BitOffset { get; }
    public int BitCount { get; }

    public T ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ChannelUtilities.ReadRawUInt32(span, BitOffset + shift, BitCount);

        var signMask = 1u << (BitCount - 1);
        if (0 != (n & signMask))
            n |= uint.MaxValue << (BitCount - 1);
        return T.CreateSaturating(n);
    }

    public void WriteValue(Span<byte> span, int shift, T value) {
        var i = int.CreateSaturating(value);
        var u = (uint) i;
        var maxValue = (1u << (BitCount - 1)) - 1;
        var minValue = uint.MaxValue << (BitCount - 1);
        if (maxValue < u && (u & 0x80000000u) == 0)
            u = (1u << (BitCount - 1)) - 1;
        else if (u < minValue && (u & 0x80000000u) != 0)
            u = 1u << (BitCount - 1);
        else
            u = (uint) i & ((1u << BitCount) - 1);
        ChannelUtilities.WriteRawUInt32(span, BitOffset + shift, BitCount, u);
    }

    public void CopyPixelToSInt<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, ISignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        var v = ReadValue(source, sourceShift);
        targetChannel.WriteValue(target, targetShift, T2.CreateTruncating(v));
    }

    public void CopyPixelToUInt<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IUnsignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        var v = ReadValue(source, sourceShift);
        targetChannel.WriteValue(target, targetShift, T2.CreateTruncating(v));
    }

    public void CopyPixelToSNorm<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, ISignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        var v = ReadValue(source, sourceShift);
        if (Unsafe.SizeOf<T>())
            targetChannel.WriteValue(target, targetShift, T2.CreateTruncating(v));
    }

    public void CopyPixelToUNorm<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IUnsignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        throw new NotImplementedException();
    }

    public void CopyPixelToFloat(ReadOnlySpan<byte> source, int sourceShift, IFloatChannel targetChannel, Span<byte> target, int targetShift) {
        throw new NotImplementedException();
    }

    public IChannel.CopyPixelDelegate? GetCopyPixelDelegate(IChannel? target) {
        throw new NotImplementedException();
    }

    public bool Equals(SIntChannel<T> other) => BitOffset == other.BitOffset && BitCount == other.BitCount;

    public bool Equals(IChannel? other) => other is SIntChannel<T> r && Equals(r);

    public override bool Equals(object? obj) => obj is SIntChannel<T> other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(BitOffset, BitCount);

    public static bool operator ==(SIntChannel<T> left, SIntChannel<T> right) => left.Equals(right);

    public static bool operator !=(SIntChannel<T> left, SIntChannel<T> right) => !(left == right);
}
