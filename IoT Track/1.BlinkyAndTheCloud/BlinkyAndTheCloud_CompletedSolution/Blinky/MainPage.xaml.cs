// Copyright (c) Microsoft. All rights reserved.

using System;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace Blinky
{
    public sealed partial class MainPage : Page
    {
        private const int LED_PIN = 5;
        private GpioPin pin;
        private GpioPinValue pinValue;
        private DispatcherTimer timer;

        //Brush colours depending on the LED used
        private SolidColorBrush redBrush = new SolidColorBrush(Windows.UI.Colors.Red);
        private SolidColorBrush yellowBrush = new SolidColorBrush(Windows.UI.Colors.Yellow);
        private SolidColorBrush greenBrush = new SolidColorBrush(Windows.UI.Colors.Green);
        private SolidColorBrush blueBrush = new SolidColorBrush(Windows.UI.Colors.Blue);

        //Brush colour for the OFF light on the app
        private SolidColorBrush whiteBrush = new SolidColorBrush(Windows.UI.Colors.White);
        private SolidColorBrush grayBrush = new SolidColorBrush(Windows.UI.Colors.LightGray);

        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1000);
            timer.Tick += Timer_Tick;
            InitGPIO();
            if (pin != null)
            {
                timer.Start();
            }        
        }

        private void InitGPIO()
        {
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                pin = null;
                GpioStatus.Text = "There is no GPIO controller on this device.";
                return;
            }

            pin = gpio.OpenPin(LED_PIN);
            pinValue = GpioPinValue.High;
            pin.Write(pinValue);
            pin.SetDriveMode(GpioPinDriveMode.Output);

            GpioStatus.Text = "GPIO pin initialized correctly.";

        }


        private void Timer_Tick(object sender, object e)
        {
            if (pinValue == GpioPinValue.High)
            {
                pinValue = GpioPinValue.Low;
                pin.Write(pinValue);
                //Update brush colour depending on LED
                LED.Fill = blueBrush;
            }
            else
            {
                pinValue = GpioPinValue.High;
                pin.Write(pinValue);
                LED.Fill = whiteBrush;
            }
        }
             

    }
}
