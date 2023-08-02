using DdsManipLib.DirectDrawSurface;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old;
using DdsManipLib.DirectDrawSurface.PixelFormats.Old.Channels;

namespace DdsManipLib.Example;

public static class Class1 {
    public static int Main(string[] args) {
        var ddsf = DdsFile.FromFile(@"Z:\test\v02_m0361b0001_n.dds");
        ddsf.ConvertTo(RgbaPixelFormat.NewRgba(16, 16, 16, 16, 0, 0, ChannelType.F16, AlphaType.Straight)).WriteToFile(@"Z:\test\rgbaf16.dds");
        ddsf.ConvertTo(RgbaPixelFormat.NewRgba(32, 32, 32, 32, 0, 0, ChannelType.F32, AlphaType.Straight)).WriteToFile(@"Z:\test\rgbaf32.dds");
        ddsf.ConvertTo(RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, ChannelType.Unorm, AlphaType.Straight)).WriteToFile(@"Z:\test\rgba8888u.dds");
        ddsf.ConvertTo(RgbaPixelFormat.NewBgra(4, 4, 4, 4, 0, 0, ChannelType.Unorm, AlphaType.Straight)).WriteToFile(@"Z:\test\rgba4444u.dds");
        ddsf.ConvertTo(LumiPixelFormat.NewL(8, 0, ChannelType.Unorm)).WriteToFile(@"Z:\test\l8.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Unorm, AlphaType.Straight, 1)).WriteToFile(@"Z:\test\bc1.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Unorm, AlphaType.Straight, 2)).WriteToFile(@"Z:\test\bc2.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Unorm, AlphaType.Straight, 3)).WriteToFile(@"Z:\test\bc3.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Unorm, AlphaType.Straight, 4)).WriteToFile(@"Z:\test\bc4u.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Snorm, AlphaType.Straight, 4)).WriteToFile(@"Z:\test\bc4s.dds");
        ddsf.ConvertTo(new BcPixelFormat(ChannelType.Unorm, AlphaType.Straight, 5)).WriteToFile(@"Z:\test\bc5.dds");
        return 0;
    }
}