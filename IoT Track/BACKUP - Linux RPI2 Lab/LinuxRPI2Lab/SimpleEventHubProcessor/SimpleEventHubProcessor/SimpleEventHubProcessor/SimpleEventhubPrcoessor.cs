namespace SimpleEventHubProcessor
{
    using System.Diagnostics;
    using System.Runtime.Serialization.Json;
    using System.Threading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.ServiceBus.Messaging;
    using Newtonsoft.Json;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using System.Configuration;

    public class SimpleEventProcessor : IEventProcessor
    {
        //IDictionary<string, int> map;
        PartitionContext partitionContext;
        Stopwatch checkpointStopWatch;

        public SimpleEventProcessor()
        {
            //this.map = new Dictionary<string, int>();
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine(string.Format("SimpleEventProcessor initialize.  Partition: '{0}', Offset: '{1}'", context.Lease.PartitionId, context.Lease.Offset));
            this.partitionContext = context;
            this.checkpointStopWatch = new Stopwatch();
            this.checkpointStopWatch.Start();
            return Task.FromResult<object>(null);
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> events)
        {
            try
            {

                foreach (EventData eventData in events)
                {

                    //string key = eventData.PartitionKey;

                    // Get message from the eventData body and convert JSON string into message object
                    string eventBodyAsString = Encoding.UTF8.GetString(eventData.GetBytes());
                    Console.WriteLine();
                    Console.WriteLine(eventBodyAsString);
                    Console.WriteLine();

                    //IList<IDictionary<string, object>> messagePayloads;
                    //try
                    //{
                    //    // Attempt to deserialze event body as single JSON message
                    //    messagePayloads = new List<IDictionary<string, object>> 
                    //    { 
                    //        JsonConvert.DeserializeObject<IDictionary<string, object>>(eventBodyAsString)
                    //    };
                    //}
                    //catch
                    //{
                    //    // Not a single JSON message: attempt to deserialize as array of messages

                    //    // Azure Stream Analytics Preview generates invalid JSON for some multi-values queries
                    //    // Workaround: turn concatenated json objects (ivalid JSON) into array of json objects (valid JSON)
                    //    if (eventBodyAsString.IndexOf("}{") >= 0)
                    //    {
                    //        eventBodyAsString = eventBodyAsString.Replace("}{", "},{");
                    //        eventBodyAsString = "[" + eventBodyAsString + "]";
                    //    }
                    //    messagePayloads = JsonConvert.DeserializeObject<IList<IDictionary<string, object>>>(eventBodyAsString);
                    //}

                }

                //Call checkpoint every 5 minutes, so that worker can resume processing from the 5 minutes back if it restarts.
                if (this.checkpointStopWatch.Elapsed > TimeSpan.FromMinutes(5))
                {
                    Console.WriteLine("Check point");
                    await context.CheckpointAsync();
                    lock (this)
                    {
                        this.checkpointStopWatch.Reset();
                        this.checkpointStopWatch.Start();
                    }
                }
            }
            catch (Exception exp)
            {
                Console.WriteLine("Error in processing: " + exp.Message);
            }
        }

        public async Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine(string.Format("Processor Shuting Down.  Partition '{0}', Reason: '{1}'.", this.partitionContext.Lease.PartitionId, reason.ToString()));
            if (reason == CloseReason.Shutdown)
            {
                await context.CheckpointAsync();
            }
        }

    }



}
