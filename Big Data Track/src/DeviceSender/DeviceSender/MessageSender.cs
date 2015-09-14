using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace DeviceSender
{
    public class MessageSender
    {
        private readonly EventHubConnectionDetails _config;
        private readonly DeviceSendingDetails _details;
        private readonly int _messagesPerBatch;

        public MessageSender(EventHubConnectionDetails config, DeviceSendingDetails details)
        {
            _config = config;
            _details = details;
            //number of messages that should be sent per batch
            _messagesPerBatch = details.NumberOfDevices * details.NumberOfDeviceTypes;
        }

        public void SendMessages(IEnumerable<String> messages)
        {
            
            var asList = messages.ToList();
            Console.Out.WriteLine("Writing {0} messages to Event Hub", asList.Count);

            var connectionString = String.Format("Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName={1};SharedAccessKey={2};TransportType=Amqp",
                                                 _config.ServiceBusNamespace,
                                                 _config.SasPolicyName,
                                                 _config.SasPolicyKey);
            var factory = MessagingFactory.CreateFromConnectionString(connectionString);
            var eventHubClient = factory.CreateEventHubClient(_config.EventHubName);

            //get the first and last message
            var firstMessage = asList[0];
            var lastMessage = asList[asList.Count - 1];

            //convert to objects
            var firstEntry = JsonConvert.DeserializeObject<FileEntry>(firstMessage);
            var lastEntry = JsonConvert.DeserializeObject<FileEntry>(lastMessage);

            //get the actual message data as an object
            var firstMessageObject = JsonConvert.DeserializeObject<Sensor>(firstEntry.Message);
            var lastMessageObject = JsonConvert.DeserializeObject<Sensor>(lastEntry.Message);

            //given the date time of first and last message determine the frequency at which they were sent
            var period = lastMessageObject.Timestamp - firstMessageObject.Timestamp;
            var totalMilliseconds = period.TotalMilliseconds;
            var delay = (int)(totalMilliseconds/(asList.Count/_messagesPerBatch));

            var startTime = DateTime.UtcNow;

            var count = 0;
            for (var i = 0; i < _details.IterationSeconds; i++)
            {
                Console.WriteLine("Messages fired onto the eventhub!");
                for (var roomNumber = 0; roomNumber < _details.NumberOfRooms; roomNumber++)
                {
                    var batch = new List<EventData>();

                    for (var deviceNumber = 0; deviceNumber < _details.NumberOfDevices; deviceNumber++)
                    {
                        for (var x = 0; x < _details.NumberOfDeviceTypes; x++)
                        {
                            var entry = JsonConvert.DeserializeObject<FileEntry>(asList[count++]);

                            batch.Add(new EventData(Encoding.UTF8.GetBytes(entry.Message))
                            {
                                PartitionKey = entry.PartitionKey
                            });
                        }
                    }

                    var tempEvents = batch.Where(evnt => evnt.PartitionKey == "temperature").Select(evnt => evnt);
                    var energyEvents = batch.Where(evnt => evnt.PartitionKey == "energy").Select(evnt => evnt);
                    var humidityEvents = batch.Where(evnt => evnt.PartitionKey == "humidity").Select(evnt => evnt);
                    var lightEvents = batch.Where(evnt => evnt.PartitionKey == "light").Select(evnt => evnt);
                    eventHubClient.SendBatch(tempEvents);
                    eventHubClient.SendBatch(energyEvents);
                    eventHubClient.SendBatch(humidityEvents);
                    eventHubClient.SendBatch(lightEvents);
                }

                //send all the messages with a delay after each batch.
                Thread.Sleep(_details.MillisecondDelay);
            }

            var duration = DateTime.UtcNow - startTime;
            Console.WriteLine("Total Time (ms) : {0}", duration.TotalMilliseconds);
        }
    }
}
