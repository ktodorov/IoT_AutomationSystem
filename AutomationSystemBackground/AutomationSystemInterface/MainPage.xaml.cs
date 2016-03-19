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

        public void MakePage(string value)
        {

            //if(value=="")
                mainPanel.Children.Add(AddTextBlock(value));
            
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
            List<string> dev1Sens = new List<string>();
            dev1Sens.Add("temperature");
            dev1Sens.Add("light");
            dev1Sens.Add("camera");
            
            Device dev1 = new Device("raspberry1", dev1Sens);
            devList.Add(dev1);
            List<string> dev2Sens = new List<string>();
            dev2Sens.Add("presure");
            dev2Sens.Add("microphone");
            dev2Sens.Add("temperature");
            dev2Sens.Add("light");
            dev2Sens.Add("camera");
            Device dev2 = new Device("raspberry2", dev2Sens);
            devList.Add(dev2);
            mainPanel.Children.Add(AddButton(dev1.DeviceName));
            mainPanel.Children.Add(AddSensorsTextBlock(dev1.Sensors));
            mainPanel.Children.Add(AddButton(dev2.DeviceName));
            mainPanel.Children.Add(AddSensorsTextBlock(dev2.Sensors));
           
 
        }


        private void SomeButton_Click(object sender, RoutedEventArgs e)
        {
            var btnSender = (Button)sender;
            foreach (var dev in devList)
            {
                if (btnSender.Content.ToString() == dev.DeviceName)
                {
                    mainPanel.Children.Clear();
                    foreach (var elem in dev.Sensors)
                    {
                        mainPanel.Children.Add(AddButton(elem));
                        //send dev.DeviceName to the server
                    }
                }
                else
                {
                    foreach (var elem in dev.Sensors)
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
