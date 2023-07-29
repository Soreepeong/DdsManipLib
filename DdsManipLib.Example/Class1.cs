using System.Diagnostics;
using DdsManipLib.DirectDrawSurface;
using DdsManipLib.DirectDrawSurface.PixelFormats;
using DdsManipLib.DirectDrawSurface.PixelFormats.Channels;

namespace DdsManipLib.Example;

public static class Class1 {
    public static int Main(string[] args) {
        var ddsf = DdsFile.FromFile(@"Z:\test\v01_m0361b0001_n.bc7_pdn.dds");
        var ddsf2 = new DdsFile();
        if (!ddsf2.TryInitializeFrom(RgbaPixelFormat.NewBgra(8, 8, 8, 8, 0, 0, ChannelType.Unorm, AlphaType.Straight), ddsf))
            Debug.Assert(false);
        ddsf2.WriteToFile(@"Z:\test\test.dds");
        return 0;
    }
}