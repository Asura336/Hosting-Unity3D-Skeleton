using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using static WPFApp.User32Native;

namespace WPFApp
{
    internal partial class UnityProcessHost : HwndHost
    {
        Process? m_unityProcess;
        nint m_unityHandle;

        const string unityExeFileName = "UnityApp_Build/UnityApp.exe";

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            try
            {
                var _startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(Environment.CurrentDirectory, unityExeFileName),
                    Arguments = $"-parentHWND {hwndParent.Handle} {Environment.CommandLine}",
                    UseShellExecute = true,
                    CreateNoWindow = true,
                };

                m_unityProcess = Process.Start(_startInfo);
                if (m_unityProcess != null)
                {
                    m_unityProcess.WaitForInputIdle(10_000);

                    const int repeatNumber = 50;
                    int repeat = repeatNumber;
                    do
                    {
                        if (repeat != repeatNumber)
                        {
                            Thread.Sleep(100);
                        }
                        EnumChildWindows(hwndParent.Handle, WindowEnum, IntPtr.Zero);
                    } while (m_unityHandle == IntPtr.Zero && repeat-- > 0);
                    if (m_unityHandle == IntPtr.Zero)
                    {
                        m_unityProcess?.Kill();
                        throw new NotSupportedException("Unity HWnd not found.");
                    }

                    ActivateUnityWindow();
                    System.Windows.Application.Current.Exit += Current_Exit;
                    // end init
                    return new HandleRef(this, m_unityHandle);
                }
            }
            catch (Exception)
            {
                m_unityProcess?.Kill();
                m_unityProcess = null;
                throw;
            }

            return default;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            var size = sizeInfo.NewSize;
            MoveWindow(m_unityHandle, 0, 0, (int)size.Width, (int)size.Height, true);
            _ = SendMessage(m_unityHandle, WM_ACTIVATE, WA_ACTIVE, 0);
            base.OnRenderSizeChanged(sizeInfo);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            DestroyWindow(hwnd.Handle);
        }

        protected override nint WndProc(nint hwnd, int msg, nint wParam, nint lParam, ref bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            m_unityProcess?.Kill();
            m_unityProcess = null;
        }

        int WindowEnum(IntPtr hwnd, IntPtr lparam)
        {
            m_unityHandle = hwnd;
            return 0;
        }

        public void ActivateUnityWindow()
        {
            _ = SendMessage(m_unityHandle, WM_ACTIVATE, WA_ACTIVE, 0);
        }

        void DeactivateUnityWindow()
        {
            _ = SendMessage(m_unityHandle, WM_ACTIVATE, WA_INACTIVE, 0);
        }
    }
}
