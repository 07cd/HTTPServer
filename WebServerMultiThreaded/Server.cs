using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace WebServerMultiThreaded
{
    internal class Server
    {
        private TcpListener _listener;
        public string Localhost = "127.0.0.1";
        public const string HOST_DIR = "/root/host";
        public const string ERR_DIR = "/root/err/";
        public const string VERSION = "HTTP/1.1";
        public const string SERVER = "SimpleServe";


        public void StartServer()
        {
            try
            {
                const int port = 3000;
                var localAddr = IPAddress.Parse(Localhost);
                _listener = new TcpListener(localAddr, port);
                _listener.Start();

                string data = null;

                while (true)
                {
                    //var client =  _listener.AcceptTcpClient();
                    var client = _listener.AcceptTcpClientAsync();
                    var thread = new Thread(() => HandleClient(client.Result));
                    thread.Start();
                    thread.Join();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public void HandleClient(TcpClient client)
        {
            try
            {
                var streamReader = new StreamReader(client.GetStream());

                var request = Request.DeConstructStreamReaderToString(streamReader);
                var response = Response.SendResponse(request);
                response.Post(client.GetStream());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            client.Close();
        }
    }
}