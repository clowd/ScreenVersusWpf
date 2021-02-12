using ScreenVersusWpf.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenVersusWpf
{
    public class CursorInfo
    {
        public static ScreenPoint Position => WinAPI.GetMouseScreenPosition();
    }
}
