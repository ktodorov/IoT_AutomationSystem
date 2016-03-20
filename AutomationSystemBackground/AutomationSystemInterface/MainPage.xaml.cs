using AutomationSystemCore.Entities;
using AutomationSystemCore.Management;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
            txtVal.FontSize = 15;
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
            tempResult=tempResult.Remove(tempResult.Length - 2);
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
        

        private void SendTemperatureBtn_Click(object sender, RoutedEventArgs e)
        {
            //send information to server
            Slider temp= (Slider)mainPanel.FindName("temperatureScroll");
            if(temp!=null)
            {
                TaskDescriptor desc;
                desc.DeviceId = "raspberry1";
                desc.Condition.SensorType = Sensor.Temperature;
                desc.Condition.ComparisonType = Condition.GreaterThan;
                desc.Condition.Target = temp.Value;
                desc.Action.Type = Action.Notification;
            }
        }

        public void MakePage(string value)
        {
            mainPanel.Children.Add(AddTextBlock(value));
            if (value=="Temperature")
            {
                Slider scroll = new Slider();
                scroll.Name = "temperatureScroll";
                mainPanel.Children.Add(AddTextBlock("Select temperature:"));
                scroll.Maximum = 40;
                scroll.Minimum = 0;
                scroll.SmallChange = 1;
                scroll.LargeChange = 5;
                scroll.Margin = new Thickness(100, 0, 100, 0);
                mainPanel.Children.Add(scroll);


                Button btnSend = new Button();
                btnSend.Name="btnSendTemp";
                btnSend.Content = "Send Data";
                btnSend.Click += SendTemperatureBtn_Click;
                btnSend.Background = new SolidColorBrush(Colors.Snow);
                btnSend.Margin = new Thickness(0, 10, 0, 0);
                btnSend.Width = 300;
                btnSend.HorizontalAlignment = HorizontalAlignment.Center;
                mainPanel.Children.Add(btnSend);
            }
            else if (value=="Lamp")
            {
                Button btnOn = new Button();
                btnOn.Name = "btnLOn";
                btnOn.Content = "Light On";
                btnOn.Click += BtnOn_Click; 
                btnOn.Background = new SolidColorBrush(Colors.Snow);
                btnOn.Margin = new Thickness(0, 10, 0, 0);
                btnOn.Width = 300;
                btnOn.HorizontalAlignment = HorizontalAlignment.Center;
                mainPanel.Children.Add(btnOn);

                Button btnOff = new Button();
                btnOff.Name = "btnLOf";
                btnOff.Content = "Light Of";
                btnOff.Click += BtnOff_Click;
                btnOff.Background = new SolidColorBrush(Colors.Snow);
                btnOff.Margin = new Thickness(0, 10, 0, 0);
                btnOff.Width = 300;
                btnOff.HorizontalAlignment = HorizontalAlignment.Center;
                mainPanel.Children.Add(btnOff);
            }   
            
        }

        private void BtnOff_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BtnOn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        List<Device> devList = new List<Device>();
        public MainPage()
        {
            this.InitializeComponent();
            mainPanel.Orientation = Orientation.Vertical;
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
            List<Sensor> dev2Sens = new List<Sensor>();
            dev2Sens.Add(Sensor.Pressure);
            dev2Sens.Add(Sensor.Camera);
            dev2Sens.Add(Sensor.Light);
            dev2Sens.Add(Sensor.Temperature);
            Device dev2 = new Device("raspberry2", dev2Sens);
            devList.Add(dev2);
            mainPanel.Children.Add(AddButton(dev1.Id));
            mainPanel.Children.Add(AddSensorsTextBlock(dev1.ModuleNames));
            mainPanel.Children.Add(AddButton(dev2.Id));
            mainPanel.Children.Add(AddSensorsTextBlock(dev2.ModuleNames));
           
 
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
    }  
}
