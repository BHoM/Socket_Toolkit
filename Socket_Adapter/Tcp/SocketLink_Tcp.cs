/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2018, the respective contributors. All rights reserved.
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

using BH.oM.Socket;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Collections.Generic;
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

        public SocketLink_Tcp(int port = 8888, string address = "127.0.0.1", bool internalServer = true, bool local = true)
        {
            // Check the port value
            if (port < 3000 || port > 65000)
                throw new InvalidOperationException("Invalid port number. Please use a number between 3000 and 65000");

            // Make sure the server already exists
            if (internalServer && address == "127.0.0.1" && !Global.TcpServers.ContainsKey(port))
            {
                SocketServer_Tcp server = new SocketServer_Tcp();
                if (server.Start(port, local))
                    Global.TcpServers[port] = server;
            }


            // Set things up
            m_Port = port;
            m_Address = address;

            // Try to connect to server
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    m_Client = new TcpClient();
                    var result = m_Client.BeginConnect(address, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(1));
                    break;
                }
                catch (Exception)
                {
                    m_Client = null;
                    Thread.Sleep(500);
                }
            }

            if (m_Client == null)
                BH.Engine.Reflection.Compute.RecordError("The socket link failed to connect to port " + port);

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

        public bool SendData(DataPackage package)
        {
            if (m_Client == null)
            {
                try { m_Client = new TcpClient(m_Address, m_Port); }
                catch (Exception) { m_Client = null; }

                if (m_Client == null)
                    throw new Exception("The socket link failed to connect to port " + m_Port);
            }


            return SendToClient(m_Client, package);
        }

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


            return SendToClient(m_Client, new DataPackage { Data = data, Tag = tag });
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
            if (DataObservers != null)
            {
                DataPackage package;

                try
                {
                    //Try to deserialise the data
                    BsonDocument doc = BsonSerializer.Deserialize(data, typeof(BsonDocument)) as BsonDocument;
                    if (doc == null) return;

                    package = BH.Engine.Serialiser.Convert.FromBson(doc) as DataPackage;
                }
                catch
                {
                    //If the deserialisation fails, create an empty package
                    package = new DataPackage();
                }

                try
                {
                    if (package != null)
                        DataObservers.Invoke(package);
                }
                catch { }
            }

        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 8888;
        private string m_Address = "";
        private TcpClient m_Client = null;
    }
}
