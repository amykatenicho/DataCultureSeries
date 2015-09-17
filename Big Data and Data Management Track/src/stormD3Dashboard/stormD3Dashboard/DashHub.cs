using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.HBase.Client;
using Newtonsoft.Json;
using org.apache.hadoop.hbase.rest.protobuf.generated;


namespace stormD3Dashboard
{
    public class DashHub : Hub
    {
        private static Task Background = null;
        public bool StartListening()
        {
            Background = Task.Run(async () =>
            {
                while (true)
                {
     
                    var clusterURL = ConfigurationManager.AppSettings["ClusterUrl"];
                    var hadoopUsername = ConfigurationManager.AppSettings["Username"];
                    var hadoopUserPassword = ConfigurationManager.AppSettings["Password"];

                    var hbaseTableName =ConfigurationManager.AppSettings["TableName"];

                    // Create a new instance of an HBase client.
                    var creds = new ClusterCredentials(new Uri(clusterURL), hadoopUsername, hadoopUserPassword);
                    var hbaseClient = new HBaseClient(creds);

                    //Scan over rows in a table. Assume the table has integer keys and you want data between keys 25 and 35. 
                    var scanSettings = new Scanner()
                    {
                        batch = 10
                    };

                    var scannerInfo = await hbaseClient.CreateScannerAsync(hbaseTableName, scanSettings);
                    CellSet next = null;

                    var res = new List<HBaseEntry>();
                    var rowNum = 0;
                    while ((next = hbaseClient.ScannerGetNext(scannerInfo)) != null)
                    {
                        res.AddRange(from row in next.rows
                            let key = Encoding.UTF8.GetString(row.key)
                            let value = Encoding.UTF8.GetString(row.values[0].data)
                            let parts = key.Split(',')
                            where parts.Length == 3
                            select new HBaseEntry()
                            {
                                Type = parts[0], Date = parts[1].Replace("0000000", "000"), RoomNumber = parts[2], Reading = value
                            });

                    }

                    var dateRangedList =
                        res.Where(a => DateTime.Parse(a.Date) >= DateTime.UtcNow.AddDays(-5)).Select(a => a).ToList();

                    var map = new Dictionary<String, Dictionary<String, Dictionary<String, String>>> ();
                    var tempList = dateRangedList.Where(a => a.Type == "Temperature").ToList();
                    var engyList = dateRangedList.Where(a => a.Type == "Energy").ToList();
                    var humList = dateRangedList.Where(a => a.Type == "Humidity").ToList();
                    var lghtList = dateRangedList.Where(a => a.Type == "Light").ToList();

                    map.Add("Temperature", GetDataAsMap(tempList));
                    map.Add("Energy", GetDataAsMap(engyList));
                    map.Add("Humidity", GetDataAsMap(humList));
                    map.Add("Light", GetDataAsMap(lghtList));

                    //get data
                    var ctx = GlobalHost.ConnectionManager.GetHubContext<DashHub>();

                    ctx.Clients.All.acceptData(JsonConvert.SerializeObject(map));
                    Thread.Sleep(15000);
                }
            });
            Background.Wait();

            return true;
        }

        public Dictionary<String, Dictionary<String, String>> GetDataAsMap(IEnumerable<HBaseEntry> data)
        {
            var dataList = data.ToList();

            var roomNumbers = dataList.Select(a => a.RoomNumber).Distinct().ToList();

            var res = new Dictionary<String, Dictionary<String, String>> ();
            foreach (var room in roomNumbers)
            {
                var readings = dataList.Where(a => a.RoomNumber == room).Select(a => a).ToList();

                var entry = new Dictionary<String, String>();
                foreach (var reading in readings.Where(reading => !entry.ContainsKey(reading.Date.Replace("0000000", "000"))))
                {
                    entry.Add(reading.Date.Replace("0000000", "000"), reading.Reading);
                }
                /*var entry = readings.ToDictionary(reading => reading.Date, reading => reading.Reading);*/
                res.Add(room, entry);

            }

            return res;
        }

        
    }

   


    public class HBaseEntry
    {
        public String Type { get; set; }
        public String Date { get; set; }
        public String RoomNumber { get; set; }
        public String Reading { get; set; }
    }
}