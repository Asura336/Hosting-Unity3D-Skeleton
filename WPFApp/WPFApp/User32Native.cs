using System.Runtime.InteropServices;

namespace WPFApp
{
    internal static partial class User32Native
    {
        public const int GWLP_WNDPROC = -4;
        public const int WM_INPUT = 0x00FF;
        public const int WM_ACTIVATE = 0x0006;
        public const int WA_ACTIVE = 1;
        public const int WA_INACTIVE = 0;

        [LibraryImport("user32.dll")]
        public static partial int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [LibraryImport("user32.dll", SetLastError = true)]
        public static partial int GetWindowLong(IntPtr hWnd, int nIndex);

        [LibraryImport("user32.dll")]
        public static partial IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool MoveWindow(IntPtr handle, int x, int y, int width, int height, [MarshalAs(UnmanagedType.Bool)] bool redraw);

        public delegate int WindowEnumProc(IntPtr hwnd, IntPtr lparam);
        [LibraryImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool EnumChildWindows(IntPtr hwnd, WindowEnumProc func, IntPtr lParam);

        [LibraryImport("user32.dll", EntryPoint = "SendMessageA")]
        public static partial int SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [LibraryImport("user32.dll", EntryPoint = "DestroyWindowW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool DestroyWindow(IntPtr hwnd);
    }
}
