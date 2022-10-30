using System;
using System.IO;
using System.IO.Pipes;

namespace NamedPipesServer
{
    public class NamedPipeClient : IDisposable
    {
        private readonly NamedPipeClientStream client;
        private StreamReader reader;
        private StreamWriter writer;
        private NamedPipeClient(string pipeName, int timeOut)
        {
            client = new NamedPipeClientStream(pipeName);
            client.Connect(100);
            reader = new StreamReader(client);
            writer = new StreamWriter(client);
            writer.AutoFlush = true;
        }
        public NamedPipeClient(string pipeName) : this(pipeName, 5000) { }
        public void Dispose()
        {
           writer.Dispose();
           reader.Dispose();
           client.Dispose();
        }
        public string SendRequest(string request)
        {
            if (request != null)
            {
                try
                {
                    writer.WriteLine(request);
                    return reader.ReadLine();
                }
                catch (Exception ex)
                {
                    return string.Format("{0}\r\nDetails:\r\n{1}", "Error on server communication.", ex.Message);
                }
            }
            else
            {
                return "Error. Null request.";
            }
        }
    }
}
