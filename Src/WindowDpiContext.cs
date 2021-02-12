using System;
using System.Windows;
using System.Windows.Interop;

namespace ScreenVersusWpf
{
    //public class WindowDpiContext : DpiContext
    //{
    //    private readonly Window _window;
    //    private WindowInfo _info;

    //    public WindowDpiContext(Window window)
    //    {
    //        _window = window;
    //    }

    //    protected WindowInfo GetWnd()
    //    {
    //        if (_info != null)
    //            return _info;

    //        var interop = new WindowInteropHelper(_window);
    //        if (interop.Handle != IntPtr.Zero)
    //            _info = WindowInfo.FromWindow(interop.Handle);

    //        return _info;
    //    }

    //    protected (int dpiX, int dpiY, int offsetX, int offsetY) Calc()
    //    {
    //        var info = GetWnd();
    //        if (info == null)
    //            return (96, 96, 0, 0);

    //        PresentationSource source = PresentationSource.FromVisual(_window);
    //        var dx = (int)Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M11);
    //        var dy = (int)Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M22);

    //        var virt = DisplayInfo.VirtualScreen;
    //        return (dx, dy, info.Bounds.Left - virt.Bounds.Left, info.Bounds.Top - virt.Bounds.Top);
    //    }

    //    public override int WorldOffsetX => Calc().offsetX;

    //    public override int WorldOffsetY => Calc().offsetY;

    //    public override int DpiX => Calc().dpiX;

    //    public override int DpiY => Calc().dpiY;
    //}
}
