using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace DeviceSender
{
    public class MessageGenerator
    {

        private readonly Random _rand = new Random();

        public MessageGenerator(DeviceSendingDetails deviceSendingDetails, EventHubConnectionDetails eventHubConnectionDetails)
        {
            DeviceSendingDetails = deviceSendingDetails;
            EventHubConnectionDetails = eventHubConnectionDetails;
        }

        private DeviceSendingDetails DeviceSendingDetails { get; set; }
        private EventHubConnectionDetails EventHubConnectionDetails { get; set; }
        private EventHubClient EventHubClient { get; set; }

        public void GenerateMessages()
        {
            if (DeviceSendingDetails.RedirectToLocalFile == false)
            {
                var connectionString =
                    String.Format(
                        "Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName={1};SharedAccessKey={2};TransportType=Amqp",
                        EventHubConnectionDetails.ServiceBusNamespace,
                        EventHubConnectionDetails.SasPolicyName,
                        EventHubConnectionDetails.SasPolicyKey);

                var factory = MessagingFactory.CreateFromConnectionString(connectionString);
                EventHubClient = factory.CreateEventHubClient(EventHubConnectionDetails.EventHubName);
            }

            if (DeviceSendingDetails.RedirectToLocalFile)
                Console.WriteLine("Redirecting messages to file {0}", DeviceSendingDetails.RedirectFileName);

            var startTime = DateTime.UtcNow;
            for (var i = 0; i < DeviceSendingDetails.IterationSeconds; i++)
            {
                SendDeviceEventStream();
                Console.WriteLine("Messages fired onto the eventhub!");
                
                Thread.Sleep(DeviceSendingDetails.MillisecondDelay);
            }

            var duration = DateTime.UtcNow - startTime;
            Console.WriteLine("Total Time (ms) : {0}", duration.TotalMilliseconds);

        }

        private void SendDeviceEventStream()
        {
           
            for (var roomNumber = 0; roomNumber < DeviceSendingDetails.NumberOfRooms; roomNumber++)
            {
                var allEvents = new List<EventData>();

                for (var deviceNumber = 0; deviceNumber < DeviceSendingDetails.NumberOfDevices; deviceNumber++)
                {
                    var tempStr = GenerateTemperatureEventData(deviceNumber, roomNumber);
                    var energyStr = GenerateEnergyData(deviceNumber, roomNumber);
                    var humidityStr = GenerateHumidityData(deviceNumber, roomNumber);
                    var lightStr = GenerateLightData(deviceNumber, roomNumber);

                    //write to Event Hub if not write to local
                    if (DeviceSendingDetails.RedirectToLocalFile)
                    {
                        RedrectToLocalFile(new List<FileEntry>()
                        {
                           new FileEntry()
                            {
                                PartitionKey = "temperature",
                                Message = tempStr,
                                MillisecondDelay = DeviceSendingDetails.MillisecondDelay
                            }, 
                            new FileEntry()
                            {
                                PartitionKey = "energy",
                                Message = energyStr,
                                MillisecondDelay = DeviceSendingDetails.MillisecondDelay
                            },
                            new FileEntry()
                            {
                                PartitionKey = "humidity",
                                Message = humidityStr,
                                MillisecondDelay = DeviceSendingDetails.MillisecondDelay
                            },
                            new FileEntry()
                            {
                                PartitionKey = "light",
                                Message = lightStr,
                                MillisecondDelay = DeviceSendingDetails.MillisecondDelay
                            }
                        }, DeviceSendingDetails.RedirectType);

                        continue;
                    }

                    allEvents.Add(new EventData(Encoding.UTF8.GetBytes(tempStr))
                    {
                        PartitionKey = "temperature"
                    });

                    allEvents.Add(new EventData(Encoding.UTF8.GetBytes(energyStr))
                    {
                        PartitionKey = "energy"
                    });

                    allEvents.Add(new EventData(Encoding.UTF8.GetBytes(humidityStr))
                    {
                        PartitionKey = "humidity"
                    });

                    allEvents.Add(new EventData(Encoding.UTF8.GetBytes(lightStr))
                    {
                        PartitionKey = "light"
                    });
                }

                if (EventHubClient == null || allEvents.Count <= 0 || DeviceSendingDetails.RedirectToLocalFile != false)
                    continue;

                var tempEvents = allEvents.Where(evnt => evnt.PartitionKey == "temperature").Select(evnt => evnt);
                var energyEvents = allEvents.Where(evnt => evnt.PartitionKey == "energy").Select(evnt => evnt);
                var humidityEvents = allEvents.Where(evnt => evnt.PartitionKey == "humidity").Select(evnt => evnt);
                var lightEvents = allEvents.Where(evnt => evnt.PartitionKey == "light").Select(evnt => evnt);
                EventHubClient.SendBatch(tempEvents);
                EventHubClient.SendBatch(energyEvents);
                EventHubClient.SendBatch(humidityEvents);
                EventHubClient.SendBatch(lightEvents);
            }

            
        }

        public void RedrectToLocalFile(IEnumerable<FileEntry> lines, FileType type)
        {
            var writer = new LocalFileWriter();

            if (type == FileType.Json)
            {
                var jsonStringList = lines.Select(JsonConvert.SerializeObject).ToList();
                if (!writer.WriteToFile(DeviceSendingDetails.RedirectFileName, jsonStringList))
                    throw new ApplicationException(String.Format("Error writing lines to file {0}", DeviceSendingDetails.RedirectFileName));

            }
            else
            {
                var csvStringList = new List<String>();
                foreach (var line in lines)
                {
                    var key = line.PartitionKey;

                    switch (key)
                    {
                        case "temperature":
                            var tempObj = JsonConvert.DeserializeObject<TemperatureData>(line.Message);
                            csvStringList.Add(String.Format("{0},{1},{2},{3},{4}", key, tempObj.Timestamp.ToString("o"), tempObj.Deviceid, tempObj.RoomNumber, tempObj.Temperature));
                            break;
                        case "energy":
                            var energyObj = JsonConvert.DeserializeObject<EnergyData>(line.Message);
                            csvStringList.Add(String.Format("{0},{1},{2},{3},{4}", key, energyObj.Timestamp.ToString("o"), energyObj.Deviceid, energyObj.RoomNumber, energyObj.Kwh));
                            break;
                        case "humidity":
                            var humidityObj = JsonConvert.DeserializeObject<HumidityData>(line.Message);
                            csvStringList.Add(String.Format("{0},{1},{2},{3},{4}", key, humidityObj.Timestamp.ToString("o"), humidityObj.Deviceid, humidityObj.RoomNumber, humidityObj.Humidity));
                            break;
                        case "light":
                            var lightObj = JsonConvert.DeserializeObject<LightData>(line.Message);
                            csvStringList.Add(String.Format("{0},{1},{2},{3},{4}", key, lightObj.Timestamp.ToString("o"), lightObj.Deviceid, lightObj.RoomNumber, lightObj.Lumens));
                            break;
                    }
                }

                if (!writer.WriteToFile(DeviceSendingDetails.RedirectFileName, csvStringList))
                    throw new ApplicationException(String.Format("Error writing lines to file {0}", DeviceSendingDetails.RedirectFileName));
            }
            
        }

        public String GenerateTemperatureEventData(int deviceNumber, int roomNumber)
        {
            var modifier = 1.0F;
            if (DeviceSendingDetails.FailureConditions.Any(
                    device => device.FailedDeviceId == deviceNumber && device.SensorType == SensorTypes.Energy))
            {
                var deviceDetails =
                    DeviceSendingDetails.FailureConditions.First(device => device.FailedDeviceId == deviceNumber);
                modifier += deviceDetails.FailedDeviceGradient;
            }

            var temperature = _rand.Next((int)((DeviceSendingDetails.TemperatureMin * modifier) * 100),
                    (int)((DeviceSendingDetails.TemperatureMax * modifier) * 100)) / 100F;

            var temperatureData = new TemperatureData()
            {
                Deviceid = "temperature" + deviceNumber,
                Temperature = (temperature),
                Timestamp = DateTime.UtcNow,
                RoomNumber = roomNumber
            };

            return JsonConvert.SerializeObject(temperatureData);
        }

        public String GenerateHumidityData(int deviceNumber, int roomNumber)
        {
            float modifier = 1.0F;
            if (DeviceSendingDetails.FailureConditions.Any(
                    device => device.FailedDeviceId == deviceNumber && device.SensorType == SensorTypes.Humidity))
            {
                var deviceDetails =
                    DeviceSendingDetails.FailureConditions.First(device => device.FailedDeviceId == deviceNumber);
                modifier += deviceDetails.FailedDeviceGradient;
            }

            var humidity = _rand.Next((int)((DeviceSendingDetails.HumidityMin * modifier) * 100),
                    (int)((DeviceSendingDetails.HumidityMax * modifier) * 100)) / 100;

            var humidityData = new HumidityData()
            {
                Deviceid = "humidity" + deviceNumber,
                Humidity = (humidity),
                Timestamp = DateTime.UtcNow,
                RoomNumber = roomNumber
            };

            return JsonConvert.SerializeObject(humidityData);
        }

        public String GenerateEnergyData(int deviceNumber, int roomNumber)
        {
            var modifier = 1.0F;
            if (DeviceSendingDetails.FailureConditions.Any(
                    device => device.FailedDeviceId == deviceNumber && device.SensorType == SensorTypes.Energy))
            {
                var deviceDetails =
                    DeviceSendingDetails.FailureConditions.First(device => device.FailedDeviceId == deviceNumber);
                modifier += deviceDetails.FailedDeviceGradient;
            }

            var energy = _rand.Next((int)((DeviceSendingDetails.EnergyMin * modifier) * 100),
                    (int)((DeviceSendingDetails.EnergyMax * modifier) * 100)) / 100 ;

            var energyData = new EnergyData()
            {
                Deviceid = "energy" + deviceNumber,
                Kwh = (energy),
                Timestamp = DateTime.UtcNow,
                RoomNumber = roomNumber
            };

            return JsonConvert.SerializeObject(energyData);
  
        }

        public String GenerateLightData(int deviceNumber, int roomNumber)
        {
            var modifier = 1.0F;
            if (DeviceSendingDetails.FailureConditions.Any(
                    device => device.FailedDeviceId == deviceNumber && device.SensorType == SensorTypes.Light))
            {
                var deviceDetails =
                    DeviceSendingDetails.FailureConditions.First(device => device.FailedDeviceId == deviceNumber);
                modifier += deviceDetails.FailedDeviceGradient;
            }

            var lumens = _rand.Next((int)((DeviceSendingDetails.LightMin * modifier) * 100),
                    (int)((DeviceSendingDetails.LightMax * modifier) * 100)) / 100;

            var lightData = new LightData()
            {
                Deviceid = "light" + deviceNumber,
                Lumens = (lumens),
                Timestamp = DateTime.UtcNow,
                RoomNumber = roomNumber
            };

            return JsonConvert.SerializeObject(lightData);
        }
    }

    public class FileEntry
    {
        public String PartitionKey;
        public String Message;
        public int MillisecondDelay;
    }

    public class Sensor 
    {
        public string Deviceid { get; set; }
        public DateTime Timestamp { get; set; }
        public int RoomNumber { get; set; }
    }

    public class TemperatureData : Sensor
    {
        public float Temperature { get; set; }
    }

    public class HumidityData : Sensor
    {
        public float Humidity { get; set; }
    }

    public class EnergyData : Sensor
    {
        public float Kwh { get; set; }
    }

    public class LightData : Sensor
    {
        public float Lumens { get; set; }
    }
}
