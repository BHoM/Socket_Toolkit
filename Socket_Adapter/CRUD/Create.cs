using System;
using System.Collections.Generic;

namespace BH.Adapter.Socket
{
    public partial class SocketAdapter
    {
        protected override bool Create<T>(IEnumerable<T> objects, bool replaceAll = false)
        {
            throw new NotImplementedException();
        }
    }
}
