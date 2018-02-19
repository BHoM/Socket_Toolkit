using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public static class Global
    {
        public static Dictionary<int, SocketServer_Tcp> TcpServers = new Dictionary<int, SocketServer_Tcp>();
    }
}
