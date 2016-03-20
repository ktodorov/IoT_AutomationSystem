﻿using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ElectronicsSystemInterface.Modules
{
	public class BlinkyLed
	{
		private const int LED_PIN = 5;
		private GpioPin pin;
		private GpioPinValue pinValue;
		private DispatcherTimer timer;
		private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
		private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

		public BlinkyLed()
		{
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(5000);
			timer.Tick += Timer_Tick;
		}

		public void Init()
		{
			InitGPIO();
			if (pin != null)
			{
				//timer.Start();
			}
		}

		private void InitGPIO()
		{
			var gpio = GpioController.GetDefault();

			// Show an error if there is no GPIO controller
			if (gpio == null)
			{
				pin = null;
				//GpioStatus.Text = "There is no GPIO controller on this device.";
				return;
			}

			pin = gpio.OpenPin(LED_PIN);
			pinValue = GpioPinValue.High;
			pin.Write(pinValue);
			pin.SetDriveMode(GpioPinDriveMode.Output);

			//GpioStatus.Text = "GPIO pin initialized correctly.";
		}

		private void Timer_Tick(object sender, object e)
		{
			if (pinValue == GpioPinValue.High)
			{
				pinValue = GpioPinValue.Low;
				pin.Write(pinValue);
				//LED.Fill = redBrush;
			}
			else
			{
				pinValue = GpioPinValue.High;
				pin.Write(pinValue);
				//LED.Fill = grayBrush;
			}
		}

        public void On()
        {
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
        }

        public void Off()
        {
            pinValue = GpioPinValue.Low;
            pin.Write(pinValue);
        }

		public bool LedIsOn()
		{
			return pinValue == GpioPinValue.High;
		}
	}
}
