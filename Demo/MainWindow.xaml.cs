using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using ScreenVersusWpf;

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
            SystemEvents.DisplaySettingsChanged += delegate { Refresh(); };
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            _initialized = true;
            Refresh();
            _timer.Interval = TimeSpan.FromSeconds(1.0 / 60);
            _timer.Tick += timer_Tick;
            _timer.Start();
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);
            Refresh();
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

            ctVirtualScreenSize.Content = $"{ScreenInfo.VirtualScreen.Bounds.Width}x{ScreenInfo.VirtualScreen.Bounds.Height}";
            ctVirtualScreenDpi.Content = dpi.DpiX;
            ctVirtualScreenZoom.Content = $"{dpi.DpiScaleX:0%}";
            ctWpfRectToScreen.Content = $"{wpfRectToScreen.Width}x{wpfRectToScreen.Height}";
            ctScreenRectToWpf.Content = $"{screenRectToWpf.Width}x{screenRectToWpf.Height}";
            ctBoxScreenSize.Width = screenRectToWpf.Width;
            ctBoxScreenSize.Height = screenRectToWpf.Height;
            ctPhysicalScreens.ItemsSource = ScreenInfo.AllScreens.Select(s => new ScreenVM(s)).ToList();

            var wi = WindowInfo.FromWindow(this);
            var wscr = wi.Bounds;
            var wvirt = dpi.ToWorldRect(wscr);

            ctWindowNative.Content = $"Native({Left:0.##},{Top:0.##},{Width:0.##},{Height:0.##})";
            ctWindowVirtual.Content = $"Virtual({wvirt.Left},{wvirt.Top},{wvirt.Width},{wvirt.Height})";
            ctWindowScreen.Content = $"Screen({wscr.Left},{wscr.Top},{wscr.Width},{wscr.Height})";

            // screens illustration

            var virtRect = ScreenInfo.VirtualScreen.Bounds.ToVisual(this);
            scrCanvas.Children.Clear();
            scrCanvas.Width = virtRect.Width;
            scrCanvas.Height = virtRect.Height;

            if (scrZoom.ActualWidth == 0)
                return;

            var zoom = virtRect.Width / scrZoom.ActualWidth;
            var thickness = dpi.Round(1) * zoom;

            foreach (var scr in ScreenInfo.AllScreens)
            {
                var b = new Border();
                b.BorderThickness = new Thickness(thickness);
                b.BorderBrush = Brushes.Turquoise;
                var wr = scr.Bounds.ToVisual(this);
                Canvas.SetLeft(b, wr.Left);
                Canvas.SetTop(b, wr.Top);
                b.Width = wr.Width;
                b.Height = wr.Height;
                scrCanvas.Children.Add(b);

                var t = new TextBlock();
                t.Text = scr.Bounds.ToString() + Environment.NewLine + $"{scr.DpiContext.DpiX} DPI";
                t.FontSize = 11 * zoom;
                Canvas.SetLeft(t, wr.Left);
                Canvas.SetTop(t, wr.Top);
                scrCanvas.Children.Add(t);
            }

            {
                var b = new Border();
                b.BorderThickness = new Thickness(thickness);
                b.BorderBrush = Brushes.Red;
                var wr = wvirt;
                Canvas.SetLeft(b, wr.Left);
                Canvas.SetTop(b, wr.Top);
                b.Width = wr.Width;
                b.Height = wr.Height;
                scrCanvas.Children.Add(b);

                var t = new TextBlock();
                t.Text = wvirt.ToString() + Environment.NewLine + $"{ScreenInfo.FromWindow(this).DpiContext.DpiX} DPI";
                t.FontSize = 11 * zoom;
                Canvas.SetLeft(t, wr.Left);
                Canvas.SetTop(t, wr.Top);
                scrCanvas.Children.Add(t);
            }


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

        //private class ViewModel : INotifyPropertyChanged
        //{
        //    public event PropertyChangedEventHandler PropertyChanged;

        //    public double CanvasWidth { get; set; }
        //    public double CanvasHeight { get; set; }

        //    public 

        //}

        private class ScreenVM : INotifyPropertyChanged
        {
            public ScreenInfo Scr { get; private set; }
            public ScreenVM(ScreenInfo scr) { Scr = scr; }
            public event PropertyChangedEventHandler PropertyChanged;
            public override string ToString() => Scr.ToString();
            public Visibility ContainsMouseVisibility { get; set; } = Visibility.Hidden;
        }
    }
}
