using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipesServer
{
    internal class NamedPipeServerInstance : IDisposable
    {
        private readonly NamedPipeServerStream server;
        public Task TaskCommunication;
        public event EventHandler newServerInstanceEvent;
        public event EventHandler<PipeMsgEventArgs> newRequestEvent;
        private bool disposeFlag = false;
        private int id;
        public NamedPipeServerInstance(string pipeName, int maxNumberOfServerInstances, int id)
        {
            server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, maxNumberOfServerInstances, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            this.id = id;
            var asyncResult = server.WaitForConnectionAsync().ContinueWith(OnConnected).ConfigureAwait(false);
        }
        private void OnConnected(IAsyncResult result)
        {
            if (!disposeFlag)
            {
                server.EndWaitForConnection(result);
                newServerInstanceEvent.Invoke(this, EventArgs.Empty);
                TaskCommunication = Task.Factory.StartNew(Communication);
            }
        }
        private void Communication()
        {
            using (var reader = new StreamReader(server))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                        var msgEventArgs = new PipeMsgEventArgs(line);
                        newRequestEvent.Invoke(this, msgEventArgs);
                        var response = msgEventArgs.Response + Environment.NewLine;
                        var bytes = Encoding.UTF8.GetBytes(response);
                        server.Write(bytes, 0, bytes.Length);
                }
            }
        }
        public void Dispose()
        {
            disposeFlag = true;
        }
    }
}
