using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using UnityEngine;

namespace UnitySide
{
    public class PipeClient : MonoBehaviour
    {
        [SerializeField] string pipeName = "pipe.teriteri";
        bool m_working = false;

        Thread m_client;
        SynchronizationContext m_syncContext;
        NamedPipeClientStream m_pipeClient;

        string m_messageResult;

        /* 跨进程递送消息
         *   - 程序入口定为外部应用（比如 WPF 应用），外部应用作为服务端，Unity 应用作为客户端
         *   - 同步读写：服务端发指令，客户端接收后回传结果，服务端立即读取结果
         *     - 服务端随机时间发消息
         *     - 客户端始终监听读
         *     - 读取后立即（阻塞）返回结果
         *     - 客户端随后读消息
         *  - 异步读写？
         *     - 用同步读写立即返回一个序号
         *     - 另开一个从客户端发消息、服务端监听读的管道
         *   
         * 
         * 数据
         *   文本 vs 二进制
         *   简单起见单行指令直接使用文本？
         *   
         * ----
         * magic number
         * ----
         *   id : uint
         *   type : enum, short
         *   size : uint
         *   buffer : bytes
         * ----
         */

        private void Awake()
        {
            m_syncContext = SynchronizationContext.Current;
            m_pipeClient = new NamedPipeClientStream(".", pipeName,
                PipeDirection.InOut);
            m_client = new Thread(() =>
            {
                var pipe = m_pipeClient;
                try
                {
                    pipe.Connect(3000);
                    if (pipe.IsConnected)
                    {
                        m_working = true;
                        InputMessage("prepared");

                        //then...
                        using var sr = new StreamReader(pipe, Encoding.UTF8);
                        using var sw = new StreamWriter(pipe, Encoding.UTF8);
                        while (m_working)
                        {
                            // read
                            string input;
                            if ((input = sr.ReadLine()) != null)
                            {
                                InputMessage(input);
                                m_pipeClient.Flush();
                                // write
                                // 加延时模拟阻塞，如果在客户端的处理过程是阻塞的，服务端也会同步等待
                                //Thread.Sleep(1000);
                                sw.WriteLine(m_messageResult);
                                sw.Flush();
                            }
                        }
                    }
                    else
                    {
                        InputMessage("timeout");
                    }
                }
                catch (TimeoutException)
                {
                    InputMessage("failed");
                }
            })
            {
                IsBackground = true,
            };
            m_client.Start();
        }

        private void OnDestroy()
        {
            m_working = false;
            m_pipeClient.Dispose();
        }

        void InputMessage(string message)
        {
            m_syncContext.Send(m =>
            {
                MessageBus.Instance.InputMessage((string)m, out m_messageResult);
            }, message);
            //MessageBus.Instance.InputMessage(message);
        }
    }
}