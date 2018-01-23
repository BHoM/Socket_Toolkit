using BH.Adapter.Socket;
using BH.Adapter.Socket.Tcp;
using BH.Engine.Structure;
using BH.oM.Base;
using BH.oM.Geometry;
using BH.oM.Structural.Elements;
using BH.oM.Structural.Properties;
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
            SpeedTest();

            Console.ReadLine();
        }



        static void SpeedTest()
        {
            int port = 8400;
            SocketLink_Tcp link1 = new SocketLink_Tcp(port);
            SocketLink_Tcp link2 = new SocketLink_Tcp(port);

            List<object> list = new List<object>();
            for (int i = 0; i < 10; i++)
                list.Add(Create.FabricatedSteelBoxSection(i, i, i, i, i));

            Thread.Sleep(1000);
            link1.SendData(list);


            list = new List<object>();
            for (int i = 0; i < 5000; i++)
                list.Add(Create.Bar(Create.Node(new Point { X = 1, Y = 2, Z = 3 }), Create.Node(new Point { X = 1, Y = 2, Z = 3 })));

            Thread.Sleep(5000);
            link1.SendData(list);
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
                new Node {Position = new Point { X = 1, Y = 2, Z = 3 }, Name = "X"},
                new List<BHoMObject> {
                    new Node {Position  = new Point { X = 1, Y = 2, Z = 3 }, Name = "A"},
                    new Node {Position  = new Point { X = 4, Y = 5, Z = 6 }, Name = "B"},
                    new Node {Position  = new Point { X = 7, Y = 8, Z = 9 }, Name = "C"}
                },
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
