using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BH.Adapter.Socket
{
    public class SocketLink
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketLink(string server, int port = 8888)
        {
            // Check the port value
            if (port < 3000 || port > 65000)
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 65000");

            // Set things up
            m_Port = port;
            m_ServerName = server;
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    m_Client = new TcpClient(server, port);
                    break;
                }
                catch(Exception)
                {
                    m_Client = null;
                    Thread.Sleep(500);
                }
            }

            if (m_Client == null)
                throw new Exception("The socket link failed to connect to port " + port);
            
        }

        /***************************************************/

        ~SocketLink()
        {
            if (m_Client != null)
                m_Client.Close();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool SendData(List<object> objects)
        {

            MemoryStream memory = new MemoryStream();
            BsonSerializer.Serialize(new BsonBinaryWriter(memory), typeof(List<BsonDocument>), objects.Select(x => x.ToBsonDocument()).ToList());

            return SendData(memory.ToArray());
        }

        /***************************************************/

        public bool SendData(byte[] data)
        {
            if (m_Client == null)
            {
                try { m_Client = new TcpClient(m_ServerName, m_Port); }
                catch (Exception) { m_Client = null; }

                if (m_Client == null)
                    throw new Exception("The socket link failed to connect to port " + m_Port);
            }


            if (!m_Client.Client.Poll(500, SelectMode.SelectWrite))
                return false; // Still sending data
                
            NetworkStream stream = m_Client.GetStream();

            // First send the size of the message
            Int32 value = IPAddress.HostToNetworkOrder(data.Length); //Convert long from Host Byte Order to Network Byte Order
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int32));
            stream.Flush();

            // Then send the message itself
            stream.Write(data, 0, data.Length);
            stream.Flush();

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/




        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private string m_ServerName = "";
        private TcpClient m_Client = null;

    }
}
