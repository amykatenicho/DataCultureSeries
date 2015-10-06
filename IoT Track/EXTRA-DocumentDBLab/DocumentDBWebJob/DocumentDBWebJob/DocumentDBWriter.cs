using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Newtonsoft.Json;

namespace DocumentDBWebJob
{
    public class DocumentDBWriter
    {
        public async Task<bool>  WriteDocument(MessageType type, String jsonString)
        {
            var client = new DocumentClient(new Uri(ConfigurationManager.AppSettings["DocumentServiceEndpoint"]), ConfigurationManager.AppSettings["DocumentKey"]);

            var dbName = ConfigurationManager.AppSettings["DocumentDatabase"];
            var database = client.CreateDatabaseQuery().Where(db => db.Id == dbName).AsEnumerable().FirstOrDefault() ??
                           await client.CreateDatabaseAsync(new Database{ Id = dbName});


            // Check to verify a document collection with the does not exist
            var docCollection = ConfigurationManager.AppSettings["DocumentCollection"];
            var documentCollection = client.CreateDocumentCollectionQuery(database.CollectionsLink).Where(c => c.Id == docCollection).AsEnumerable().FirstOrDefault() ??
                                     await client.CreateDocumentCollectionAsync(database.CollectionsLink, new DocumentCollection { Id = docCollection });

            var id = Guid.NewGuid().ToString();

            var response = HttpStatusCode.Unused;
            switch (type)
            {
                case MessageType.Energy:
                    var energyDoc = JsonConvert.DeserializeObject<EnergyDocument>(jsonString);
                    energyDoc.id = id;
                    response = (await client.CreateDocumentAsync(documentCollection.DocumentsLink, energyDoc)).StatusCode;
                    break;
                case MessageType.Humidity:
                    var humidityDoc = JsonConvert.DeserializeObject<HumidityDocument>(jsonString);
                    humidityDoc.id = id;
                    response = (await client.CreateDocumentAsync(documentCollection.DocumentsLink, humidityDoc)).StatusCode;
                    break;
                case MessageType.Light:
                    var lightDoc = JsonConvert.DeserializeObject<LightDocument>(jsonString);
                    lightDoc.id = id;
                    response = (await client.CreateDocumentAsync(documentCollection.DocumentsLink, lightDoc)).StatusCode;
                    break;
                case MessageType.Temperature:
                    var tempDoc = JsonConvert.DeserializeObject<TemperatureDocument>(jsonString);
                    tempDoc.id = id;
                    response = (await client.CreateDocumentAsync(documentCollection.DocumentsLink, tempDoc)).StatusCode;
                    break;
            }

            return response == HttpStatusCode.Created;
        }
    }

    public enum MessageType
    {
        Energy,
        Humidity,
        Temperature,
        Light,
        None
    }
}
