using System.Runtime.InteropServices;

namespace BlazorStandaloneApplicationExample
{
    public static class WindowsInterops
    {
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_TOOLWINDOW = 0x00000080;
        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        public static void HideConsoleWindow()
        {
            var consoleWindow = GetConsoleWindow();
            ShowWindow(consoleWindow, WindowsInterops.SW_HIDE);
            SetWindowLong(consoleWindow, GWL_EXSTYLE, WS_EX_TOOLWINDOW);
        }
    }
}
