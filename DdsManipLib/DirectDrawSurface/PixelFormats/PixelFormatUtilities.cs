#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public static class PixelFormatUtilities {
    public static int RawToSInt(uint rawValue, int sourceBits) {
        rawValue &= (1u << sourceBits) - 1u;
        var signMask = 1u << (sourceBits - 1);
        if ((rawValue & signMask) == 0)
            return (int) rawValue;
        return (int) ((uint.MaxValue << sourceBits) | rawValue);
    }

    public static uint SIntToRaw(int intValue, int sourceBits) {
        var maxValue = (1 << (sourceBits - 1)) - 1;
        var minValue = -(1 << (sourceBits - 1));
        if (intValue > maxValue)
            intValue = maxValue;
        else if (intValue < minValue)
            intValue = minValue;
        return (uint) intValue & ((1u << sourceBits) - 1u);
    }

    public static uint UIntToRaw(uint uintValue, int sourceBits) {
        var maxValue = (1u << sourceBits) - 1;
        return uintValue > maxValue ? maxValue : uintValue;
    }
}
