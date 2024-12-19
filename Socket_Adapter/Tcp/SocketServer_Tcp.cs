/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2025, the respective contributors. All rights reserved.
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

        public bool Start(int port = 8888, bool local = true)
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
                m_Listener = new TcpListener(local ? IPAddress.Loopback : IPAddress.Any, m_Port);
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
            List<TcpClient> failingClients = new List<TcpClient>();
            foreach (TcpClient client in m_Clients)
            {
                try
                {
                    if (source != client)
                        SendToClient(client, data);
                }
                catch (Exception)
                {
                    failingClients.Add(client);
                }

            }
            foreach (TcpClient client in failingClients)
            {
                m_Clients.Remove(client);
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






