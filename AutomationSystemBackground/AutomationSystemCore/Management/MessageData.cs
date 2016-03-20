namespace AutomationSystemCore.Management
{
    public class MessageHeaders
    {
        public const string Connect = "connect";
        public const string Disconnect = "disconnect";
        public const string Ping = "ping";
        public const string GetDevices = "getDevices";
        public const string DevicesList = "devicesList";
        public const string GetTasks = "getTasks";
        public const string TasksList = "tasksList";
        public const string CreateTask = "createTask";
        public const string TaskCreated = "taskCreated";
        public const string DeleteTask = "deleteTask";
        public const string TaskDeleted = "taskDeleted";
        public const string CompleteTask = "completeTask";
        public const string TaskCompleted = "taskCompleted";
    }

    public struct TaskCompletedMessage
    {
        public string TaskId;
        public string TaskAction;
        public object Parameter;
    }
}