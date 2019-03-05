using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

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
            //if (request == null) return NullRequest();
            if (request.Type == "GET")
            {
                Console.WriteLine(request.URL);
                var file = Environment.CurrentDirectory + Server.HOST_DIR + request.URL;
                var fileInfo = new FileInfo(file);
                if (fileInfo.Exists) return ReturnAFile(fileInfo);
            }

            if (request.Type == "POST")
            {
                var d = GetParams(request.URL);

                Console.WriteLine(d);

                foreach (var m in d)
                {
                    Console.WriteLine(m);
                }

                // should return something here
            }


            return NullRequest();
        }

        static Dictionary<string, string> GetParams(string uri)
        {
            var matches = Regex.Matches(uri, @"[\?&](([^&=]+)=([^&=#]*))", RegexOptions.Compiled);
            return matches.Cast<Match>().ToDictionary(
                m => Uri.UnescapeDataString(m.Groups[2].Value),
                m => Uri.UnescapeDataString(m.Groups[3].Value)
            );
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

/*
    Headers to return

   - HTTP/1.1 200 OK 
   - Date: Mon, 27 Jul 2009 12:28:53 GMT
   - Server: Apache
   Last-Modified: Wed, 22 Jul 2009 19:15:56 GMT
   ETag: "34aa387-d-1568eb00"
   Accept-Ranges: bytes
   - Content-Length: 51
   Vary: Accept-Encoding
   - Content-Type: text/plain
 */
