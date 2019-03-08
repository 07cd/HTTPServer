using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace WebServerMultiThreaded
{
    internal class Response
    {
        private readonly byte[] data;
        private readonly string status;
        private readonly string type;

        private Response(string status, string type, byte[] data)
        {
            this.status = status;
            this.data = data;
            this.type = type;
        }

        public static Response SendResponse(Request request)
        {
            if (request == null) return NullRequest();
            if (request.Type == "GET")
            {
                Console.WriteLine(request.URL);
                var file = Environment.CurrentDirectory + Server.HOST_DIR + request.URL;
                var fileInfo = new FileInfo(file);
                if (fileInfo.Exists) return ReturnAFile(fileInfo);
            }

            return NullRequest();
        }

        private static Response ReturnAFile(FileInfo file)
        {
            var fs = file.OpenRead();
            var reader = new BinaryReader(fs);
            var d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("200 OK", "html/text", d);
        }

        public static Response NullRequest()
        {
            var file = Environment.CurrentDirectory + Server.ERR_DIR + "400.html";
            var fi = new FileInfo(file);
            var fs = fi.OpenRead();
            var reader = new BinaryReader(fs);
            var d = new byte[fs.Length];
            reader.Read(d, 0, d.Length);
            fs.Close();
            return new Response("400 Bad Request", "html/text", d);
        }

        public void Post(NetworkStream stream)
        {
            var sbHeader = new StringBuilder();
            sbHeader.AppendLine(Server.VERSION + " " + status);
            // CONTENT-LENGTH
            sbHeader.AppendLine("Content-Length: " + data.Length);
            sbHeader.AppendLine("Date: " + DateTime.Now);
            sbHeader.AppendLine("Server: " + Server.SERVER);
            sbHeader.AppendLine("Accept-Ranges: " + "bytes" );
            sbHeader.AppendLine("ETag: " + "");

            // Append one more line breaks to seperate header and content.
            sbHeader.AppendLine();
            var response = new List<byte>();
            // response.AddRange(bHeadersString);
            response.AddRange(Encoding.ASCII.GetBytes(sbHeader.ToString()));
            response.AddRange(data);
            var responseByte = response.ToArray();
            stream.Write(responseByte, 0, responseByte.Length);
        }
    }
}
