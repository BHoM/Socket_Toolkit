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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BH.Adapter.Socket
{
    public abstract class DataTransmitter
    {

        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/


        /***************************************************/
        /**** Abstract Methods                          ****/
        /***************************************************/

        protected abstract void HandleNewData(byte[] data, TcpClient source);


        /***************************************************/
        /**** Protected Methods                         ****/
        /***************************************************/

        protected void ListenToClient(TcpClient client)
        {
            int messageSize = 0;
            int bytesRead = 0;
            byte[] sizeBuffer = new byte[4];
            byte[] messageBuffer = null;
            NetworkStream stream = client.GetStream();

            while (true)
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
                        int toRead = Math.Min(available, messageSize - bytesRead);
                        stream.Read(messageBuffer, bytesRead, toRead);
                        bytesRead += toRead;
                        if (bytesRead == messageSize)
                        {
                            messageSize = 0;
                            HandleNewData(messageBuffer, client);
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

        protected bool SendToClient(TcpClient client, DataPackage package)
        {

            MemoryStream memory = new MemoryStream();
            BsonSerializer.Serialize(new BsonBinaryWriter(memory), typeof(DataPackage), package);

            return SendToClient(client, memory.ToArray());
        }

        /***************************************************/

        protected bool SendToClient(TcpClient client, byte[] data)
        {
            if (!client.Connected || !client.Client.Poll(500, SelectMode.SelectWrite))
                return false; // Still sending data

            Debug.WriteLine("Sending to client at " + (DateTime.Now.Ticks / (TimeSpan.TicksPerSecond / 10)).ToString());

            NetworkStream stream = client.GetStream();

            // First send the size of the message
            Int32 value = IPAddress.HostToNetworkOrder(data.Length); //Convert long from Host Byte Order to Network Byte Order
            stream.Write(BitConverter.GetBytes(value), 0, sizeof(Int32));
            stream.Flush();

            // Then send the message itself
            stream.Write(data, 0, data.Length);
            stream.Flush();

            Debug.WriteLine("Sent to client at " + (DateTime.Now.Ticks / (TimeSpan.TicksPerSecond / 10)).ToString());

            return true;
        }

        /***************************************************/
    }
}
