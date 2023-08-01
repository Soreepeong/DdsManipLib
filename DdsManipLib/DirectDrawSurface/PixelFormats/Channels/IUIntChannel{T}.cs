using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IUIntChannel<T> : IUIntChannel, IChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
    T IChannel<T>.ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ReadRawUInt32(span, shift);
        var bitmask = (1u << BitCount) - 1;
        n &= bitmask;

        return (n & (1 << (BitCount - 1))) == 0
            ? T.CreateSaturating(n)
            : -T.CreateSaturating((~n + 1) & bitmask);
    }

    void IChannel<T>.WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            WriteRawUInt32(span, shift, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = Math.Min(uint.CreateSaturating(~value + T.One), 1u << (BitCount - 1));
            WriteRawUInt32(span, shift, (~n + 1) & ((1u << BitCount) - 1u));
        }
    }
}