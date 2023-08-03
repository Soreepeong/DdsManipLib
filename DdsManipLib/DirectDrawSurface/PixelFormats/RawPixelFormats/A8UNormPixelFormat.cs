﻿using System;

#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

public sealed class A8UNormPixelFormat : RawPixelFormat, IRawAPixelFormat<byte> {
    public const int OffsetA = 0;

    public override DxgiFormat DxgiFormat => DxgiFormat.R8UNorm;

    public override DdsPixelFormat DdsPixelFormat => AlphaType != AlphaType.Straight
        ? DdsPixelFormat.FromFourCc(DdsFourCc.Dx10)
        : DdsPixelFormat.FromAlpha(8, 0xFF);

    public override int BitsPerPixel => 8;
    public override int BytesPerPixel => 1;
    public float GetAlpha(ReadOnlySpan<byte> pixel) => pixel[OffsetA] / 255f;
    public byte GetAlphaTyped(ReadOnlySpan<byte> pixel) => pixel[OffsetA];
    public void SetAlpha(Span<byte> pixel, float value) => SetAlpha(pixel, byte.CreateTruncating(value * 256f));
    public void SetAlpha(Span<byte> pixel, byte value) => pixel[OffsetA] = value;
    public A8UNormPixelFormat(AlphaType alphaType) : base(alphaType) { }
}
