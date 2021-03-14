using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BroadcastMany
{
    class Program
    {
        private static HashSet<TcpClient> clients = new HashSet<TcpClient>();
        private static string MsgToSend;
        static void Main(string[] args)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, 9999);
            TcpClient client;
            listener.Start();

            while (true) 
            {
                client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);
            }
        }
        private static void ThreadProc(object obj)
        {
            var Client = (TcpClient)obj;
            clients.Add(Client);

            // Buffer for reading data
            Byte[] Bytes = new Byte[1024];
            string StrBytes = null;

            NetworkStream stream = Client.GetStream();
            
            int i;
            while ((i = stream.Read(Bytes, 0, Bytes.Length)) != 0)
            {
                StrBytes = System.Text.Encoding.ASCII.GetString(Bytes, 0, i);
                MsgToSend = MsgToSend + StrBytes;
                if (StrBytes == "\r\n")
                {

                    MsgToSend = MsgToSend + Environment.NewLine;
                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(MsgToSend);
                    foreach (var c in clients)
                    {
                        if (Client != c)
                        {
                            NetworkStream s = c.GetStream();
                            s.Write(msg, 0, msg.Length);
                        }
                    }
                    MsgToSend = "";
                }
            }
        }
    }
}






//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace BroadcastMany
//{
//    class Program
//    {
//        private HashSet<NetworkStream> streams = new HashSet<NetworkStream>();
//        static void Main(string[] args)
//        {
//            TcpListener listener = new TcpListener(IPAddress.Any, 9090);
//            TcpClient client;
//            listener.Start();

//            while (true) // Add your exit flag here
//            {
//                client = listener.AcceptTcpClient();
//                ThreadPool.QueueUserWorkItem(ThreadProc, client);
//            }
//        }
//        private static void ThreadProc(object obj)
//        {
//            var client = (TcpClient)obj;
//            // Buffer for reading data
//            Byte[] bytes = new Byte[256];
//            String data = null;

//            //data = null;

//            // Get a stream object for reading and writing
//            NetworkStream stream = client.GetStream();


//            int i;

//            // Loop to receive all the data sent by the client. 
//            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
//            {
//                // Translate data bytes to a ASCII string.
//                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
//                Console.WriteLine("Received: {0}", data);

//                // Process the data sent by the client.
//                data = data.ToUpper();

//                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

//                // Send back a response.
//                stream.Write(msg, 0, msg.Length);
//                Console.WriteLine("Sent: {0}", data);
//            }
//        }
//    }
//}
