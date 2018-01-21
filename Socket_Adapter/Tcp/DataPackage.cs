using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BH.Adapter.Socket.Tcp
{
    public class DataPackage
    {
        /***************************************************/
        /**** Properties                                ****/
        /***************************************************/

        public List<object> Data { get; set; } = new List<object>();

        public string Tag { get; set; } = "";


        /***************************************************/
        /**** Constructors                              ****/
        /***************************************************/

        public DataPackage(List<object> data, string tag = "")
        {
            Data = data;
            Tag = tag;
        }

        /***************************************************/
    }
}
