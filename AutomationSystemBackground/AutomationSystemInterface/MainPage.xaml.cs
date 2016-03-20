using AutomationSystemCore.Entities;
using AutomationSystemCore.Management;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;



// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AutomationSystemInterface
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public TextBlock AddTextBlock(string value)
		{
			TextBlock txtVal = new TextBlock();
			txtVal.HorizontalAlignment = HorizontalAlignment.Center;
			txtVal.TextWrapping = TextWrapping.Wrap;
			txtVal.Width = 300;
			txtVal.TextAlignment = TextAlignment.Center;
			txtVal.FontSize = 30;
			txtVal.Margin = new Thickness(0, 0, 0, 10);
			txtVal.Text = value;
			return txtVal;
		}

		public TextBlock AddSensorsTextBlock(List<string> dataSensors)
		{
			TextBlock txtDataSens = new TextBlock();
			txtDataSens.HorizontalAlignment = HorizontalAlignment.Center;
			txtDataSens.TextWrapping = TextWrapping.Wrap;
			txtDataSens.Width = 300;
			txtDataSens.Margin = new Thickness(0, 0, 0, 10);
			string tempResult = "";
			foreach (var sensor in dataSensors)
			{
				tempResult += sensor + ", ";
			}
			tempResult = tempResult.Remove(tempResult.Length - 2);
			txtDataSens.Text = tempResult;
			return txtDataSens;
		}

		public Button AddButton(string dataName)
		{
			Button btn = new Button();
			btn.Name = "btn" + dataName;
			btn.Content = dataName;
			btn.Background = new SolidColorBrush(Colors.Snow);
			btn.Margin = new Thickness(0, 10, 0, 0);
			btn.Width = 300;
			btn.HorizontalAlignment = HorizontalAlignment.Center;
			btn.Click += SomeButton_Click;
			return btn;
		}

		public void MakePage(string value)
		{
			mainPanel.Children.Add(AddTextBlock(value));

			if (value == "Light")
			{
				var slider = new Slider();
				slider.Minimum = 10;
				slider.Maximum = 60;
				slider.Name = "lightSlider";

				var button = new Button();
				button.Content = "Submit";
				button.HorizontalAlignment = HorizontalAlignment.Center;
				button.Click += Button_Click;

				mainPanel.Children.Add(slider);
				mainPanel.Children.Add(button);
			}
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			var slider = mainPanel.FindName("lightSlider") as Slider;
			if (slider != null)
			{
				var value = slider.Value;

				await SendMessage(Action.Indication, Sensor.Clock, value);
			}
		}

		List<Device> devList = new List<Device>();
		public MainPage()
		{
			this.InitializeComponent();

			mainPanel.Background = new SolidColorBrush(Colors.BurlyWood);
			TextBlock txtb = new TextBlock();
			txtb.Text = "Choose one:";
			txtb.FontSize = 20;
			txtb.Margin = new Thickness(0, 20, 0, 20);
			txtb.HorizontalAlignment = HorizontalAlignment.Center;
			mainPanel.Children.Add(txtb);
			List<Sensor> dev1Sens = new List<Sensor>();
			dev1Sens.Add(Sensor.Temperature);
			dev1Sens.Add(Sensor.Light);
			dev1Sens.Add(Sensor.Pressure);
			Device dev1 = new Device("raspberry1", dev1Sens);
			devList.Add(dev1);
			//List<Sensor> dev2Sens = new List<Sensor>();
			//dev2Sens.Add(Sensor.Pressure);
			//dev2Sens.Add(Sensor.Camera);
			//dev2Sens.Add(Sensor.Light);
			//dev2Sens.Add(Sensor.Temperature);
			//Device dev2 = new Device("raspberry2", dev2Sens);
			//devList.Add(dev2);
			mainPanel.Children.Add(AddButton(dev1.Id));
			mainPanel.Children.Add(AddSensorsTextBlock(dev1.ModuleNames));
			//mainPanel.Children.Add(AddButton(dev2.Id));
			//mainPanel.Children.Add(AddSensorsTextBlock(dev2.ModuleNames));
		}


		private void SomeButton_Click(object sender, RoutedEventArgs e)
		{
			var btnSender = (Button)sender;
			foreach (var dev in devList)
			{
				if (btnSender.Content.ToString() == dev.Id)
				{
					mainPanel.Children.Clear();
					foreach (var elem in dev.ModuleNames)
					{
						mainPanel.Children.Add(AddButton(elem));
						//send dev.DeviceName to the server
					}
				}
				else
				{
					foreach (var elem in dev.ModuleNames)
					{
						if (btnSender.Content.ToString() == elem)
						{
							mainPanel.Children.Clear();
							//send elem to the server
							MakePage(elem);
						}
					}
				}
			}
		}

		public async Task SendMessage(Action action, Sensor sensor, double value)
		{
			var typedMessage = new TypedMessage();

			var taskDescriptor = new TaskDescriptor();
			var actionDescriptor = new ActionDescriptor();
			actionDescriptor.Parameter = "true";
			actionDescriptor.Type = Action.Indication;

			taskDescriptor.Action = actionDescriptor;

			var conditionDescriptor = new ConditionDescriptor();
			conditionDescriptor.ComparisonType = Condition.GreaterThan;
			conditionDescriptor.SensorType = sensor;
			conditionDescriptor.Target = value;
			taskDescriptor.Condition = conditionDescriptor;

			taskDescriptor.DeviceId = "raspberry1";

			typedMessage.Data = taskDescriptor;
			typedMessage.Type = AutomationSystemCore.Management.MessageHeaders.CreateTask;

			string connectionString = "HostName=AutomationSystemHub.azure-devices.net;DeviceId=test;SharedAccessKey=O/B4sln1uG0KmnBeSeEZoxbVODL27q5coJmd8G/z2s4=";

			var deviceClientSend = DeviceClient.CreateFromConnectionString(connectionString);

			var messageString = JsonConvert.SerializeObject(typedMessage);
			var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

			await deviceClientSend.SendEventAsync(message);
			Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, messageString);
		}
	}
}
