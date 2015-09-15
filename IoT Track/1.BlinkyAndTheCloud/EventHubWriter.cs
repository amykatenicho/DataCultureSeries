namespace com.mtaulty.EventHubLibrary
{
  using Amqp;
  using Amqp.Framing;
  using System;
  using System.Net;
  using System.Text;
  using System.Threading.Tasks;

  public class EventHubWriter
  {
    public EventHubWriter(string eventHubName)
    {
      if (string.IsNullOrEmpty(eventHubName))
      {
        throw new InvalidOperationException("No event hub name specified");
      }
      this.eventHubName = eventHubName;

      this.address = new Lazy<Address>(() =>
      {
        return (new Address(
          string.Format(AmqpAddressFormatString, EventHubPolicyName, WebUtility.UrlEncode(EventHubSasKey))));
      });

      this.connection = new Lazy<Connection>(() =>
      {
        return (new Connection(this.address.Value));
      });

      this.session = new Lazy<Session>(() =>
      {
        return (new Session(this.connection.Value));
      });

      this.senderLink = new Lazy<SenderLink>(() =>
      {
        // directly addresses partition 0
        return (
          new SenderLink(
            this.session.Value,
            "send-link:" + this.eventHubName,
            this.eventHubName + "/Partitions/" + PartitionKey));
      });
    }
    public Task WriteAsync(string messageToSend)
    {
      return (Task.Run(() =>
       {
         // not sure where the async went?
         var message = new Message()
         {
           BodySection = new Data()
           {
             Binary = UTF8Encoding.UTF8.GetBytes(messageToSend)
           }
         };
         this.senderLink.Value.Send(message);
       }));    
    }
    Lazy<Connection> connection;
    Lazy<Session> session;
    Lazy<Address> address;
    Lazy<SenderLink> senderLink;

    static readonly string AmqpAddressFormatString = "amqps://{0}:{1}@mtdemos.servicebus.windows.net";
    static readonly string EventHubSasKey = "SICWlk+hIeMJkxXy4jaCieI2Yp/9Czgem0u+/MQ+yNA=";
    static readonly string EventHubPolicyName = "sender";
    static readonly string PartitionKey = "0";
    string eventHubName;
  }
}
