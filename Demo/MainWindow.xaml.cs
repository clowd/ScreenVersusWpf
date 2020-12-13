using System;
using System.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;
using ScreenVersusWpf;
using WpfCrutches;

namespace Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DispatcherTimer _timer = new DispatcherTimer();
        private ScreenPoint _prevMousePos;
        private bool _initialized;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _initialized = true;
            ScreenTools.ScreenSettingsChanged += delegate { Refresh(); };
            Refresh();
            _timer.Interval = TimeSpan.FromSeconds(1.0 / 60);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            Refresh();
        }

        protected override void OnLocationChanged(EventArgs e)
        {
            base.OnLocationChanged(e);
            Refresh();
        }

        private void Refresh()
        {
            if (!_initialized)
                return;

            var dpi = DpiContext.FromVisual(this);
            var wpfRectToScreen = dpi.ToScreenRect(0, 0, 200, 200);
            var screenRectToWpf = dpi.ToWorldRect(0, 0, 200, 200);

            ctVirtualScreenSize.Content = $"{ScreenTools.VirtualScreen.Bounds.Width}x{ScreenTools.VirtualScreen.Bounds.Height}";
            ctVirtualScreenDpi.Content = dpi.DpiX;
            ctVirtualScreenZoom.Content = $"{dpi.DpiScaleX:0%}";
            ctWpfRectToScreen.Content = $"{wpfRectToScreen.Width}x{wpfRectToScreen.Height}";
            ctScreenRectToWpf.Content = $"{screenRectToWpf.Width}x{screenRectToWpf.Height}";
            ctBoxScreenSize.Width = screenRectToWpf.Width;
            ctBoxScreenSize.Height = screenRectToWpf.Height;
            ctPhysicalScreens.ItemsSource = ScreenTools.Screens.Select(s => new ScreenVM(s)).ToList();


            WinAPI.GetWindowRect(new WindowInteropHelper(this).Handle, out var nrect);
            var wscr = new ScreenRect(nrect.left, nrect.top, nrect.right - nrect.left, nrect.bottom - nrect.top);
            var wvirt = dpi.ToWorldRect(wscr);

            ctWindowNative.Content = $"Native({Left:0.##},{Top:0.##},{Width:0.##},{Height:0.##})";
            ctWindowVirtual.Content = $"Virtual({wvirt.Left},{wvirt.Top},{wvirt.Width},{wvirt.Height})";
            ctWindowScreen.Content = $"Screen({wscr.Left},{wscr.Top},{wscr.Width},{wscr.Height})";

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            //var mousePosScreen = ScreenTools.GetMousePosition();
            //if (mousePosScreen == _prevMousePos)
            //    return;
            //_prevMousePos = mousePosScreen;
            //var mousePosWpf = mousePosScreen.ToWpfPoint();
            //ctMouseScreenPoint.Content = $"ScreenPoint({mousePosScreen.X}, {mousePosScreen.Y})";
            //ctMouseWpfPoint.Content = $"WpfPoint({mousePosWpf.X}, {mousePosWpf.Y})";
            //var scrContaining = ScreenTools.GetScreenContaining(mousePosScreen);
            //foreach (ScreenVM vm in ctPhysicalScreens.ItemsSource)
            //    vm.ContainsMouseVisibility.Value = vm.Scr == scrContaining ? Visibility.Visible : Visibility.Collapsed;
        }

        private class ScreenVM
        {
            public ScreenInfo Scr { get; private set; }
            public ScreenVM(ScreenInfo scr) { Scr = scr; }
            public override string ToString() => Scr.ToString();
            public ObservableValue<Visibility> ContainsMouseVisibility { get; set; } = new ObservableValue<Visibility>();
        }
    }
}
