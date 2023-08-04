using System;
using System.Diagnostics;
using DdsManipLib.Utilities;

namespace DdsManipLib.BcCodec.Bptc;

internal static class Bc7Codec {
    public static void Decompress(ReadOnlySpan<byte> block, Span<byte> pixelBuffer) {
        Debug.Assert(block.Length >= 16);
        Debug.Assert(pixelBuffer.Length >= 64);

        var parsed = new Bc7ParsedBlock(stackalloc Vector4<byte>[Bc7ParsedBlock.MaxNumEndpoints]);
        if (!parsed.ReadBlock(block)) {
            pixelBuffer[..64].Clear();
            return;
        }

        var colorBitCount = parsed.ColorIndexBitCount;
        var alphaBitCount = parsed.AlphaIndexBitCount;
        var xyzw = new Vector4<byte>();
        for (var i = 0; i < 16; i++) {
            // decode partition data from explicit partition bits
            var subsetIndex = parsed.GetPartitionIndex(i);

            // endpoints are now complete.
            var endpoint0 = parsed.Endpoints[2 * subsetIndex + 0];
            var endpoint1 = parsed.Endpoints[2 * subsetIndex + 1];

            // Determine the palette index for this pixel
            var colorIndex = parsed.GetColorIndex(colorBitCount, i);
            var alphaIndex = parsed.GetAlphaIndex(alphaBitCount, i);

            // determine output
            xyzw.X = BptcConstants.Interpolate(endpoint0.X, endpoint1.X, colorIndex, colorBitCount);
            xyzw.Y = BptcConstants.Interpolate(endpoint0.Y, endpoint1.Y, colorIndex, colorBitCount);
            xyzw.Z = BptcConstants.Interpolate(endpoint0.Z, endpoint1.Z, colorIndex, colorBitCount);
            xyzw.W = BptcConstants.Interpolate(endpoint0.W, endpoint1.W, alphaIndex, alphaBitCount);

            // rotate accordingly
            xyzw = parsed.Rotation switch {
                Bc7RotationMode.NoChange => xyzw,
                Bc7RotationMode.SwapAlphaRed => new(xyzw.W, xyzw.Y, xyzw.Z, xyzw.X),
                Bc7RotationMode.SwapAlphaGreen => new(xyzw.X, xyzw.W, xyzw.Z, xyzw.Y),
                Bc7RotationMode.SwapAlphaBlue => new(xyzw.X, xyzw.Y, xyzw.W, xyzw.Z),
                _ => xyzw,
            };
            xyzw.CopyTo(pixelBuffer);
            pixelBuffer = pixelBuffer[4..];
        }
    }
}
