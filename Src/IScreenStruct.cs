#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using System;

namespace ScreenVersusWpf
{
    public interface IScreenStruct<TSelf, TWorld> : IEquatable<TSelf> where TSelf : IScreenStruct<TSelf, TWorld>
    {
        //TWorld ToVisual(System.Windows.Media.Visual visual);
        //TWorld ToDisplay(DisplayInfo display);
        //TWorld ToDisplay(IntPtr hMonitor);
        //TWorld ToPrimaryDisplay();
    }
}
