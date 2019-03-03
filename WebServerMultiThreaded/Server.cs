using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebServerMultiThreaded
{
    internal class Server
    {
        private TcpListener _listener;
        public string Localhost = "127.0.0.1";
        public const string HOST_DIR = "/root/host";
        public const string ERR_DIR = "/root/err/";
        public const string VERSION = "HTTP/1.1";


        public void StartServer()
        {
            try
            {
                const int port = 3000;
                IPAddress localAddr = IPAddress.Parse(Localhost);
                _listener = new TcpListener(localAddr, port);
                _listener.Start();

                string data = null;

                while (true)
                {
                    //var client =  _listener.AcceptTcpClient();
                    Task<TcpClient> client = _listener.AcceptTcpClientAsync();
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

                Request request = Request.DeConstructStreamReaderToString(streamReader);
                Response response = Response.SendResponse(request);
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
