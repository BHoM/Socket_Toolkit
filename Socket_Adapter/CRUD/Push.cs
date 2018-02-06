using System.Collections.Generic;
using BH.oM.Base;
using System.Linq;
using BH.Adapter.Socket.Tcp;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        public override List<IObject> Push(IEnumerable<IObject> objects, string tag = "", Dictionary<string, object> config = null)
        {
            if (m_Link.SendData(objects.ToList<object>(), tag))
                return objects.ToList();
            else
                return new List<IObject>();
        }
    }
}
