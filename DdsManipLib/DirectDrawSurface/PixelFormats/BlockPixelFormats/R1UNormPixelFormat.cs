using System;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;

public class R1UNormPixelFormat : BlockPixelFormat {
    public override DdsPixelFormat DdsPixelFormat => DdsPixelFormat.FromRgba(1, 1, 0, 0);
    public override DxgiFormat DxgiFormat => DxgiFormat.R1UNorm;
    public override int BitsPerPixel => 1;
    public override int CalculatePitch(int width) => (width + 7) / 8;
    public override int CalculateLinearSize(int width, int height) => (width + 7) / 8 * height;
    public override int BlockBytes => 1;
    public override int BlockWidth => 8;
    public override int BlockHeight => 1;
    public override IRawPixelFormat SuggestedRawPixelFormat => new R8UNormPixelFormat();

    public override bool SupportsRawPixelFormat(IRawPixelFormat rawpf) => rawpf is IRawRPixelFormat;

    public override void Decompress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) {
        if (rawPixelFormat is not IRawRPixelFormat rpf)
            throw new ArgumentException(null, nameof(rawPixelFormat));

        sourceSpan = sourceSpan[..CalculateLinearSize(width, height)];
        targetSpan = targetSpan[..rpf.CalculateLinearSize(width, height)];
        var sourcePitch = CalculatePitch(width);
        var targetPitch = rpf.CalculatePitch(width);
        var targetBpp = rpf.BytesPerPixel;
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < width; x++)
                rpf.SetRed(targetSpan[(targetBpp * x)..], ((sourceSpan[x / 8] >> (x % 8)) & 1) == 0 ? 0f : 1f);
        }
    }

    public override void Compress(IRawPixelFormat rawPixelFormat, ReadOnlySpan<byte> sourceSpan, int width, int height, Span<byte> targetSpan) {
        if (rawPixelFormat is not IRawRPixelFormat rpf)
            throw new ArgumentException(null, nameof(rawPixelFormat));

        sourceSpan = sourceSpan[..rpf.CalculateLinearSize(width, height)];
        targetSpan = targetSpan[..CalculateLinearSize(width, height)];
        var targetPitch = CalculatePitch(width);
        var sourcePitch = rpf.CalculatePitch(width);
        var sourceBpp = rpf.BytesPerPixel;
        for (; !sourceSpan.IsEmpty; sourceSpan = sourceSpan[sourcePitch..], targetSpan = targetSpan[targetPitch..]) {
            for (var x = 0; x < targetPitch; x++) {
                targetSpan[x] = 0;
                for (var sx = 0; sx < 8 && x * 8 + sx < width; sx++) {
                    if (rpf.GetRed(sourceSpan[(sourceBpp * (x * 8 + sx))..]) > 0)
                        targetSpan[x] |= (byte) (1 << sx);
                }
            }
        }
    }

    public R1UNormPixelFormat() : base(AlphaType.None) { }
}
