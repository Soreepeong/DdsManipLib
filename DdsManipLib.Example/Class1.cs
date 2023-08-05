using DdsManipLib.DirectDrawSurface;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.BlockPixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.RawPixelFormats;

namespace DdsManipLib.Example;

public static class Class1 {
    public static int Main(string[] args) {
        var ddsf = DdsFile.FromFile(@"Z:\test\v02_m0361b0001_n.dds");
        _ = ddsf.PixelFormat;
        ddsf.ConvertTo(new DdspfYxUxVxPixelFormat<byte>(24, 0, 8, 8, 8, 16, 8)).ConvertTo(new R8G8B8A8UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\yuv8.dds");
        // ddsf.ConvertTo(new DdspfYxUNormUxVxSNormAxUNormPixelFormat<byte>(32, 0, 8, 8, 8, 16, 8, 24, 8)).ConvertTo(new R8G8B8A8UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\yuva8.dds");
        // ddsf.ConvertTo(new DdspfLxUNormPixelFormat<byte>(8, 0, 8)).WriteToFile(@"Z:\test\l8.dds");
        // ddsf.ConvertTo(new DdspfLxAxUNormPixelFormat<byte>(16, 0, 8, 8, 8)).WriteToFile(@"Z:\test\la8.dds");
        // ddsf.ConvertTo(new R1UNormPixelFormat()).ConvertTo(new B8G8R8X8UNormPixelFormat()).WriteToFile(@"Z:\test\r1.dds");
        // ddsf.ConvertTo(new B5G5R5A1UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bgra5551.dds");
        // ddsf.ConvertTo(new B5G6R5UNormPixelFormat()).WriteToFile(@"Z:\test\bgr565.dds");
        // ddsf.ConvertTo(new B4G4R4A4UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bgra4u.dds");
        // ddsf.ConvertTo(new B8G8R8A8UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bgra8u.dds");
        // ddsf.ConvertTo(new B8G8R8X8UNormPixelFormat()).WriteToFile(@"Z:\test\bgrx4u.dds");
        // ddsf.ConvertTo(new R8G8B8A8UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\rgba8u.dds");
        // ddsf.ConvertTo(new R8G8B8A8SNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\rgba8s.dds");
        // ddsf.ConvertTo(new R8G8UNormPixelFormat()).WriteToFile(@"Z:\test\rg8u.dds");
        // ddsf.ConvertTo(new R8G8SNormPixelFormat()).WriteToFile(@"Z:\test\rg8s.dds");
        // ddsf.ConvertTo(new R16G16FloatPixelFormat()).WriteToFile(@"Z:\test\rg16f.dds");
        // ddsf.ConvertTo(new R16G16UNormPixelFormat()).WriteToFile(@"Z:\test\rg16u.dds");
        // ddsf.ConvertTo(new R16G16SNormPixelFormat()).WriteToFile(@"Z:\test\rg16s.dds");
        // ddsf.ConvertTo(new R32G32FloatPixelFormat()).WriteToFile(@"Z:\test\rg32f.dds");
        // ddsf.ConvertTo(new R32G32UIntPixelFormat()).WriteToFile(@"Z:\test\rg32u.dds");
        // ddsf.ConvertTo(new R32G32SIntPixelFormat()).WriteToFile(@"Z:\test\rg32s.dds");
        // ddsf.ConvertTo(new R16G16B16A16UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\rgba16.dds");
        // ddsf.ConvertTo(new R16G16B16A16FloatPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\rgba16F.dds");
        // ddsf.ConvertTo(new R32G32B32A32FloatPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\rgba32F.dds");
        // ddsf.ConvertTo(new Bc1UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bc1.dds");
        // ddsf.ConvertTo(new Bc2UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bc2.dds");
        // ddsf.ConvertTo(new Bc3UNormPixelFormat(AlphaType.Straight)).WriteToFile(@"Z:\test\bc3.dds");
        // ddsf.ConvertTo(new Bc4UNormPixelFormat()).WriteToFile(@"Z:\test\bc4u.dds");
        // ddsf.ConvertTo(new Bc4SNormPixelFormat()).WriteToFile(@"Z:\test\bc4s.dds");
        // ddsf.ConvertTo(new Bc5UNormPixelFormat()).WriteToFile(@"Z:\test\bc5u.dds");
        // ddsf.ConvertTo(new Bc5SNormPixelFormat()).WriteToFile(@"Z:\test\bc5s.dds");
        return 0;
    }
}