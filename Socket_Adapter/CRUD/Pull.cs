using BH.oM.Queries;
using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        public override IEnumerable<object> Pull(IQuery query, Dictionary<string, object> config = null)
        {
            if (query is FilterQuery)
            {
                string tag = ((FilterQuery)query).Tag;
                if (m_LastPackages.ContainsKey(tag))
                    return m_LastPackages[tag];
                else
                    return new List<object>();
            }

            return new List<object>();
        }
    }
}
