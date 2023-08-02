using System;
using System.Diagnostics;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface ISIntChannel<T> : ISIntChannel, IChannel<T>
    where T : unmanaged, ISignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {

    public void CopyPixelToInt<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        targetChannel.WriteValue(target, targetShift, T2.CreateSaturating(ReadValue(source, sourceShift)));
    }
    
    public void CopyPixelToUNorm<T2>(ReadOnlySpan<byte> source, int sourceShift, IChannel<T2> targetChannel, Span<byte> target, int targetShift)
        where T2 : unmanaged, IUnsignedNumber<T2>, IBinaryInteger<T2>, IBinaryNumber<T2>, IMinMaxValue<T2> {
        var v = ReadValue(source, sourceShift);
        targetChannel.WriteValue(target, targetShift, T2.CreateSaturating(v));
    }
}
