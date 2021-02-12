using System;
using System.Windows;
using System.Windows.Media;

namespace ScreenVersusWpf
{
    public static class Extensions
    {
        //public static ScreenRect FromVisualToScreen(this Rect rect, Visual visual) => DpiContext.FromVisual(visual).ToScreenRect(rect);

        public static (int dpiX, int dpiY) GetCurrentScalingFactor(this Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            var dx = (int)Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M11);
            var dy = (int)Math.Round(96.0d * source.CompositionTarget.TransformToDevice.M22);
            return (dx, dy);
        }
    }
}
