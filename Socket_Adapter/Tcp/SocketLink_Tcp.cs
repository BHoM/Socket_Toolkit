using BH.Adapter.Socket.Tcp;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BH.Adapter.Socket
{
    public class SocketLink_Tcp : DataTransmitter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public event DataEvent DataObservers;

        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketLink_Tcp(int port = 8888, string address = "127.0.0.1")
        {
            // Check the port value
            if (port < 3000 || port > 65000)
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 65000");

            // Make sure the server already exists
            if (address == "127.0.0.1" && !Global.TcpServers.ContainsKey(port))
            {
                Global.TcpServers[port] = new SocketServer_Tcp();
                Global.TcpServers[port].Start(port);
            }
                

            // Set things up
            m_Port = port;
            m_Address = address;

            // Try to connect to server
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    m_Client = new TcpClient(address, port);
                    break;
                }
                catch (Exception)
                {
                    m_Client = null;
                    Thread.Sleep(500);
                }
            }

            if (m_Client == null)
                throw new Exception("The socket link failed to connect to port " + port);

            // Start listening for server
            Thread listeningThread = new Thread(() => ListenToClient(m_Client));
            listeningThread.Start();
        }

        /***************************************************/

        ~SocketLink_Tcp()
        {
            if (m_Client != null)
                m_Client.Close();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool SendData(List<object> data, string tag = "")
        {
            if (m_Client == null)
            {
                try { m_Client = new TcpClient(m_Address, m_Port); }
                catch (Exception) { m_Client = null; }

                if (m_Client == null)
                    throw new Exception("The socket link failed to connect to port " + m_Port);
            }

            Console.WriteLine("Sending Data at " + (DateTime.Now.Ticks / (TimeSpan.TicksPerSecond / 10)).ToString());
            return SendToClient(m_Client, new DataPackage(data, tag));
        }

        /***************************************************/

        public bool SendData(byte[] data)
        {
            if (m_Client == null)
            {
                try { m_Client = new TcpClient(m_Address, m_Port); }
                catch (Exception) { m_Client = null; }

                if (m_Client == null)
                    throw new Exception("The socket link failed to connect to port " + m_Port);
            }

            return SendToClient(m_Client, data);
        }


        /***************************************************/
        /**** Inherited Methods                         ****/
        /***************************************************/

        protected override void HandleNewData(byte[] data, TcpClient source)
        {
            Debug.WriteLine("Received Data at " + (DateTime.Now.Ticks / (TimeSpan.TicksPerSecond / 10)).ToString());
            Console.WriteLine("Received Data at " + (DateTime.Now.Ticks / (TimeSpan.TicksPerSecond / 10)).ToString());
            if (DataObservers != null)
                DataObservers.Invoke(BsonSerializer.Deserialize(data, typeof(DataPackage)) as DataPackage);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private string m_Address = "";
        private TcpClient m_Client = null;
    }
}
