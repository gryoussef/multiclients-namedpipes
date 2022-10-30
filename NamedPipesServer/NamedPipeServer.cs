using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NamedPipesServer
{
    public class NamedPipeServer : IDisposable
    {
        public readonly string pipeName;
        private readonly int maxNumberOfServerInstances;
        private List<NamedPipeServerInstance> servers = new List<NamedPipeServerInstance>();
        public event EventHandler<PipeMsgEventArgs> NewRequestEvent;
        public NamedPipeServer(string pipeName, int maxNumberOfServerInstances)
        {
            this.pipeName = pipeName;
            this.maxNumberOfServerInstances = maxNumberOfServerInstances;
            NewServerInstance();
        }
        private void NewServerInstance()
        {
            if (servers.Count < maxNumberOfServerInstances)
            {
                var server = new NamedPipeServerInstance(pipeName, maxNumberOfServerInstances, servers.Count - 1);
                server.newServerInstanceEvent += (s, e) => NewServerInstance();
                server.newRequestEvent += (s, e) => NewRequestEvent.Invoke(s, e);
                servers.Add(server);
            }
             CleanServers(false);
        }
        private void CleanServers(bool disposeAll)
        {
            if (disposeAll)
            {
                foreach (var server in servers)
                {
                    server.Dispose();
                }
            }
            else
            {
                for (int i = servers.Count - 1; i >= 0; i--)
                {
                    if (servers[i] == null)
                    {
                        servers.RemoveAt(i);
                    }
                    else if (servers[i].TaskCommunication != null &&
                        (servers[i].TaskCommunication.Status == TaskStatus.RanToCompletion ||
                        servers[i].TaskCommunication.Status == TaskStatus.Canceled ||
                        servers[i].TaskCommunication.Status == TaskStatus.Faulted))
                    {
                        servers[i].Dispose();
                        servers.RemoveAt(i);
                    }
                }
            }
        }
        public void Dispose()
        {
            CleanServers(true);
        }
    }
}
