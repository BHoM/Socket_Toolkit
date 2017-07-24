//using MongoDB.Bson;
//using MongoDB.Bson.IO;
//using MongoDB.Bson.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace BH.Adapter.Socket
//{
//    public class SocketListener
//    {
//        public delegate void DataEvent(List<object> data);

//        /***************************************************/
//        /**** Properties                                ****/
//        /***************************************************/

//        public event DataEvent DataObservers;


//        /***************************************************/
//        /**** Constructors                              ****/
//        /***************************************************/

//        public SocketListener(int port = 8888)
//        {
//            m_Port = port;
//            m_Listener = new TcpListener(IPAddress.Any, m_Port);
//        }


//        /***************************************************/
//        /**** Public Methods                            ****/
//        /***************************************************/

//        public bool Start()
//        {
//            m_Listening = true;
//            m_Listener.Start();
//            m_Listener.BeginAcceptTcpClient(AcceptClient, m_Listener);
//            return true;
//        }

//        /***************************************************/

//        public void Stop()
//        {
//            m_Listening = false;
//            m_Listener.Stop();
//        }


//        /***************************************************/
//        /**** Private Methods                           ****/
//        /***************************************************/

//        private void AcceptClient(IAsyncResult ar)
//        {
//            TcpListener listener = (TcpListener)ar.AsyncState;
//            TcpClient client = listener.EndAcceptTcpClient(ar);
//            m_Clients.Add(client);
//        }

//        /***************************************************/

//        private void Listen()
//        {
//            while (m_Listening)
//            {
//                foreach (TcpClient client in m_Clients)
//                {
//                    if (client.Available > 0)
//                    {
//                        var reader = new BsonBinaryReader(client.GetStream());
//                        List<BsonDocument> readBson = BsonSerializer.Deserialize(reader, typeof(object)) as List<BsonDocument>;
//                        List<object> objects = readBson.Select(x => BsonSerializer.Deserialize(x, typeof(object))).ToList();
//                        if (DataObservers != null)
//                            DataObservers.Invoke(objects);
//                    }
//                }

//                Thread.Sleep(100);
//            }
            
//        }

//        /***************************************************/
//        /**** Private Fields                            ****/
//        /***************************************************/

//        private bool m_Listening = false;
//        private int m_Port = 8888;
//        private TcpListener m_Listener = null;
//        private List<TcpClient> m_Clients = new List<TcpClient>();
//    }
//}
