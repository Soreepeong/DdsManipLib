using System;
using System.Numerics;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

public interface IUIntChannel<T> : IUIntChannel, IChannel<T>
    where T : unmanaged, IUnsignedNumber<T>, IBinaryInteger<T>, IBinaryNumber<T>, IMinMaxValue<T> {
}