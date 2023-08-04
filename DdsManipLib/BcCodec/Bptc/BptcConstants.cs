using DdsManipLib.Utilities;

namespace DdsManipLib.BcCodec.Bptc;

internal static class BptcConstants {
    public static readonly byte[] InterpolationWeights2 = {0, 21, 43, 64};
    public static readonly byte[] InterpolationWeights3 = {0, 9, 18, 27, 37, 46, 55, 64};
    public static readonly byte[] InterpolationWeights4 = {0, 4, 9, 13, 17, 21, 26, 30, 34, 38, 43, 47, 51, 55, 60, 64};

    public static byte Interpolate(byte e0, byte e1, int index, int indexPrecision) => indexPrecision switch {
        2 => (byte) (((64 - InterpolationWeights2[index]) * e0 + InterpolationWeights2[index] * e1 + 32) >> 6),
        3 => (byte) (((64 - InterpolationWeights3[index]) * e0 + InterpolationWeights3[index] * e1 + 32) >> 6),
        4 => (byte) (((64 - InterpolationWeights4[index]) * e0 + InterpolationWeights4[index] * e1 + 32) >> 6),
        _ => e0,
    };

    public static int Interpolate(int e0, int e1, int index, int indexPrecision) => indexPrecision switch {
        2 => (byte) (((64 - InterpolationWeights2[index]) * e0 + InterpolationWeights2[index] * e1 + 32) >> 6),
        3 => (byte) (((64 - InterpolationWeights3[index]) * e0 + InterpolationWeights3[index] * e1 + 32) >> 6),
        4 => (byte) (((64 - InterpolationWeights4[index]) * e0 + InterpolationWeights4[index] * e1 + 32) >> 6),
        _ => e0,
    };

    public static Vector3<int> Interpolate(in Vector3<int> endPointStart, in Vector3<int> endPointEnd, int colorIndex, int colorBitCount) => new(
        Interpolate(endPointStart.X, endPointEnd.X, colorIndex, colorBitCount),
        Interpolate(endPointStart.Y, endPointEnd.Y, colorIndex, colorBitCount),
        Interpolate(endPointStart.Z, endPointEnd.Z, colorIndex, colorBitCount));
}