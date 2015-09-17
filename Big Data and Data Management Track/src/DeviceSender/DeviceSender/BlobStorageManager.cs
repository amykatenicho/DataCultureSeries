using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DeviceSender
{
    public class BlobStorageManager
    {
        private readonly CloudBlobContainer _container;
        private readonly BlobStorageConnectionDetails _config;
        public BlobStorageManager(BlobStorageConnectionDetails config)
        {
            _config = config;
            var connectionString = String.Format("DefaultEndpointsProtocol=https;AccountName={0};AccountKey={1}", _config.SasPolicyName, _config.SasPolicyKey);
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var containerRef = blobClient.GetContainerReference(config.Container);
            if (!containerRef.Exists())
                throw new FileNotFoundException("Could not retrieve blob storage container {0}", config.Container);

            Console.WriteLine("Connected to Blob Storage account.");
            _container = containerRef;
        }

        public bool MoveFile(String blobName)
        {
            var blob = _container.GetBlockBlobReference(Path.Combine(_config.RootFolder, blobName));

            var archived = _container.GetBlockBlobReference(Path.Combine(_config.ArchiveFolder, blobName));

            archived.StartCopyFromBlob(blob);

            while (archived.CopyState.Status != CopyStatus.Success)
                Thread.Sleep(1000);

            return archived.CopyState.Status == CopyStatus.Success;
        }

        public IEnumerable<String> ListBlobs()
        {
            return _container.ListBlobs(_config.RootFolder).Select(blob => Path.GetFileName(blob.Uri.AbsoluteUri)).ToList();
        }

        public IEnumerable<String> GetLines(String blobName)
        {
            var filenamepath = Path.Combine(_config.RootFolder, blobName);
            Console.Out.WriteLine("Retrieving contents of file {0}", filenamepath);

            var blob = _container.GetBlockBlobReference(filenamepath);
            return blob.DownloadText().Replace("\r\n", "\n").Split('\n').Where(line => !String.IsNullOrEmpty(line)).Select(line => line);
        }

        public bool CreateFile(string sourceFile)
        {
            Console.WriteLine("Getting Block Blob reference ...");

            var filenamepath = Path.Combine(_config.RootFolder, Path.GetFileName(sourceFile));
            var blob = _container.GetBlockBlobReference(filenamepath);

            Console.WriteLine("Uploading file {0} to Block Blob {1} ...", sourceFile, filenamepath);
            blob.BeginUploadFromFile(sourceFile, FileMode.Open, null, null);
      
            while (!blob.Exists())
                Thread.Sleep(1000);

            Console.WriteLine("File created successfully");
            return true;
        }
    }
}
