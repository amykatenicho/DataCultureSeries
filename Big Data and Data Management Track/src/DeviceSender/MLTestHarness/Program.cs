using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using DeviceSender;

namespace MLTestHarness
{

    internal class Program
    {
        private static void Main(string[] args)
        {
            var config = new BlobStorageConnectionDetails("mynewstorageaccnt",
                "R3FFPLqamAfvzpHjx3mJDpC5EHRxzKtTpCiOKgyIjYAd+L59QWHlLRnYlZXcha7pHJ7O/01G5MrpvDcydmfaHw==",
                "mycontainer", "my/root/folder/", "my/archive/folder/");
            var eventHubConfig = new EventHubConnectionDetails("myneweventhub-ns", "myneweventhub",
                "RootManageSharedAccessKey", "52vBalJmNMvgITAvpffQ+6NGFTsyoMNUWlFekLJSxNw=");

             var deviceDetails = new DeviceSendingDetails()
            {
                FailureConditions = new[]
                {
                    new FailedDeviceSettings(3, 0.2F, SensorTypes.Energy),
                    new FailedDeviceSettings(8, 0.3F, SensorTypes.Energy),
                    new FailedDeviceSettings(16, 0.05F, SensorTypes.Energy),
                    new FailedDeviceSettings(19, 0.07F, SensorTypes.Energy),
                    new FailedDeviceSettings(22, 0.25F, SensorTypes.Energy),
                    new FailedDeviceSettings(2, 0.2F, SensorTypes.Light),
                    new FailedDeviceSettings(4, 0.2F, SensorTypes.Light),
                    new FailedDeviceSettings(7, 0.3F, SensorTypes.Light),
                    new FailedDeviceSettings(18, 0.05F, SensorTypes.Light),
                    new FailedDeviceSettings(5, 0.1F, SensorTypes.Humidity),
                    new FailedDeviceSettings(6, 0.2F, SensorTypes.Humidity),
                    new FailedDeviceSettings(8, 0.3F, SensorTypes.Humidity),
                    new FailedDeviceSettings(19, 0.05F, SensorTypes.Humidity),
                    new FailedDeviceSettings(20, 0.07F, SensorTypes.Humidity),
                    new FailedDeviceSettings(3, 0.1F, SensorTypes.Temperature),
                    new FailedDeviceSettings(6, 0.2F, SensorTypes.Temperature),
                    new FailedDeviceSettings(9, 0.3F, SensorTypes.Temperature),
                    new FailedDeviceSettings(12, 0.05F, SensorTypes.Temperature),
                    new FailedDeviceSettings(15, 0.07F, SensorTypes.Temperature),
                    new FailedDeviceSettings(18, 0.15F, SensorTypes.Temperature),
                    new FailedDeviceSettings(21, 0.25F, SensorTypes.Temperature),    
                },
                NumberOfRooms = 10,
                IterationSeconds = 30,
                NumberOfDevices = 50,
                NumberOfDeviceTypes = 4,
                TemperatureMax = 28.9F,
                TemperatureMin = 19.6F,
                HumidityMin = 40.0F,
                HumidityMax = 80.0F,
                EnergyMin = 2000.0F,
                EnergyMax = 4900.0F,
                LightMin = 10.0F,
                LightMax = 1800.0F,
                MillisecondDelay = 1000,
                RedirectToLocalFile = true,
                RedirectFileName = "c:\\\\tmp\\messages.txt"
            };
            var manager = new BlobStorageManager(config);
            var lines = manager.GetLines("messages.txt");

            var sender = new MessageSender(eventHubConfig, deviceDetails);
            sender.SendMessages(lines);
        }

    }
}
