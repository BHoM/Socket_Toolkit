using BH.oM.Base;
using System.Collections.Generic;
using BH.oM.Reflection.Debuging;

namespace BH.oM.Socket
{
    public class DataPackage : IObject
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<object> Data { get; set; } = new List<object>();

        public List<Event> Events { get; set; } = new List<Event>();

        public string Tag { get; set; } = "";


        /***************************************************/
    }
}
