using System.Collections.Generic;

namespace AutomationManager.Management
{
    struct DeviceDescriptor
    {
        public DeviceDescriptor(string id, List<Sensor> modules)
        {
            DeviceId = id;
            Modules = modules;
        }

        public string DeviceId;
        public List<Sensor> Modules;
    }

    struct ConditionDescriptor
    {
        public Sensor SensorType;
        public Condition ComparisonType;
        public double Target;
    }

    struct ActionDescriptor
    {
        public Action Type;
        public string Parameter;
    }

    struct TaskDescriptor
    {
        public string DeviceId;
        public ConditionDescriptor Condition;
        public ActionDescriptor Action;
    }

    struct TypedMessage
    {
        public string Type;
        public object Data;
    }
}