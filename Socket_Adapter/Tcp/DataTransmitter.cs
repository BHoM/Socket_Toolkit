/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2021, the respective contributors. All rights reserved.
 *
 * Each contributor holds copyright over their respective contributions.
 * The project versioning (Git) records all such contribution source information.
 *                                           
 *                                                                              
 * The BHoM is free software: you can redistribute it and/or modify         
 * it under the terms of the GNU Lesser General Public License as published by  
 * the Free Software Foundation, either version 3.0 of the License, or          
 * (at your option) any later version.                                          
 *                                                                              
 * The BHoM is distributed in the hope that it will be useful,              
 * but WITHOUT ANY WARRANTY; without even the implied warranty of               
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the                 
 * GNU Lesser General Public License for more details.                          
 *                                                                            
 * You should have received a copy of the GNU Lesser General Public License     
 * along with this code. If not, see <https://www.gnu.org/licenses/lgpl-3.0.html>.      
 */

using BH.oM.Adapters.Socket;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
            if (client == null)
                return;

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
            BsonDocument doc = BH.Engine.Serialiser.Convert.ToBson(package);
            BsonSerializer.Serialize(new BsonBinaryWriter(memory), typeof(BsonDocument), doc);

            return SendToClient(client, memory.ToArray());
        }

        /***************************************************/

        protected bool SendToClient(TcpClient client, byte[] data)
        {
            if (!client.Connected || !client.Client.Poll(500, SelectMode.SelectWrite))
                return false; // Still sending data

            NetworkStream stream = client.GetStream();

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
    }
}


