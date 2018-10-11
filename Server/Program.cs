using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace EchoServer
{
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
}
