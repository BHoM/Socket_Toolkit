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
using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public SocketAdapter(string address = "127.0.0.1", int port = 8888)
        {
            m_Link = new SocketLink_Tcp(port, address);
            m_Link.DataObservers += M_Link_DataObservers;
        }


        /***************************************************/
        /**** Private Methods                           ****/
        /***************************************************/

        private void M_Link_DataObservers(DataPackage package)
        {
            m_LastPackages[package.Tag] = package.Data;
            OnDataUpdated();
        }


        /***************************************************/
        /**** Private Fields                            ****/
        /***************************************************/

        private SocketLink_Tcp m_Link = null;
        private Dictionary<string, List<object>> m_LastPackages = new Dictionary<string, List<object>>();


        /***************************************************/
    }
}
