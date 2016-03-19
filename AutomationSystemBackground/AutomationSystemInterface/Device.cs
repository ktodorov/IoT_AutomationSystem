using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationSystemInterface
{
    public class Device
    {
        private string deviceName;
        private List<string> sensors;

        public string DeviceName
        {
            get
            {
                return deviceName;
            }
            set
            {
                if (value != null)
                {
                    deviceName = value;
                }
                else
                    deviceName = "raspberry";
            }
        }

        public List<string> Sensors
        {
            get
            {
                return sensors;
            }
            set
            {
                if(value!=null)
                {
                    sensors = value;
                }
            }
        }

        public Device(string devName, List<string> devList)
        {
            DeviceName = devName;
            Sensors = devList;
        }

        public Device() : this("", null) { }

        public Device(Device dev) : this(dev.DeviceName, dev.Sensors) { }

    }
}
