using ElectronicsSystemInterface.Modules;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json.Linq;
using AutomationSystemCore.Management;
using AutomationSystemCore.Entities;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ElectronicsSystemInterface
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
		DeviceClient deviceClientSend;
		DeviceClient deviceClientRecevie;
		BlinkyLed blinkyLed = new BlinkyLed();
        List<AutomationTask> tasks;
        Device thisDevice;

        DateTime asdf;

		public MainPage()
		{
			this.InitializeComponent();

            tasks = new List<AutomationTask>();
            thisDevice = new Device("raspberry1");

            blinkyLed.Init();

			Loaded += MainPage_Loaded;
		}

		private async void MainPage_Loaded(object sender, RoutedEventArgs e)
		{
			string connectionString = "HostName=AutomationSystemHub.azure-devices.net;DeviceId=test;SharedAccessKey=O/B4sln1uG0KmnBeSeEZoxbVODL27q5coJmd8G/z2s4=";

			deviceClientSend = DeviceClient.CreateFromConnectionString(connectionString);
			deviceClientRecevie = DeviceClient.CreateFromConnectionString(connectionString);

            await ConnectMessage();
            await ReceiveMessage();
            await DisconnectMessage();
            //await SendMessage();
        }

        public async Task ConnectMessage()
        {
            AutomationSystemCore.Management.TypedMessage connectMsg = new AutomationSystemCore.Management.TypedMessage
            {
                Type = AutomationSystemCore.Management.MessageHeaders.Connect,
                Data = "raspberry1",
            };

            var messageString = JsonConvert.SerializeObject(connectMsg);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await deviceClientSend.SendEventAsync(message);
        }

        public async Task DisconnectMessage()
        {
            AutomationSystemCore.Management.TypedMessage connectMsg = new AutomationSystemCore.Management.TypedMessage
            {
                Type = AutomationSystemCore.Management.MessageHeaders.Disconnect,
                Data = "raspberry1",
            };

            var messageString = JsonConvert.SerializeObject(connectMsg);
            var message = new Message(Encoding.ASCII.GetBytes(messageString));

            await deviceClientSend.SendEventAsync(message);
        }

        int x = 0;
		public async Task SendMessage()
		{
            while (true)
            {
                x++;
                var telemetryDataPoint = new
                {
                    messageNo = x.ToString(),
                    sendTime = DateTime.Now.ToString("HH:mm:ss"),
                    LEDisOn = blinkyLed.LedIsOn()
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

				await deviceClientSend.SendEventAsync(message);
				Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                //SendMessages.Items.Add(messageString);

                await Task.Delay(5000);
            }
            
		}

        private async Task ReceiveMessage()
        {
            while (true)
            {
                ManageTasks();
                Message receivedMessage = await deviceClientRecevie.ReceiveAsync();
                if (receivedMessage == null) continue;

                string message = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Debug.WriteLine("<<< Got " + message);

                string jsonData = JsonConvert.DeserializeObject(message).ToString();
                JObject data = JObject.Parse(jsonData);
                var messageData = data.ToObject<TypedMessage>();

                DigestMessage(messageData);

                await deviceClientRecevie.CompleteAsync(receivedMessage);
            }
        }

        private void DoAction(AutomationTask task)
        {
            if (task.ActionType == Action.Indication)
            {
                if (task.ActionParameter == "true")
                    blinkyLed.Off();
                else
                    blinkyLed.On();
            }
        }

        private void ManageTasks()
        {
            bool finished = false;

            foreach (var task in tasks)
            {
                switch (task.DeviceSensor)
                {
                case Sensor.Clock:
                    DateTime today = DateTime.Now;
                    today.AddHours(-today.Hour);
                    today.AddMinutes(-today.Minute);
                    today.AddSeconds(-today.Second);
                    TimeSpan diff = DateTime.Now - asdf;
                    switch (task.TaskCondition)
                    {
                    case Condition.GreaterThan:
                        if (diff.Seconds > (double)task.Value)
                        {
                            DoAction(task);
                            finished = true;
                        }
                        break;
                    }
                    break;
                }
            }

            if (finished)
                tasks.Clear();
        }

        private void DigestMessage(TypedMessage msg)
        {
            switch (msg.Type)
            {
                case MessageHeaders.CreateTask:
                    TaskDescriptor desc = ((JObject)msg.Data).ToObject<TaskDescriptor>();
                    AutomationTask newTask = new AutomationTask(thisDevice)
                    {
                        Id = "0",

                        DeviceSensor = desc.Condition.SensorType,
                        TaskCondition = desc.Condition.ComparisonType,
                        Value = desc.Condition.Target,

                        ActionType = desc.Action.Type,
                        ActionParameter = desc.Action.Parameter,
                    };
                    asdf = DateTime.Now;
                    tasks.Add(newTask);
                    break;
            }
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
		{
			await SendMessage();
		}

		private async void ReciveButton_Click(object sender, RoutedEventArgs e)
		{
			//ReciveButton.IsEnabled = false;

			Debug.WriteLine("\nReceiving cloud to device messages from service");
			while (true)
			{
				Message receivedMessage = await deviceClientRecevie.ReceiveAsync();
				if (receivedMessage == null) continue;

				string message = string.Format("Received message: {0}", Encoding.ASCII.GetString(receivedMessage.GetBytes()));
				Debug.WriteLine(message);

				await deviceClientRecevie.CompleteAsync(receivedMessage);
				//ReciveMessages.Items.Add(message);
			}
		}
	}
}
