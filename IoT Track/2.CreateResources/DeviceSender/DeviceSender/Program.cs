using System;
using System.Configuration;
using Microsoft.Data.Edm.Validation;
using System.Globalization;

namespace DeviceSender
{
    class Program
    {
        static void Main(string[] args)
        {
            var invokedVerb = "";
            object invokedVerbInstance = null;

            var options = new Options();
            if (!CommandLine.Parser.Default.ParseArguments(args, options,
              (verb, subOptions) =>
              {
                  // if parsing succeeds the verb name and correct instance
                  // will be passed to onVerbCommand delegate (string,object)
                  invokedVerb = verb;
                  invokedVerbInstance = subOptions;
              }))
            {
                Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
            }

            switch (invokedVerb)
            {
                case "GenerateDataToEventHub":
                    ProcessGenerateOptions((GenerateDataOptions)invokedVerbInstance);
                    Console.WriteLine("Finished sending all messages. Press any key to exit ...");
                    break;
                case "ReplayFileFromBlockBlob":
                    ProcessReplayOptions((ReplayOptions)invokedVerbInstance);
                    Console.WriteLine("Finished replaying file. Press any key to exit ...");
                    break;
                case "GenerateLocalFileUpload":          
                    ProcessGenerateLocalUploadOptions((GenerateLocalUploadOptions)invokedVerbInstance);
                    Console.WriteLine("Finished uploading locally generated file. Press any key to exit ...");
                    break;
                case "GenerateLocalFile":
                    ProcessGenerateLocalDataOptions((GenerateLocalDataOptions)invokedVerbInstance);
                    Console.WriteLine("Finished generating local data file. Press any key to exit ...");
                    break;
                case "UploadLocalFile":
                    ProcessUploadDataOptions((UploadDataOptions)invokedVerbInstance);
                    Console.WriteLine("Finished uploading file. Press any key to exit ...");
                    break;
                case "CreateConsumerGroup":
                    ProcessCreateEventHubConsumerGroupOptions((CreateEventHubConsumerGroupOptions)invokedVerbInstance);
                    Console.WriteLine("Finished creating consumer group(s). Press any key to exit ...");
                    break;
            }
            
            Console.Read();
        }

        private static void ProcessGenerateOptions(GenerateDataOptions options)
        {
            var details = new EventHubConnectionDetails(options.EventHubNamespace, options.EventHubName, options.EventHubSasPolicy, options.EventHubSasKey);

            var deviceDetails = GetDeviceDetails();

            var generator = new MessageGenerator(deviceDetails, details);
            generator.GenerateMessages();
        }

        private static void ProcessReplayOptions(ReplayOptions options)
        {
            var connection = new BlobStorageConnectionDetails(options.BlobStoragePolicyName,
                    options.BlobStoragePolicyKey, options.BlobStorageContainer, options.BlobStorageRootFolder,
                    options.BlobStorageArchiveFolder);
            var blobManager = new BlobStorageManager(connection);
            var lines = blobManager.GetLines(options.Filename);

            var deviceDetails = GetDeviceDetails();
            deviceDetails.RedirectToLocalFile = true;
            deviceDetails.RedirectFileName = options.Filename;
            

            var eventHubConfig = new EventHubConnectionDetails(options.EventHubNamespace, options.EventHubName, options.EventHubSasPolicy, options.EventHubSasKey);

            var sender = new MessageSender(eventHubConfig, deviceDetails);
            sender.SendMessages(lines);
        }

        private static void ProcessGenerateLocalUploadOptions(GenerateLocalUploadOptions options)
        {
            var deviceDetails = GetDeviceDetails();
            deviceDetails.RedirectToLocalFile = true;
            deviceDetails.RedirectFileName = options.Filename;
            deviceDetails.RedirectType = options.Type;

            var generator = new MessageGenerator(deviceDetails, null);
            generator.GenerateMessages();

            var connection = new BlobStorageConnectionDetails(options.BlobStoragePolicyName,
                    options.BlobStoragePolicyKey, options.BlobStorageContainer, options.BlobStorageRootFolder,
                    options.BlobStorageArchiveFolder);
            var blobManager = new BlobStorageManager(connection);
            blobManager.CreateFile(options.Filename);
        }

        private static void ProcessGenerateLocalDataOptions(GenerateLocalDataOptions options)
        {
            var deviceDetails = GetDeviceDetails();
            deviceDetails.RedirectToLocalFile = true;
            deviceDetails.RedirectFileName = options.Filename;
            deviceDetails.RedirectType = options.Type;

            var generator = new MessageGenerator(deviceDetails, null);
            generator.GenerateMessages();
        }

