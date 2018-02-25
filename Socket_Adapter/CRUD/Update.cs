using BH.oM.DataManipulation.Queries;
using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter : BHoMAdapter
    {
        public override int UpdateProperty(FilterQuery query, string property, object value, Dictionary<string, object> config = null)
        {
            return 0;
        }
    }
}
