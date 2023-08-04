using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using DdsManipLib.BcCodec.Bptc;

namespace DdsManipLib.BcCodec.SquishInternal;

internal unsafe struct BlockDecompresserF32 {
    private readonly SquishOptions2 _options;
#pragma warning disable CS0649
    private fixed float _rgb[48];
#pragma warning restore CS0649

    public BlockDecompresserF32(SquishOptions2 options) {
        _options = options;
    }

    public Vector3 this[int y, int x] => new(
        _rgb[y * 12 + x * 3 + 0],
        _rgb[y * 12 + x * 3 + 1],
        _rgb[y * 12 + x * 3 + 2]);

    public void DecompressFrom(ReadOnlySpan<byte> block) {
        fixed (float* pRgb = _rgb) {
            Span<float> rgb = new(pRgb, 48);
            switch (_options.Method) {
                case SquishMethod.Bc6U:
                    Debug.Assert(block.Length >= 16);
                    Bc6Codec.Decompress(false, block, rgb);
                    break;
                case SquishMethod.Bc6S:
                    Debug.Assert(block.Length >= 16);
                    Bc6Codec.Decompress(true, block, rgb);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }

    public void RemapChannelsInto(Span<byte> pixels) {
        fixed (float* pRgba = _rgb) {
            Span<float> rgb = new(pRgba, 48);
            Debug.Assert(pixels.Length == 12 * _options.NumBytesPerPixel);

            var p = _options.NumBytesPerPixel;
            var (r, g, b, _) = _options.ChannelOffsets;
            if (r < p)
                for (int i = 0, j = r; i < 48; i += 3, j += p)
                    BinaryPrimitives.WriteSingleLittleEndian(pixels[j..], rgb[i]);
            if (g < p)
                for (int i = 1, j = g; i < 48; i += 3, j += p)
                    BinaryPrimitives.WriteSingleLittleEndian(pixels[j..], rgb[i]);
            if (b < p)
                for (int i = 2, j = b; i < 48; i += 3, j += p)
                    BinaryPrimitives.WriteSingleLittleEndian(pixels[j..], rgb[i]);
        }
    }

    public void RemapChannelsInto(Span<byte> pixels, int x0, int y0, int stride, int width, int height) {
        fixed (float* pRgb = _rgb) {
            Span<float> rgb = new(pRgb, 48);
            var p = _options.NumBytesPerPixel;

            var (r, g, b, _) = _options.ChannelOffsets;

            var availHorzChannels = Math.Min(4, width - x0) * 3;
            var availVertPixels = Math.Min(4, height - y0);

            for (int by = 0, y = y0; by < availVertPixels; by++, y++) {
                var rgbaRow = rgb[(by * 16)..];
                var pixelsRow = pixels[(stride * y + p * x0)..];
                if (r < p)
                    for (int i = 0, j = r; i < availHorzChannels; i += 3, j += p)
                        BinaryPrimitives.WriteSingleLittleEndian(pixelsRow[j..], rgbaRow[i]);
                if (g < p)
                    for (int i = 1, j = g; i < availHorzChannels; i += 3, j += p)
                        BinaryPrimitives.WriteSingleLittleEndian(pixelsRow[j..], rgbaRow[i]);
                if (b < p)
                    for (int i = 2, j = b; i < availHorzChannels; i += 3, j += p)
                        BinaryPrimitives.WriteSingleLittleEndian(pixelsRow[j..], rgbaRow[i]);
            }
        }
    }
}
