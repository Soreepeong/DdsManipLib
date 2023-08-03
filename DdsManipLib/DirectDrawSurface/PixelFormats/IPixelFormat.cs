﻿#pragma warning disable CS1591

namespace DdsManipLib.DirectDrawSurface.PixelFormats;

public interface IPixelFormat {
    public DxgiFormat DxgiFormat { get; }
    public DdsPixelFormat DdsPixelFormat { get; }
    public AlphaType AlphaType { get; }
    public int BitsPerPixel { get; }
    public int CalculatePitch(int width);
    public int CalculateLinearSize(int width, int height);
    public bool IsDdsPixelFormat(in DdsPixelFormat ddspf) => ddspf == DdsPixelFormat;
}
