using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siemens
{
    public  class OPC_Event
    {
        public bool isRun;
        public string RegAddress;
        public Value_Reset_Event value_Reset_Event;
        public Value_Set_Event value_Set_Event;

        public delegate void Value_Reset_Event(string RegAddress);

        public delegate void Value_Set_Event(string RegAddress);

        public OPC_Event(string RegAddress, Value_Set_Event value_Set_Event, Value_Reset_Event value_Reset_Event)
        {
            this.isRun = false;
            this.RegAddress = RegAddress;
            this.value_Set_Event = value_Set_Event;
            this.value_Reset_Event = value_Reset_Event;
        }

 


    }
}