        private static void ProcessUploadDataOptions(UploadDataOptions options)
        {
            var connection = new BlobStorageConnectionDetails(options.BlobStoragePolicyName,
                options.BlobStoragePolicyKey, options.BlobStorageContainer, options.BlobStorageRootFolder,
                options.BlobStorageArchiveFolder);
            var blobManager = new BlobStorageManager(connection);
            blobManager.CreateFile(options.Filename);
        }

        private static void ProcessCreateEventHubConsumerGroupOptions(CreateEventHubConsumerGroupOptions options)
        {
            var eventHubConfig = new EventHubConnectionDetails(options.EventHubNamespace, options.EventHubName, options.EventHubSasPolicy, options.EventHubSasKey);
            var configurator = new EventHubConfigurator(eventHubConfig);

            if (options.NumberOfInstances == 1)
                configurator.AddConsumerGroup(options.ConsumerGroupName);
            else
            {
                for (var i = 0; i < options.NumberOfInstances; i++)
                {
                    configurator.AddConsumerGroup(options.ConsumerGroupName+i);
                }
            }
        }

        private static DeviceSendingDetails GetDeviceDetails()
        {
            return new DeviceSendingDetails()
            {
                FailureConditions = new[]
                {
                    new FailedDeviceSettings(3, 1.8F, SensorTypes.Energy),
                    new FailedDeviceSettings(8, 0.2F, SensorTypes.Energy),
                    new FailedDeviceSettings(16, 0.05F, SensorTypes.Energy),
                    new FailedDeviceSettings(19, 0.07F, SensorTypes.Energy),
                    new FailedDeviceSettings(22, 0.25F, SensorTypes.Energy),
                    new FailedDeviceSettings(2, 0.2F, SensorTypes.Light),
                    new FailedDeviceSettings(4, 0.5F, SensorTypes.Light),
                    new FailedDeviceSettings(7, 1.3F, SensorTypes.Light),
                    new FailedDeviceSettings(18, 0.05F, SensorTypes.Light),
                    new FailedDeviceSettings(5, 1.1F, SensorTypes.Humidity),
                    new FailedDeviceSettings(6, 0.6F, SensorTypes.Humidity),
                    new FailedDeviceSettings(8, 0.3F, SensorTypes.Humidity),
                    new FailedDeviceSettings(19, 0.05F, SensorTypes.Humidity),
                    new FailedDeviceSettings(20, 0.07F, SensorTypes.Humidity),
                    new FailedDeviceSettings(3, 7.3F, SensorTypes.Temperature),
                    new FailedDeviceSettings(6, 5.2F, SensorTypes.Temperature),
                    new FailedDeviceSettings(9, 0.9F, SensorTypes.Temperature),
                    new FailedDeviceSettings(12, 0.05F, SensorTypes.Temperature),
                    new FailedDeviceSettings(15, 0.07F, SensorTypes.Temperature),
                    new FailedDeviceSettings(18, 0.15F, SensorTypes.Temperature),
                    new FailedDeviceSettings(21, 0.25F, SensorTypes.Temperature),    
                },
                NumberOfRooms = int.Parse(ConfigurationManager.AppSettings["NumberOfRooms"]),
                IterationSeconds = int.Parse(ConfigurationManager.AppSettings["IterationSeconds"]),
                NumberOfDevices = int.Parse(ConfigurationManager.AppSettings["NumberOfDevices"]),
                NumberOfDeviceTypes = 4,
                TemperatureMax = float.Parse(ConfigurationManager.AppSettings["TemperatureMax"], CultureInfo.InvariantCulture),
                TemperatureMin = float.Parse(ConfigurationManager.AppSettings["TemperatureMin"], CultureInfo.InvariantCulture),
                HumidityMin = float.Parse(ConfigurationManager.AppSettings["HumidityMin"], CultureInfo.InvariantCulture),
                HumidityMax = float.Parse(ConfigurationManager.AppSettings["HumidityMax"], CultureInfo.InvariantCulture),
                EnergyMin = float.Parse(ConfigurationManager.AppSettings["EnergyMin"], CultureInfo.InvariantCulture),
                EnergyMax = float.Parse(ConfigurationManager.AppSettings["EnergyMax"], CultureInfo.InvariantCulture),
                LightMin = float.Parse(ConfigurationManager.AppSettings["LightMin"], CultureInfo.InvariantCulture),
                LightMax = float.Parse(ConfigurationManager.AppSettings["LightMax"], CultureInfo.InvariantCulture),
                MillisecondDelay = int.Parse(ConfigurationManager.AppSettings["MillisecondDelay"]),
                RedirectToLocalFile = false,
                RedirectFileName = "" 
            };
        }
    }
}
