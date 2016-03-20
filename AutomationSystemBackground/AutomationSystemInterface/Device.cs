using AutomationSystemCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationManager.Entities
{
    public class Device
    {
        private static uint NextTaskId = 0;
        public string Id;

        public List<AutomationTask> Tasks;
        public List<Sensor> Modules;

        public List<string> ModuleNames
        {
            get
            {
                var _moduleNames = new List<string>();
                foreach (var module in Modules)
                    _moduleNames.Add(module.ToString());

                return _moduleNames;
            }
        }

        public Device(string id)
        {
            Id = id;
            Tasks = new List<AutomationTask>();
        }

        public Device(string devId, List<Sensor> _modules)
        {
            Id = devId;
            Tasks = new List<AutomationTask>();
            Modules = _modules;
        }

        public string NewTaskId()
        {
            ++NextTaskId;
            uint count = NextTaskId;

            string result = "";
            while (count > 0)
            {
                result += (count % 26) + 'A';
                count /= 26;
            }

            return result;
        }
    }
}
