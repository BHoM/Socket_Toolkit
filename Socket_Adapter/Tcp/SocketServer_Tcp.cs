using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BH.Adapter.Socket
{
    public class SocketServer_Tcp : DataTransmitter
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketServer_Tcp()
        {
        }

        /***************************************************/

        ~SocketServer_Tcp()
        {
            Stop();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Start(int port = 8888)
        {
            // Check the port value
            if (port < 3000 || port > 65000) 
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 65000");

            // Stop any existing server
            if (m_Listener != null)
            { 
                m_Listener.Stop();
                m_Listener = null;
            }

            try
            {
                // Start new server
                m_Port = port;
                m_Listener = new TcpListener(IPAddress.Any, m_Port);
                m_Listener.Start();

                // Start accepting client
                m_Listener.BeginAcceptTcpClient(AcceptClient, m_Listener);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /***************************************************/

        public void Stop()
        {
            // Stop the server
            if (m_Listener != null)
                m_Listener.Stop();
            m_Listener = null;
        }

        /***************************************************/

        public bool IsActive()
        {
            return (m_Listener != null);
        }


        /***************************************************/
        /**** Inherited Methods                         ****/
        /***************************************************/

        protected override void HandleNewData(byte[] data, TcpClient source)
        {
            foreach (TcpClient client in m_Clients)
            {
                if (source != client)
                    SendToClient(client, data);
            }
                
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void AcceptClient(IAsyncResult ar)
        {
            try
            {
                TcpListener listener = (TcpListener)ar.AsyncState;
                TcpClient client = listener.EndAcceptTcpClient(ar);
                m_Clients.Add(client);
                Thread clientThread = new Thread(() => ListenToClient(client));
                clientThread.Start();

                if (m_LastMessage != null)
                    SendToClient(client, m_LastMessage);

                m_Listener.BeginAcceptTcpClient(AcceptClient, m_Listener);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to accept client: " + e);
            }
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private TcpListener m_Listener = null;
        private List<TcpClient> m_Clients = new List<TcpClient>();
        private byte[] m_LastMessage = null;

    }
}
