using System.Collections.Generic;

public enum Condition
{
    LessThan = 0,
	GreaterThan,
	EqualTo,
    Approximately,
};

public enum Sensor
{
    None = 0,
	Temperature,
	Light,
	Pressure,
	Camera
};

public enum Action
{
    None = 0,
    Indication = 1,
    Facebook = 2,
    Activation = 4,
    Notification = 8,
};

namespace AutomationManager.Entities
{
	public class AutomationTask
	{
        public string Id;
        public Device Device;
		public List<UserMachine> Machines;

        public Sensor DeviceSensor;
        public Condition TaskCondition;
        public object Value;
        public object Approximation;

        public Action ActionType;
        public object ActionParameter;

        public AutomationTask(Device parent)
		{
            Id = parent.NewTaskId();
            Device = parent;

			Machines = new List<UserMachine>();
		}
	}
}
