// Adapted from https://github.com/jrusbatch/MSBuild.IISExpress/blob/fd5b26f890bac28c319a7f2dd59b37bf7acb44b8/src/Core/NativeMethods.cs

using System;
using System.Runtime.InteropServices;

namespace TestDrive.SmokeTests.IISExpress
{
    internal static class NativeMethods
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetTopWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    }
}