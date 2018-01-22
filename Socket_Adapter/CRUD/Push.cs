using System.Collections.Generic;
using BH.oM.Base;
using System.Linq;
using BH.Adapter.Socket.Tcp;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        public override bool Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            return m_Link.SendData(objects.ToList<object>(), tag);
        }
    }
}
