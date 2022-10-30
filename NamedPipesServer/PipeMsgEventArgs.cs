using System;

namespace NamedPipesServer
{
    public class PipeMsgEventArgs : EventArgs
    {
        public string Request { get; set; }
        public string Response { get;set; }
        public PipeMsgEventArgs()
        {
        }
        public PipeMsgEventArgs(string request)
        {
            Request = request;
        }
    }
}
