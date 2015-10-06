using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DocumentDBWebApp.Controllers;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;

namespace DocumentDBWebApp
{
    public class DocumentDBDataReader
    {
        private readonly DocumentCollection _documentCollection;
        private readonly Database _database;
        private readonly DocumentClient _documentClient;

        public DocumentDBDataReader()
        {
            var dict = new Dictionary<HighchartsHelper.DocumentTypes, IEnumerable<Document>>();
            _documentClient = new DocumentClient(new Uri(ConfigurationManager.AppSettings["DocumentServiceEndpoint"]), ConfigurationManager.AppSettings["DocumentKey"]);

            _database = _documentClient.CreateDatabaseQuery().Where(db => db.Id == ConfigurationManager.AppSettings["DocumentDatabase"]).AsEnumerable().FirstOrDefault();

            if (_database == null)
                throw new ApplicationException("Error: DocumentDB database does not exist");

            // Check to verify a document collection with the id=FamilyCollection does not exist
            _documentCollection = _documentClient.CreateDocumentCollectionQuery(_database.CollectionsLink).Where(c => c.Id == ConfigurationManager.AppSettings["DocumentCollection"]).AsEnumerable().FirstOrDefault();

            if (_documentCollection == null)
                throw new ApplicationException("Error: DocumentDB collection does not exist");


            try
            {
                _documentClient.CreateUserDefinedFunctionAsync(_documentCollection.SelfLink, new UserDefinedFunction
                {
                    Id = "ISDEFINED",
                    Body = @"function ISDEFINED(doc, prop) {
                            return doc[prop] !== undefined;
                        }"
                });  
            }
            catch (Exception)
            {
                //fail silently for now..
            }
        }

        public IEnumerable<EnergyDocument> GetEnergyData()
        {
            return _documentClient.CreateDocumentQuery<EnergyDocument>(_documentCollection.DocumentsLink, String.Format("SELECT * FROM {0} t WHERE udf.ISDEFINED(t, 'Kwh')",
                ConfigurationManager.AppSettings["DocumentCollection"])).ToList();
        }

        public IEnumerable<TemperatureDocument> GetTemperatureData()
        {
            return _documentClient.CreateDocumentQuery<TemperatureDocument>(_documentCollection.DocumentsLink, String.Format("SELECT * FROM {0} t WHERE udf.ISDEFINED(t, 'Temperature')",
                ConfigurationManager.AppSettings["DocumentCollection"])).ToList();
        }

        public IEnumerable<HumidityDocument> GetHumidityData()
        {
            return _documentClient.CreateDocumentQuery<HumidityDocument>(_documentCollection.DocumentsLink, String.Format("SELECT * FROM {0} t WHERE udf.ISDEFINED(t, 'Humidity')",
                ConfigurationManager.AppSettings["DocumentCollection"])).ToList();
            
        }

        public IEnumerable<LightDocument> GetLightData()
        {
            return _documentClient.CreateDocumentQuery<LightDocument>(_documentCollection.DocumentsLink, String.Format("SELECT * FROM {0} t WHERE udf.ISDEFINED(t, 'Lumens')",
                ConfigurationManager.AppSettings["DocumentCollection"])).ToList();
        }
    }
}