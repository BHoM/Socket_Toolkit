/*
 * This file is part of the Buildings and Habitats object Model (BHoM)
 * Copyright (c) 2015 - 2024, the respective contributors. All rights reserved.
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
using System.Text;
using System.Net;
using SS = System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace BH.Adapter.Socket
{
    public class SocketServer_Udp
    {
        /***************************************************/
        /**** Events                                    ****/
        /***************************************************/

        public MessageEvent MessageReceived;


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketServer_Udp()
        {
            m_Socket = new SS.Socket(SS.AddressFamily.InterNetwork, SS.SocketType.Dgram, SS.ProtocolType.Udp);
        }


        /***************************************************/
        /**** Destructors                               ****/
        /***************************************************/

        ~SocketServer_Udp()
        {
            if (m_Socket != null && m_Socket.Connected)
                m_Socket.Disconnect(false);

            if (m_Thread != null && m_Thread.IsAlive)
                m_Thread.Abort();
        }


        /***************************************************/
        /**** Public Methods                            ****/
        /***************************************************/

        public bool Listen(int port = 8888, bool local = true)
        {
            /*if (m_Port == port || port == 0)
                return true;*/
            if (m_Port != 0 && port != 0)
                m_Socket.Disconnect(true);

            try
            {
                if (m_Port != port && port != 0)
                {
                    m_Socket.Bind(new IPEndPoint(local ? IPAddress.Loopback : IPAddress.Any, port));
                    m_Port = port;
                }
                    
                StateObject state = new StateObject();
                state.Callback = MessageReceived;
                state.Handler = m_Socket;
                state.Port = port;

                if (m_Thread != null && m_Thread.IsAlive)
                    m_Thread.Abort();

                m_Thread = new Thread(() => ReadMessage(state));
                m_Thread.Start();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Something went wrong inside the Listen method of the socket on port " + port);
                Debug.WriteLine(e);
                return false;
            }

            return true;
        }

        /***************************************************/

        public static string ReadString(StateObject state)
        {
            int messageLength = state.Buffer.Length;
            EndPoint remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            while (state.TotalBytesRead < messageLength)
            {
                int receivedDataLength = state.Handler.ReceiveFrom(state.Buffer, state.TotalBytesRead, messageLength - state.TotalBytesRead, 0, ref remote);
                state.TotalBytesRead += receivedDataLength;
            }

            return Encoding.ASCII.GetString(state.Buffer, 0, messageLength);
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private static void ReadMessage(StateObject state)
        {
            while (true)
            {
                try
                {
                    int bufferSize = ReadInt(state);
                    if (bufferSize > 0)
                    {
                        state.TotalBytesRead = 0;
                        state.Buffer = new byte[bufferSize];
                        state.Handler.ReceiveBufferSize = bufferSize;
                        string message = ReadString(state);

                        if (state.Callback != null)
                            state.Callback.Invoke(message);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Thread.Sleep(100);
            }
            
        }

        /***************************************************/

        private static int ReadInt(StateObject state)
        {
            byte[] buffer = new byte[sizeof(Int32)];
            EndPoint remote = (EndPoint)(new IPEndPoint(IPAddress.Any, 0));

            state.Handler.ReceiveFrom(buffer, buffer.Length, 0, ref remote);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            return BitConverter.ToInt32(buffer, 0);
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private int m_Port = 0;
        private SS.Socket m_Socket = null;
        private Thread m_Thread = null;

        /***************************************************/
    }
}




