﻿using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AutomationManager.Networking
{
	struct NetworkParameters
	{
		public string ConsumerGroupName;
		public string DeviceName;
		public string ConnectionString;
		public DateTime StartTime;
	}

	class NetworkReceiver
	{
		private EventHubClient eventHubClient = null;
		private EventHubReceiver eventHubReceiver = null;
		private int partitionsCount;
		private bool keepReceiving;

		public delegate void MessageFeedDelegate(string deviceId, DateTime time, string data);
		public MessageFeedDelegate MessageFeed;

		public async Task Setup(NetworkParameters parameters)
		{
			eventHubClient = EventHubClient.CreateFromConnectionString(parameters.ConnectionString, "messages/events");
			var runtimeinfo = eventHubClient.GetRuntimeInformation();
			partitionsCount = runtimeinfo.PartitionCount;

			string partition = EventHubPartitionKeyResolver.ResolveToPartition(parameters.DeviceName, partitionsCount);

			if (parameters.StartTime == null)
				parameters.StartTime = DateTime.Now;

			eventHubReceiver = eventHubClient.GetConsumerGroup(parameters.ConsumerGroupName).CreateReceiver(partition, parameters.StartTime);
			var pastEvents = await eventHubReceiver.ReceiveAsync(int.MaxValue, TimeSpan.FromSeconds(20));
		}

		public void StopRecieving()
		{
			keepReceiving = false;
		}

		private void Digest(EventData data)
		{
			string message = Encoding.UTF8.GetString(data.GetBytes());
			DateTime time = data.EnqueuedTimeUtc.ToLocalTime();
			string deviceId = data.SystemProperties["iothub-connection-device-id"].ToString();
			IDictionary<string, object> properties = data.Properties;

			if (MessageFeed != null)
				MessageFeed(deviceId, time, message);
		}

		public async Task Receive()
		{
			keepReceiving = true;
			while (keepReceiving)
			{
				var eventData = await eventHubReceiver.ReceiveAsync(TimeSpan.FromSeconds(1));
				if (eventData == null)
					continue;

				Digest(eventData);
			}
		}
	}

	class NetworkSender
	{
		private string connectionString;
		private string deviceName;

		public void Setup(NetworkParameters parameters)
		{
			connectionString = parameters.ConnectionString;
			deviceName = parameters.DeviceName;
		}

		public async void Broadcast(string message)
		{
			ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(connectionString);

			var serviceMessage = new Message(Encoding.ASCII.GetBytes(message));
			serviceMessage.Ack = DeliveryAcknowledgement.Full;
			serviceMessage.MessageId = Guid.NewGuid().ToString();
			await serviceClient.SendAsync(deviceName, serviceMessage);

			await serviceClient.CloseAsync();
		}
	}

}