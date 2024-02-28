using System.IO;
using System.IO.Pipes;

namespace WPFApp
{
    internal class PipeServer : IDisposable
    {
        public readonly string pipeName;

        readonly NamedPipeServerStream m_serverPipe;
        readonly StreamWriter m_writer;
        readonly StreamReader m_reader;

        bool m_working = false;

        public PipeServer(string pipeName = "pipe.teriteri")
        {
            this.pipeName = pipeName;
            m_serverPipe = new NamedPipeServerStream(pipeName, PipeDirection.InOut);
            m_writer = new StreamWriter(m_serverPipe);
            m_reader = new StreamReader(m_serverPipe);

            _ = Task.Run(() =>
            {
                m_serverPipe.WaitForConnection();
                m_working = true;
            });
        }

        public void Dispose()
        {
            m_working = false;
            m_serverPipe.Dispose();
        }

        public string? Send(string message)
        {
            if (m_working)
            {
                m_writer.WriteLine(message);
                m_writer.Flush();
                // recieve?
                string? result = null;
                if (m_reader.Peek() != -1)
                {
                    result = m_reader.ReadLine();
                }
                m_serverPipe.Flush();
                return result;
            }
            return null;
        }
    }
}
