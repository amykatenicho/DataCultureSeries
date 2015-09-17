namespace DeviceSender
{
    public class EventHubConnectionDetails
    {
        public EventHubConnectionDetails(string serviceBusNamespace, string eventHubName, string sasPolicyName, string sasPolicyKey)
        {
            ServiceBusNamespace = serviceBusNamespace;
            EventHubName = eventHubName;
            SasPolicyName = sasPolicyName;
            SasPolicyKey = sasPolicyKey;
        }

        public string ServiceBusNamespace { get; set; }
        public string EventHubName { get; set; }
        public string SasPolicyName { get; set; }
        public string SasPolicyKey { get; set; }
    }
}
