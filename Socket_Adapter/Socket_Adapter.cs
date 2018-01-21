using BH.Adapter.Socket.Tcp;
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
