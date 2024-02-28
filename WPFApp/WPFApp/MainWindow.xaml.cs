using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace WPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly PipeServer m_server;

        /* 窗口装载 Unity 窗口作为控件后如果同一窗口还有其他控件，
         * 输入焦点可能无法正确传递给 Unity 窗口（推测），
         * 表现为与其他 WPF 控件交互（比如按一次按钮）后 Unity 窗口可能无法继续响应键盘输入，
         * 包括文本框
         * 
         * 拖拽窗口边缘以改变窗口尺寸后 Unity 窗口可以响应键盘输入
         * 
         * 鼠标事件不受影响，Unity 部分可以正确响应输入（视口被视为全屏）
         * 
         * 通过监听键盘焦点，以及主动清除焦点，问题可能不完全在焦点上
         * 
         * 解决办法
         *   a. 从 Unity 窗口所在的 WPF 窗口转发按键消息
         *   b. Unity 部分不使用关联按键的事件，快捷键、文本输入等由 WPF 部分处理
         *   c. 承载 Unity 的窗口不加入其他可聚焦的控件，类似 VS 那样使用多窗口
         */

        bool m_keyboardListenerWorking = false;

        public MainWindow()
        {
            InitializeComponent();

            m_server = new PipeServer();
            Application.Current.Exit += Current_Exit;

            // 监听键盘焦点，
            m_keyboardListenerWorking = true;
            ListenKeyboardFocusThread();
        }

        async void ListenKeyboardFocusThread()
        {
            while (m_keyboardListenerWorking)
            {
                var _kbFocus = Keyboard.FocusedElement;
                this.Text_KBFocus.Content = $"{(_kbFocus as ContentControl)?.Content}({_kbFocus?.GetType().Name ?? "Nothing"})";

                var _focus = FocusManager.FocusedElementProperty;
                this.Text_Focus.Content = $"{_focus.Name}(index = {_focus.GlobalIndex})";

                await Task.Delay(50);
            }
        }

        private void Current_Exit(object sender, ExitEventArgs e)
        {
            m_keyboardListenerWorking = false;
            m_server.Dispose();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            if (!e.Cancel)
            {
                var result = MessageBox.Show("exit?", "Process will exit", MessageBoxButton.YesNo);
                e.Cancel = result != MessageBoxResult.Yes;
            }
        }

        private void Msg_Plus_Click(object sender, RoutedEventArgs e)
        {
            string? result = m_server.Send("plus");
            Msg_Result.Content = result ?? "(Nothing)";
        }

        private async void Msg_Reset_Click(object sender, RoutedEventArgs e)
        {
            string? result = m_server.Send("reset");
            Msg_Result.Content = result ?? "(Nothing)";

            // 点击此按钮后主动清除键盘焦点，Unity 侧依然无法正确响应键盘输入
            await Task.Yield();
            Keyboard.ClearFocus();
        }
    }
}