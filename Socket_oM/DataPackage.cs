using BH.oM.Base;
using System.Collections.Generic;

namespace BH.oM.Socket
{
    public class DataPackage : IObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<object> Data { get; set; } = new List<object>();

        public string Tag { get; set; } = "";


        /***************************************************/
    }
}
