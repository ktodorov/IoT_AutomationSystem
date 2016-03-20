using System;
using System.Collections.Generic;
using AutomationSystemCore.Entities;
using AutomationSystemCore.Management;

namespace AutomationManager.Management
{
    class DeviceHolder
    {
        public static TimeSpan KeepAlive { get { return TimeSpan.FromMinutes(1); } }

        public DeviceHolder(string id)
        {
            Device = new Device(id);
            Present = true;
            Update();
        }

        public void Update()
        {
            LastPing = DateTime.Now;
        }

        public Device Device;
        public DateTime LastPing;
        public bool Present;
    }

    class AutomationManager
    {
        private List<DeviceHolder> holders;

        private AutomationManager()
        {
            holders = new List<DeviceHolder>();
        }

        private void Connect(string deviceId)
        {
            DeviceHolder holder = GetHolder(deviceId);
            if (holder != null)
            {
                Console.WriteLine("Reconnecting device {0}", deviceId);
                holder.Present = true;
            }
            else
            {
                Console.WriteLine("<<< Connect {0}", deviceId);
                holders.Add(new DeviceHolder(deviceId));
            }
        }

        private void Disconnect(string deviceId)
        {
            DeviceHolder holder = GetHolder(deviceId);
            if (holder == null)
            {
                Console.WriteLine("Disconnecting non-existing device {0}", deviceId);
                return;
            }

            Console.WriteLine(">>> Disonnect {0}", deviceId);
            holder.Present = false;
        }

        private void Ping(string deviceId)
        {
            DeviceHolder holder = GetHolder(deviceId);
            if (holder == null)
            {
                Console.WriteLine("Ping from non-existing device {0}", deviceId);
                return;
            }
        }

        private List<DeviceDescriptor> GetDevices()
        {
            return holders.ConvertAll( device => new DeviceDescriptor(device.Device.Id, device.Device.Modules) );
        }

        private List<AutomationTask> GetTasks(string deviceId)
        {
            DeviceHolder holder = GetHolder(deviceId);
            if (holder == null)
                return null;

            return holder.Device.Tasks;
        }

        private string CreateTask(TaskDescriptor description)
        {
            DeviceHolder holder = GetHolder(description.DeviceId);
            if (holder == null)
            {
                Console.WriteLine("Creating task for non-existing device {0}", description.DeviceId);
                return "";
            }

            AutomationTask task = new AutomationTask(holder.Device);
            if (task.Id == "")
            {
                Console.WriteLine("Couldn't create task for {0}", description.DeviceId);
                return "";
            }

            task.DeviceSensor = description.Condition.SensorType;
            task.TaskCondition = description.Condition.ComparisonType;
            task.Value = description.Condition.Target;

            task.ActionType = description.Action.Type;
            task.ActionParameter = description.Action.Parameter;

            Console.WriteLine("New task for {0}", holder.Device.Id);
            holder.Device.Tasks.Add(task);
            return task.Id;
        }

        private string DeleteTask(string taskId)
        {
            foreach (var holder in holders)
            {
                foreach (var task in holder.Device.Tasks)
                {
                    if (task.Id != taskId)
                        continue;

                    Console.WriteLine("Deleting task {0} for {1}", taskId, holder.Device.Id);
                    holder.Device.Tasks.Remove(task);
                    return holder.Device.Id;
                }
            }

            return "";
        }

        private void CompleteTask(string taskId)
        {
            DeviceHolder holder = GetTaskHolder(taskId);
            if (holder == null)
            {
                Console.WriteLine("Completing task for non-existing device");
                return;
            }

            if (!holder.Present)
                Console.WriteLine("Completing task for non-present device");

            AutomationTask thisTask = null;
            foreach (var task in holder.Device.Tasks)
            {
                if (task.Id != taskId)
                    continue;

                thisTask = task;
                break;
            }

            if (thisTask == null)
            {
                Console.WriteLine("Task wasn't found the second time!?");
                return;
            }
            
            Console.WriteLine("Task {0} for {1} completed!", taskId, holder.Device.Id);
            if ((thisTask.ActionType & Action.Indication) == Action.Indication)
            { /* Do Nothing... */ }

            if ((thisTask.ActionType & Action.Activation) == Action.Activation)
            {
                TaskCompletedMessage data = new TaskCompletedMessage
                {
                    TaskId = taskId,
                    TaskAction = "activation",
                    Parameter = (string)thisTask.ActionParameter,
                };
                Broadcast(MessageHeaders.TaskCompleted, data);
            }

            if ((thisTask.ActionType & Action.Facebook) == Action.Facebook)
            {
                Console.WriteLine("Celebrating on facebook (:");
            }

            if ((thisTask.ActionType & Action.Notification) == Action.Notification)
            {
                Console.WriteLine("Sending notification to origin device");
            }
            
            DeleteTask(taskId);
        }

        public void Manage()
        {
            List<DeviceHolder> toRemove = new List<DeviceHolder>(holders.Count);
            DateTime now = DateTime.Now;

            foreach (var holder in holders)
            {
                TimeSpan diff = now - holder.LastPing;
                if (diff > DeviceHolder.KeepAlive)
                {
                    Console.WriteLine("Device {0} timed out", holder.Device.Id);
                    Disconnect(holder.Device.Id);
                }
            }
        }

        public void Feed(string message)
        {
            object jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject(message);
            TypedMessage messageData = (TypedMessage)jsonData;
            
            switch (messageData.Type)
            {
                case MessageHeaders.Connect:
                    Connect((string)messageData.Data);
                    break;

                case MessageHeaders.Disconnect:
                    Disconnect((string)messageData.Data);
                    break;

                case MessageHeaders.Ping:
                    Ping((string)messageData.Data);
                    break;

                case MessageHeaders.GetDevices:
                    Broadcast(MessageHeaders.DevicesList, GetDevices());
                    break;

                case MessageHeaders.GetTasks:
                    Broadcast(MessageHeaders.TasksList, GetTasks((string)messageData.Data));
                    break;

                case MessageHeaders.CreateTask:
                    Broadcast(MessageHeaders.TaskCreated, CreateTask((TaskDescriptor)messageData.Data));
                    break;

                case MessageHeaders.DeleteTask:
                    Broadcast(MessageHeaders.TaskDeleted, DeleteTask((string)messageData.Data));
                    break;

                case MessageHeaders.CompleteTask:
                    CompleteTask((string)messageData.Data);
                    break;
            }
        }

        private void Broadcast(string messageHeader, object data)
        {
            TypedMessage messageData = new TypedMessage();
            messageData.Type = messageHeader;
            messageData.Data = data;
            string jsonData = Newtonsoft.Json.JsonConvert.SerializeObject(messageData);
            Console.WriteLine("Broadcasting: '{0}'", messageData);
        }

        private DeviceHolder GetTaskHolder(string taskId)
        {
            foreach (var holder in holders)
            {
                foreach (var task in holder.Device.Tasks)
                {
                    if (task.Id != taskId)
                        continue;

                    holder.Update();
                    return holder;
                }
            }

            return null;
        }

        private DeviceHolder GetHolder(string deviceId)
        {
            foreach (var holder in holders)
            {
                if (holder.Device.Id != deviceId)
                    continue;

                holder.Update();
                return holder;
            }

            return null;
        }
    }
}
