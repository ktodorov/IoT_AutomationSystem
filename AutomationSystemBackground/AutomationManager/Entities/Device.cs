using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationManager.Entities
{
	public class Device
	{
		public string Id;

		public string Name;

		public List<AutomationTask> Tasks;

		public List<Sensor> Modules;

		public Device()
		{
			Tasks = new List<AutomationTask>();
		}
	}
}
