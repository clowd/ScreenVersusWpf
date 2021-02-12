using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace ScreenVersusWpf
{
    public class VisualDpiContext : DpiContext
    {
        private readonly Visual visual;

        public VisualDpiContext(Visual visual)
        {
            this.visual = visual;
        }

        public override int WorldOffsetX => 0;

        public override int WorldOffsetY => 0;

        public override int DpiX
        {
            get
            {
                var source = PresentationSource.FromVisual(visual);
                CompositionTarget target;

                if (source == null)
                {
                    target = HwndSource.FromHwnd(new WindowInteropHelper(Window.GetWindow(visual)).EnsureHandle()).CompositionTarget;
                }
                else
                {
                    target = source.CompositionTarget;
                }

                return (int)Math.Round(96.0d * target.TransformToDevice.M11);
            }
        }

        public override int DpiY => DpiX; // (int)Math.Round(96.0d * PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice.M22);
    }
}
