using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSender
{
    public class BlobStorageConnectionDetails
    {
        public BlobStorageConnectionDetails(string sasPolicyName, string sasPolicyKey, string container, string rootFolder, string archiveFolder)
        {
            SasPolicyName = sasPolicyName;
            SasPolicyKey = sasPolicyKey;
            Container = container;
            ArchiveFolder = archiveFolder;
            RootFolder = rootFolder;
        }

        public string ServiceBusNamespace { get; set; }
        public string Container { get; set; }
        public string SasPolicyName { get; set; }
        public string SasPolicyKey { get; set; }
        public string ArchiveFolder { get; set; }
        public string RootFolder { get; set; }

    }
}
