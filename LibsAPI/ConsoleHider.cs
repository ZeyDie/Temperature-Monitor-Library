using System.Runtime.InteropServices;

namespace TemperatureLibrary.LibsAPI
{
    public class ConsoleHider
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private int flag = 0;

        public void UpdateWindow()
        {
            ShowWindow(GetConsoleWindow(), flag);
        }

        public void VisibleWindow()
        {
            flag = 1;

            UpdateWindow();
        }

        public void HideWindow()
        {
            flag = 0;

            UpdateWindow();
        }
    }
}
