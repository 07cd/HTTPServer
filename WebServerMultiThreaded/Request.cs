using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServerMultiThreaded
{
    internal class Request
    {
       
        public string Type { get; set; }
        public string URL { get; set; }
        public string Host { get; set; }

        public Request(string type, string url, string host)
        {
            Type = type;
            URL = url;
            Host = host;
        }

        public static Request DeConstructStreamReaderToString(StreamReader streamReader)
        {
            // converts the streamReader into a string
            string request = "";
            while (streamReader.Peek() != -1)
            {
                request += streamReader.ReadLine() + "\n";
            }

            // check if its not null
            if (string.IsNullOrEmpty(request))
                return null;

            // gets specific headers from the streamReader that was converted above
            var header = request.Split(' ');
            var type = header[0];
            var url = header[1];
            var host = header[4];

            return new Request(type, url, host);
        }

    }
}
