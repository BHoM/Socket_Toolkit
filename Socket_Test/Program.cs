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

using BH.Adapter.Socket;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Socket;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Socket_Test
{
    class A
    {

        public A(double gVal = 0, double pVal = 0)
        {
            getProp = gVal;
            privateField = pVal;
        }

        public double a { get; set; }
        public double publicField;
        private double privateField;
        public double getProp { get; private set; }
    }
    class B : A { public double b { get; set; } }
    class C : A { public double c { get; set; } }
    class D : B { public double d { get; set; } }
    class E : C { public double e { get; set; } }


    class Program
    {
        static void Main(string[] args)
        {
            TestTcpSocket();

            Console.ReadLine();
        }


        static void TestTcpSocket()
        {
            List<object> items = new List<object>
            {
                new A (-6, -7) { a = 1, publicField = -4 },
                new B { a = 2, b = 45 },
                new C { a = 3, c = 56 },
                new D { a = 4, b = 67, d = 123 },
                new E { a = 5, c = 78, e = 456 },
                new Dictionary<string, A> {
                    { "A",  new A { a = 1 } },
                    { "C",  new C { a = 3, c = 56 } },
                    { "E",  new E { a = 5, c = 78, e = 456 } }
                }
            };

            int port = 8888;
            int nbLinks = 3;
            List<SocketLink_Tcp> links = new List<SocketLink_Tcp>();
            for (int i = 0; i < nbLinks; i++)
            {
                Thread.Sleep(200);
                SocketLink_Tcp link = new SocketLink_Tcp(port);
                link.DataObservers += ReceivedData;
                links.Add(link);
            }

            Console.WriteLine("Sending data...");
            int nbSenders = 3;
            for (int i = 0; i < nbSenders; i++)
            {
                Thread.Sleep(200);
                links[i].SendData(items, i.ToString());
            }
                

            /*Thread.Sleep(1000);

            for (int i = 0; i < 10000; i++)
                items.Add(new Node { Position = new Point { X = 1, Y = 2, Z = 3 }, Name = ("X" + i) });
            Console.WriteLine("Sending data...");
            client.SendData(items);

            while(true)
            {
                string userMessage = Console.ReadLine();
                if (userMessage.Length == 0)
                    break;
                else
                    client.SendData(new List<object> { userMessage });
            }*/
        }

        static void ReceivedData(DataPackage package)
        {
            Console.WriteLine("Link received " + package.Data.Count + " objects with the tag " + package.Tag);
        }
    }
}
