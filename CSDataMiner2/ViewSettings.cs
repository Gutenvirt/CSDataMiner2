using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSDataMiner2
{
    public class ViewSettings
    {
        public Corner UpperRight { get; set; }
        public Corner UpperLeft { get; set; }

        public ViewSettings (Corner UpperRightCorner, Corner UpperLeftCorner)
        {
            UpperRight = UpperRightCorner;
            UpperLeft = UpperLeftCorner; 
        }
    }
}
