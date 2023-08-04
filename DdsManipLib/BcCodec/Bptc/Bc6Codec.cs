using System;
using System.Diagnostics;
using DdsManipLib.Utilities;

namespace DdsManipLib.BcCodec.Bptc;

internal static class Bc6Codec {
    public static void Decompress(bool signed, ReadOnlySpan<byte> block, Span<float> pixelBuffer) {
        Debug.Assert(pixelBuffer.Length == 4 * 4 * 3);
        var parsed = new Bc6ParsedBlock(stackalloc Vector3<int>[Bc6ParsedBlock.MaxNumEndpoints]);
        if (!parsed.ReadBlock(block, signed)) {
            pixelBuffer[..64].Clear();
            return;
        }

        var colorBitCount = parsed.ColorIndexBitCount;
        for (var i = 0; i < 16; i++) {
            var subsetIndex = parsed.GetPartitionIndex(i);

            var endPointStart = parsed.Endpoints[2 * subsetIndex];
            var endPointEnd = parsed.Endpoints[2 * subsetIndex + 1];

            var colorIndex = parsed.GetColorIndex(colorBitCount, i);

            var interpolated = BptcConstants.Interpolate(endPointStart, endPointEnd, colorIndex, colorBitCount);
            pixelBuffer[i * 3 + 0] = Bc6ParsedBlock.FinishUnquantize(signed, interpolated.X);
            pixelBuffer[i * 3 + 1] = Bc6ParsedBlock.FinishUnquantize(signed, interpolated.Y);
            pixelBuffer[i * 3 + 2] = Bc6ParsedBlock.FinishUnquantize(signed, interpolated.Z);
        }
    }
}