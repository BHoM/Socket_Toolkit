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
    public class StateObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public SS.Socket Handler { get; set; } = null;                    // Client  socket.

        public byte[] Buffer { get; set; } = null;                        // Receive buffer.

        public MessageEvent Callback { get; set; } = null;                // Callback method

        public int TotalBytesRead { get; set; } = 0;                      // Total number of bytes read so far

        public int Port { get; set; } = 0;                                // port used by socket

        /***************************************************/
    }
}




