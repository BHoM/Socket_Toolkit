using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace BH.Adapter.Socket
{
    public class SocketServer
    {
        public delegate void DataEvent(List<object> data);


        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public event DataEvent DataObservers;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketServer(int port = 8888)
        {
            Start(port);
        }

        /***************************************************/

        ~SocketServer()
        {
            Stop();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Start(int port = 8888)
        {
            // Check the port value
            if (port < 3000 || port > 49000) 
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 49000");

            // Stop any existing server
            if (m_Listener != null)
            { 
                m_Listener.Stop();
                m_Listener = null;
            }

            // Start new server
            m_Port = port;
            m_Listener = new TcpListener(IPAddress.Any, m_Port);
            m_Listener.Start();

            // Start accepting client
            m_Listener.BeginAcceptTcpClient(AcceptClient, m_Listener);

            return true;
        }

        /***************************************************/

        public void Stop()
        {
            // Stop the server
            m_Listener.Stop();
            m_Listener = null;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void AcceptClient(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient client = listener.EndAcceptTcpClient(ar);
            m_Clients.Add(client);
            Thread clientThread = new Thread(() => ListenToClient(client));
            clientThread.Start();
        }

        /***************************************************/

        private void ListenToClient(TcpClient client)
        {
            int messageSize = 0;
            int bytesRead = 0;
            byte[] sizeBuffer = new byte[4];
            byte[] messageBuffer = null;
            NetworkStream stream = client.GetStream();

            while(true)
            {
                int available = client.Available;
                if (messageSize == 0) 
                {
                    if (available >= 4)
                    {
                        stream.Read(sizeBuffer, 0, 4);
                        if (BitConverter.IsLittleEndian)
                            Array.Reverse(sizeBuffer);

                        messageSize = BitConverter.ToInt32(sizeBuffer, 0);
                        bytesRead = 0;
                        messageBuffer = new byte[messageSize];
                    }
                }
                else
                {
                    if (available > 0)
                    {
                        stream.Read(messageBuffer, bytesRead, available);
                        bytesRead += available;
                        if (bytesRead == messageSize)
                        {
                            List<object> objects = BsonSerializer.Deserialize(messageBuffer, typeof(List<object>)) as List<object>;
                            messageSize = 0;
                            if (DataObservers != null)
                                DataObservers.Invoke(objects);
                        }
                        else if (bytesRead > messageSize)
                        {
                            throw new Exception("Incorrect message received. Exceeded expected size of " + messageSize + " bytes.");
                        }
                    }
                }

                if (available == 0)
                    Thread.Sleep(100);
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private TcpListener m_Listener = null;
        private List<TcpClient> m_Clients = new List<TcpClient>();

    }
}
