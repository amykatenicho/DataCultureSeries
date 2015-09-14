using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSender
{
    public class DeviceSendingDetails
    {
        public bool RedirectToLocalFile { get; set; }
        public String RedirectFileName { get; set; }
        public FileType RedirectType { get; set; }
        public int NumberOfRooms { get; set; }
        public int NumberOfDevices { get; set; }
        public int NumberOfDeviceTypes { get; set; }
        public float TemperatureMin { get; set; }
        public float TemperatureMax { get; set; }
        public float HumidityMax { get; set; }
        public float HumidityMin { get; set; }
        public float EnergyMax { get; set; }
        public float EnergyMin { get; set; }
        public float LightMax { get; set; }
        public float LightMin { get; set; }
        public FailedDeviceSettings[] FailureConditions { get; set; }
        public int MessagesPerDevice { get; set; }
        public int IterationSeconds { get; set; }
        public int MillisecondDelay { get; set; }

    }

    public class FailedDeviceSettings
    {
        public FailedDeviceSettings(int deviceId, float deviceGradient, SensorTypes type)
        {
            SensorType = type;
            FailedDeviceId = deviceId;
            FailedDeviceGradient = deviceGradient;
        }

        public SensorTypes SensorType { get; set; }
        public int FailedDeviceId { get; set; }
        public float FailedDeviceGradient { get; set; }
    }

    public enum SensorTypes
    {
        Temperature,
        Energy,
        Light,
        Humidity
    }


}
