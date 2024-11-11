using System;
using System.Runtime.InteropServices;

namespace BUT.TTOR.Core.Utils
{
    public class WindowsMonitorUtils
    {
    #if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN

        private static float _cachedDpi = -1;

        public enum DPITYPE
        {
            Effective = 0,
            Angular = 1,
            Raw = 2
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        public static extern int GetSystemMetricsForDpi(int nIndex, uint dpi);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("Shcore.dll")]
        public static extern IntPtr GetDpiForMonitor([In] IntPtr hmonitor, [In] DPITYPE dpiType, [Out] out uint dpiX, [Out] out uint dpiY);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(IntPtr hwnd, int hwndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        public static float GetRawDPIForCurrentWindow() {
            if (_cachedDpi != -1 && _cachedDpi != 0)
            {
                return _cachedDpi;
            }
            IntPtr monitorId = MonitorFromWindow(GetActiveWindow(), 0);
            uint dpiX;
            uint dpiY;
            GetDpiForMonitor(monitorId, DPITYPE.Raw, out dpiX, out dpiY);
            _cachedDpi = dpiX;
            return _cachedDpi;
        }
    #endif
    }
}


