using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace DocumentDBWebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
             // make sure consumer group exists
            NamespaceManager manager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["ServiceBus.ConnectionString"] + ";TransportType=Amqp");
            ConsumerGroupDescription description = new ConsumerGroupDescription(ConfigurationManager.AppSettings["ServiceBus.Path"], ConfigurationManager.AppSettings["ServiceBus.ConsumerGroup"]);
            manager.CreateConsumerGroupIfNotExists(description);

            //get a handle on the consumer group for the event hub we want to read from
            var factory = MessagingFactory.CreateFromConnectionString(ConfigurationManager.AppSettings["ServiceBus.ConnectionString"] + ";TransportType=Amqp");
            var client = factory.CreateEventHubClient(ConfigurationManager.AppSettings["ServiceBus.Path"]);
            var group = client.GetConsumerGroup(ConfigurationManager.AppSettings["ServiceBus.ConsumerGroup"]);

            while (true)
            {
                Task.WaitAll(client.GetRuntimeInformation().PartitionIds.Select(id => Task.Run(() =>
                {
                    var receiver = @group.CreateReceiver(id);

                    Trace.TraceInformation("Waiting for messages " + receiver.PartitionId);

                    while (true)
                    {
                        try
                        {
                            //read the message
                            var message = receiver.Receive();

                            if (message == null)
                                continue;

                            var body = Encoding.UTF8.GetString(message.GetBytes());

                            if (body == null)
                                continue;

                            var type = MessageType.None;

                            switch (message.PartitionKey.ToLower())
                            {
                                case "energy":
                                    type = MessageType.Energy;
                                    break;
                                case "temperature":
                                    type = MessageType.Temperature;
                                    break;
                                case "humidity":
                                    type = MessageType.Humidity;
                                    break;
                                case "light":
                                    type = MessageType.Light;
                                    break;
                            }

                            if (type == MessageType.None)
                                continue;

                            var writer = new DocumentDBWriter();
                            var task = writer.WriteDocument(type, body);

                            Task.WaitAll(task); // block while the task completes

                            Console.WriteLine(task.Result);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            //suppress for simplicity
                        }
                    }
                })).ToArray());
            }
        }
    }
}
