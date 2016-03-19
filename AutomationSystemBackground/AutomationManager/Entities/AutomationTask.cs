using AutomationManager.Sensors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum Condition
{
	LessThan = 0,
	MoreThan = 1,
	EqualTo = 2
};

public enum Sensor
{
	Temperature = 0,
	Light = 1,
	Pressure = 2,
	Camera = 3
};

namespace AutomationManager.Entities
{
	public class AutomationTask
	{
		public Device IoTDevice;

		public List<UserMachine> Machines;

		public Condition TaskCondition;

		public Sensor DeviceSensor;

		public object Value;

		public AutomationTask()
		{
			Machines = new List<UserMachine>();
		}
	}
}
