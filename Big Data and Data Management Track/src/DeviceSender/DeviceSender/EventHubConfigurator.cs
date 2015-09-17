using System;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;

namespace DeviceSender
{
    public class EventHubConfigurator
    {
        private readonly EventHubConnectionDetails _config;

        public EventHubConfigurator(EventHubConnectionDetails config)
        {
            _config = config;
        }

        public bool AddConsumerGroup(String groupname)
        {
            var connectionString = String.Format("Endpoint=sb://{0}.servicebus.windows.net/;SharedAccessKeyName={1};SharedAccessKey={2};TransportType=Amqp",
                                                 _config.ServiceBusNamespace,
                                                 _config.SasPolicyName,
                                                 _config.SasPolicyKey);

            Console.WriteLine("Creating consumer group {0}", groupname);

            var manager = NamespaceManager.CreateFromConnectionString(connectionString);
            var description = new ConsumerGroupDescription(_config.EventHubName, groupname);
            var result = manager.CreateConsumerGroupIfNotExists(description);

            Console.WriteLine("Consumer group created : {0}", result != null);
            return result != null;
        }
    }
}
