using System;
using System.Diagnostics;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface ISIntChannel<T> : ISIntChannel, IChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
    T IChannel<T>.ReadValue(ReadOnlySpan<byte> span, int shift) {
        var n = ReadRawUInt32(span, shift);
        return (n & (1 << (BitCount - 1))) == 0
            ? T.CreateSaturating(n)
            : -T.CreateSaturating((~n + 1) & ((1u << BitCount) - 1));
    }

    void IChannel<T>.WriteValue(Span<byte> span, int shift, T value) {
        if (T.Sign(value) >= 0) {
            WriteRawUInt32(span, shift, Math.Min(uint.CreateSaturating(value), 1u << (BitCount - 1)) - 1u);
        } else {
            var n = -int.CreateSaturating(value);
            Debug.Assert(n > 0);
            WriteRawUInt32(span, shift, (uint) n | (1u << (BitCount - 1)));
        }
    }
}