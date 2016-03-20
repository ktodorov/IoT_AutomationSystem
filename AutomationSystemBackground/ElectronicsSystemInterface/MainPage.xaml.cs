using ElectronicsSystemInterface.Modules;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

		public MainPage()
		{
			this.InitializeComponent();

			blinkyLed.Init();
		}

		private async void Page_Loaded(object sender, RoutedEventArgs e)
		{
			string connectionString = "HostName=AutomationSystemHub.azure-devices.net;DeviceId=test;SharedAccessKey=yGDXie6keh70o03QWjC8+I/fAmIEKgkxAszERplHPyQ=";

			deviceClientSend = DeviceClient.CreateFromConnectionString(connectionString);
			//deviceClientRecevie = DeviceClient.CreateFromConnectionString(connectionString);

			await SendMessage();
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
                    sendTime = DateTime.Now,
                    LED = blinkyLed.LedIsOn()
                };
                var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));

				await deviceClientSend.SendEventAsync(message);
				Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
                //SendMessages.Items.Add(messageString);

                await Task.Delay(1000);
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
