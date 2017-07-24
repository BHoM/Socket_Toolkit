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
            if (port < 3000 || port > 49000)
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 49000");

            // Set things up
            m_Port = port;
            m_Client = new TcpClient(server, port);
        }

        /***************************************************/

        ~SocketLink()
        {
            
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool SendData(List<object> objects)
        {
            MemoryStream memory = new MemoryStream();
            BsonSerializer.Serialize(new BsonBinaryWriter(memory), typeof(object), objects.Select(x => x.ToBsonDocument()));

            return SendData(memory.ToArray());
        }

        /***************************************************/

        public bool SendData(byte[] data)
        {
            NetworkStream stream = m_Client.GetStream();

            // First send teh size of the message
            Int32 value = IPAddress.HostToNetworkOrder(data.Length); //Convert long from Host Byte Order to Network Byte Order
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int32));

            // Then send the message itself
            stream.Write(data, 0, data.Length);

            return true;
        }

        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/




        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private TcpClient m_Client = null;

    }
}
