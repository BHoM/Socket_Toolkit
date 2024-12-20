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

using BH.oM.Adapter;
using BH.oM.Data.Requests;
using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        public override IEnumerable<object> Pull(IRequest request, oM.Adapter.PullType pullType = 0, ActionConfig config = null)
        {
            if (request is FilterRequest)
            {
                string tag = ((FilterRequest)request).Tag;
                if (m_LastPackages.ContainsKey(tag))
                    return m_LastPackages[tag];
                else
                    return new List<object>();
            }
            else
            {
                Engine.Base.Compute.RecordWarning("The socket adapter only supports FilterRequest with Tags.");
                return new List<object>();
            }
                
        }
    }
}






