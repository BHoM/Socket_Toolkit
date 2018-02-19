using System.Collections.Generic;

namespace BH.oM.Socket
{
    public class DataPackage
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<object> Data { get; set; } = new List<object>();

        public string Tag { get; set; } = "";


        /***************************************************/
    }
}
