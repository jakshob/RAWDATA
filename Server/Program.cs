using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace Assignment3TestSuite
{

    public class Response
    {
        public string Status { get; set; }
        public string Body { get; set; }
    }

    public class Category
    {
        [JsonProperty("cid")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var server = new TcpListener(IPAddress.Parse("127.0.0.1"), 5000);
            server.Start();
            Console.WriteLine("Server started ...");

            while (true)
            {

               

                var client = server.AcceptTcpClient();

                var strm = client.GetStream();

                var buffer = new byte[client.ReceiveBufferSize];

                var readCnt = strm.Read(buffer, 0, buffer.Length);

                var msg = Encoding.UTF8.GetString(buffer, 0, readCnt);

                Console.WriteLine($"Message: {msg}");

                var payload = Encoding.UTF8.GetBytes(msg.ToUpper());

                strm.Write(payload, 0, payload.Length);

                strm.Close();

                client.Close();
            }
        }

    }
        /**********************************************************
         * 
         *  Helper Methods
         * 
        **********************************************************/

        private static string UnixTimestamp()
        {
            return DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
        }

        private static TcpClient Connect()
        {
            IPEndPoint ipLocalEndPoint = new IPEndPoint(IPAddress.Loopback, Port);
            var client = new TcpClient();
            client.Connect(ipLocalEndPoint);
            return client;
        }

    }

    /**********************************************************
    * 
    *  Helper Clases
    * 
    **********************************************************/

    public static class Util
    {
        public static string ToJson(this object data)
        {
            return JsonConvert.SerializeObject(data,
            new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public static T FromJson<T>(this string element)
        {
            return JsonConvert.DeserializeObject<T>(element);
        }

        public static void SendRequest(this TcpClient client, string request)
        {
            var msg = Encoding.UTF8.GetBytes(request);
            client.GetStream().Write(msg, 0, msg.Length);
        }

        public static Response ReadResponse(this TcpClient client)
        {
            var strm = client.GetStream();
            //strm.ReadTimeout = 250;
            byte[] resp = new byte[2048];
            using (var memStream = new MemoryStream())
            {
                int bytesread = 0;
                do
                {
                    bytesread = strm.Read(resp, 0, resp.Length);
                    memStream.Write(resp, 0, bytesread);

                } while (bytesread == 2048);
                
                var responseData = Encoding.UTF8.GetString(memStream.ToArray());
                return JsonConvert.DeserializeObject<Response>(responseData);
            }
        }
    }
}