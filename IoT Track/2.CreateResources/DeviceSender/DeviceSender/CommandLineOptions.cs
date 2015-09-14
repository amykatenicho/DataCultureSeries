using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;

namespace DeviceSender
{
    public class Options
    {
        public Options()
        {
            // Since we create this instance the parser will not overwrite it
            GenerateDataVerb = new GenerateDataOptions();
            ReplayVerb = new ReplayOptions();
            GenerateLocalUploadVerb = new GenerateLocalUploadOptions();
            LocalVerb = new GenerateLocalDataOptions();
            UploadVerb = new UploadDataOptions();
            CreateVerb = new CreateEventHubConsumerGroupOptions();
        }

        [VerbOption("GenerateDataToEventHub", HelpText = "Generate data and upload to Event Hub")]
        public GenerateDataOptions GenerateDataVerb { get; set; }

        [VerbOption("ReplayFileFromBlockBlob", HelpText = "Replay a set of messages from Blob Storage by sending them to the Event Hub")]
        public ReplayOptions ReplayVerb { get; set; }

        [VerbOption("GenerateLocalFileUpload", HelpText = "Generate a dataset locally, upload to blob storage")]
        public GenerateLocalUploadOptions GenerateLocalUploadVerb { get; set; }

        [VerbOption("GenerateLocalFile", HelpText = "Generate a dataset locally")]
        public GenerateLocalDataOptions LocalVerb { get; set; }

        [VerbOption("UploadLocalFile", HelpText = "Upload a dataset to blob storage")]
        public UploadDataOptions UploadVerb { get; set; }

