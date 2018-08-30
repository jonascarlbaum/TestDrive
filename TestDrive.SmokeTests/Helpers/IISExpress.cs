using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace TestDrive.SmokeTests.Helpers
{
    public static class IISExpress
    {
        private static readonly List<string> sites = new List<string>();
        private static readonly List<string> paths = new List<string>();
        
        internal class NativeMethods
        {
            // Methods
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetTopWindow(IntPtr hWnd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);
            [DllImport("user32.dll", SetLastError = true)]
            internal static extern bool PostMessage(HandleRef hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        }

        public static Process StartIISExpress(string site, int port = 7329)
        {
            if (!sites.Contains(site.ToLower()))
                sites.Add(site.ToLower());
            else return null;

            var index = Environment.CurrentDirectory.LastIndexOf("\\bin\\");
            var projectDir = Environment.CurrentDirectory.Remove(index);
            var solutionDir = System.IO.Directory.GetParent(projectDir);
            var path = solutionDir + "\\" + site;

            var arguments = new StringBuilder();
            arguments.Append(@"/path:");
            arguments.Append(path);
            arguments.Append(@" /Port:" + port);
            // arguments.Append(@"/site:" + site);

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\IIS Express\\iisexpress.exe",
                Arguments = arguments.ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });
            
            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains("Application started."))
                    break;
            }

            return process;
        }
        
        public static Process StartIISExpressFromPath(string path, int port = 7329)
        {
            if (!paths.Contains(path.ToLower()))
                paths.Add(path.ToLower());
            else return null;

            var arguments = new StringBuilder();
            arguments.Append(@"/path:" + path);
            arguments.Append(@" /Port:" + port);

            var process = Process.Start(new ProcessStartInfo()
            {
                FileName = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) + "\\IIS Express\\iisexpress.exe",
                Arguments = arguments.ToString(),
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            while (!process.StandardOutput.EndOfStream)
            {
                string line = process.StandardOutput.ReadLine();
                if (line.Contains("Application started."))
                    break;
            }

            return process;
        }

        public static void Stop(this Process process)
        {
            try
            {
                for (IntPtr ptr = NativeMethods.GetTopWindow(IntPtr.Zero); ptr != IntPtr.Zero; ptr = NativeMethods.GetWindow(ptr, 2))
                {
                    NativeMethods.GetWindowThreadProcessId(ptr, out uint num);
                    if (process?.Id == num)
                    {
                        HandleRef hWnd = new HandleRef(null, ptr);
                        NativeMethods.PostMessage(hWnd, 0x12, IntPtr.Zero, IntPtr.Zero);
                        return;
                    }
                }
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine("Couldn't stop process", e);
            }
        }
    }
}
