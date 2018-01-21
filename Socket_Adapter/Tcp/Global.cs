using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Socket
{
    public static class Global
    {
        public static Dictionary<int, SocketServer_Tcp> TcpServers = new Dictionary<int, SocketServer_Tcp>();
    }
}