        [VerbOption("CreateConsumerGroup", HelpText = "Create an Event Hub consumer group")]
        public CreateEventHubConsumerGroupOptions CreateVerb { get; set; }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }
    }

    public class GenerateDataOptions
    {
        [Option('n', "EventHubNamespace", Required = true, HelpText = "The Event Hub namespace")]
        public string EventHubNamespace { get; set; }

        [Option('e', "EventHubName", Required = true, HelpText = "The Event Hub name")]
        public string EventHubName { get; set; }

        [Option('p', "EventHubSasPolicy", Required = true, HelpText = "The Event Hub policy name")]
        public string EventHubSasPolicy { get; set; }

        [Option('k', "EventHubSasKey", Required = true, HelpText = "The Event Hub key")]
        public string EventHubSasKey { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

    public class ReplayOptions
    {
        [Option('n', "EventHubNamespace", Required = true, HelpText = "The Event Hub namespace")]
        public string EventHubNamespace { get; set; }

        [Option('e', "EventHubName", Required = true, HelpText = "The Event Hub name")]
        public string EventHubName { get; set; }

        [Option('p', "EventHubSasPolicy", Required = true, HelpText = "The Event Hub policy name")]
        public string EventHubSasPolicy { get; set; }

        [Option('k', "EventHubSasKey", Required = true, HelpText = "The Event Hub key")]
        public string EventHubSasKey { get; set; }

        [Option('q', "BlobStoragePolicyName", Required = true, HelpText = "Blob Storage name")]
        public string BlobStoragePolicyName { get; set; }

        [Option('l', "BlobStoragePolicyKey", Required = true, HelpText = "Blob Storage key")]
        public string BlobStoragePolicyKey { get; set; }

        [Option('c', "BlobStorageContainer", Required = true, HelpText = "Blob Storage container (should exist)")]
        public string BlobStorageContainer { get; set; }

        [Option('r', "BlobStorageRootFolder", Required = true, HelpText = "Root directory in Blob Storage in the form \"some/root/folder/\"")]
        public string BlobStorageRootFolder { get; set; }

        [Option('a', "BlobStorageArchiveFolder", Required = true, HelpText = "Archive folder in Blob Storage for processed files \"some/archive/folder/\"")]
        public string BlobStorageArchiveFolder { get; set; }

        [Option('f', "Filename", Required = true, HelpText = "The name of the file to replay")]
        public string Filename { get; set; }


        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

    public class GenerateLocalDataOptions
    {
        [Option('f', "Filename", Required = true, HelpText = "The name of the file to create")]
        public string Filename { get; set; }

        [Option('t', "Type", DefaultValue = FileType.Json, Required = true, HelpText = "The type of file to generate")]
        public FileType Type { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

    public class UploadDataOptions
    {
        [Option('q', "BlobStoragePolicyName", Required = true, HelpText = "Blob Storage name")]
        public string BlobStoragePolicyName { get; set; }

        [Option('l', "BlobStoragePolicyKey", Required = true, HelpText = "Blob Storage key")]
        public string BlobStoragePolicyKey { get; set; }

        [Option('c', "BlobStorageContainer", Required = true, HelpText = "Blob Storage container (should exist)")]
        public string BlobStorageContainer { get; set; }

        [Option('r', "BlobStorageRootFolder", Required = true, HelpText = "Root directory in Blob Storage in the form \"some/root/folder/\"")]
        public string BlobStorageRootFolder { get; set; }

        [Option('a', "BlobStorageArchiveFolder", Required = true, HelpText = "Archive folder in Blob Storage for processed files \"some/archive/folder/\"")]
        public string BlobStorageArchiveFolder { get; set; }

        [Option('f', "Filename", Required = true, HelpText = "The name of the file to upload")]
        public string Filename { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

    public class GenerateLocalUploadOptions
    {
        [Option('q', "BlobStoragePolicyName", Required = true, HelpText = "Blob Storage name")]
        public string BlobStoragePolicyName { get; set; }

        [Option('l', "BlobStoragePolicyKey", Required = true, HelpText = "Blob Storage key")]
        public string BlobStoragePolicyKey { get; set; }

        [Option('c', "BlobStorageContainer", Required = true, HelpText = "Blob Storage container (should exist)")]
        public string BlobStorageContainer { get; set; }

        [Option('r', "BlobStorageRootFolder", Required = true, HelpText = "Root directory in Blob Storage in the form \"some/root/folder/\"")]
        public string BlobStorageRootFolder { get; set; }

        [Option('a', "BlobStorageArchiveFolder", Required = true, HelpText = "Archive folder in Blob Storage for processed files \"some/archive/folder/\"")]
        public string BlobStorageArchiveFolder { get; set; }

        [Option('f', "Filename", Required = true, HelpText = "Print details during execution.")]
        public string Filename { get; set; }

        [Option('t', "Type", DefaultValue = FileType.Json, Required = true, HelpText = "The type of file to generate")]
        public FileType Type { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }
    }

    public class CreateEventHubConsumerGroupOptions
    {
        [Option('n', "EventHubNamespace", Required = true, HelpText = "The Event Hub namespace")]
        public string EventHubNamespace { get; set; }

        [Option('e', "EventHubName", Required = true, HelpText = "The Event Hub name")]
        public string EventHubName { get; set; }

        [Option('p', "EventHubSasPolicy", Required = true, HelpText = "The Event Hub policy name")]
        public string EventHubSasPolicy { get; set; }

        [Option('k', "EventHubSasKey", Required = true, HelpText = "The Event Hub key")]
        public string EventHubSasKey { get; set; }

        [Option('g', "ConsumerGroupName", Required = true, HelpText = "The Consumer Group name")]
        public string ConsumerGroupName { get; set; }

        [Option('i', "NumberOfInstances",  DefaultValue = 1, Required = false, HelpText = "The number of instances to create")]
        public int NumberOfInstances { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var usage = new StringBuilder();
            usage.AppendLine("Quickstart Application 1.0");
            usage.AppendLine("Read user manual for usage instructions...");
            return usage.ToString();
        }

    }

    public enum FileType
    {
        Json,
        csv
    }
}
