using AutomationManager.Networking;
using Microsoft.Azure.Devices;
using Microsoft.ServiceBus.Messaging;
using Nito.AsyncEx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationManager
{
    class Program
    {
		static ServiceClient serviceClient;
		static string connectionString = "HostName=AutomationSystemHub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=oQnus5yhy5ybc10s8XidG1HOL+SQIRLTzNV7sY2Ba3Q=";
		static EventProcessorHost eventProcessorHost;
		public static string LastMessage { get; set; }

		static void Main(string[] args)
		{
			//Console.WriteLine("Send Cloud-to-Device message\n");
			//serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

			////Console.WriteLine("Press any key to send a C2D message.");
			////Console.ReadLine();

			string eventHubConnectionString = "Endpoint=sb://iothub-ns-automation-24228-c92f97de12.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=oQnus5yhy5ybc10s8XidG1HOL+SQIRLTzNV7sY2Ba3Q=";
			string eventHubName = "automationsystemhub";
			string storageAccountName = "automationhubstorage";
			string storageAccountKey = "F+xz9CkJvBVz1o/aYFAuARMe8XSBPVYPf3s70yl4FVGcR4DyP3X1lfgKfoeed19qshiZjPnaBeP0rJPSAp21Gw==";
			string storageConnectionString = string.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", storageAccountName, storageAccountKey);

			//Task.Run(() =>
			//{
			//	SendCloudToDeviceMessageAsync().Wait();
			//});

			//string eventProcessorHostName = Guid.NewGuid().ToString();
			//eventProcessorHost = new EventProcessorHost(eventProcessorHostName, eventHubName, EventHubConsumerGroup.DefaultGroupName, eventHubConnectionString, storageConnectionString);
			//Console.WriteLine("Registering EventProcessor...");
			//var options = new EventProcessorOptions();
			//options.ExceptionReceived += (sender, e) => { Console.WriteLine(e.Exception); };
			//eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(options).Wait();

			//Console.WriteLine("Receiving. Press enter key to stop worker.");
			//Console.ReadLine();
			//eventProcessorHost.UnregisterEventProcessorAsync().Wait();

			//Console.ReadLine();

			AsyncContext.Run(() => MainAsync(args));
		}

		static async void MainAsync(string[] args)
		{
			var parameters = new NetworkParameters();
			parameters.ConnectionString = connectionString;
			parameters.ConsumerGroupName = "$Default";
			parameters.DeviceName = "test";
			parameters.StartTime = DateTime.Now;

			var receiver = new NetworkReceiver();
			receiver.MessageFeed = Digest;
			await receiver.Setup(parameters);
			await receiver.Receive();
		}


		private static void Digest(string devId, DateTime time, string msg)
		{
			Console.WriteLine(msg);
		}
		
		private static async Task SendCloudToDeviceMessageAsync()
		{
			//Console.WriteLine("\nReceiving cloud to device messages from service");
			//while (true)
			//{
			//	string message = Program.LastMessage;
			//	Program.LastMessage = string.Empty;

			//	if (!string.IsNullOrEmpty(message))
			//	{
			//		try
			//		{

			//			var commandMessage = new Message(Encoding.ASCII.GetBytes("Cloud to device message." + message));
			//			//	await serviceClient.SendAsync("test", commandMessage);
			//		}
			//		catch (Exception e)
			//		{
			//			Console.WriteLine("[Exception] " + e.ToString());
			//		}
			//	}
			//}
		}
	}
}
