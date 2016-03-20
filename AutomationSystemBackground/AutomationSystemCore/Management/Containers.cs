using System.Collections.Generic;

namespace AutomationSystemCore.Management
{
    public struct DeviceDescriptor
    {
        public DeviceDescriptor(string id, List<Sensor> modules)
        {
            DeviceId = id;
            Modules = modules;
        }

        public string DeviceId;
        public List<Sensor> Modules;
    }

    public struct ConditionDescriptor
    {
        public Sensor SensorType;
        public Condition ComparisonType;
        public double Target;
    }

    public struct ActionDescriptor
    {
        public Action Type;
        public string Parameter;
    }

    public struct TaskDescriptor
    {
        public string DeviceId;
        public ConditionDescriptor Condition;
        public ActionDescriptor Action;
    }

    public struct TypedMessage
    {
        public string Type;
        public object Data;
    }
}