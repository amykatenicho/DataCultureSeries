package com.microsoft.ted.storm;

import backtype.storm.Config;
import backtype.storm.LocalCluster;
import backtype.storm.StormSubmitter;
import backtype.storm.generated.StormTopology;
import backtype.storm.topology.TopologyBuilder;
import backtype.storm.tuple.Fields;
import com.microsoft.eventhubs.spout.EventHubSpout;
import com.microsoft.eventhubs.spout.EventHubSpoutConfig;
import com.microsoft.ted.storm.bolts.DateToMinutesBolt;
import com.microsoft.ted.storm.bolts.DeriveKeyBolt;
import com.microsoft.ted.storm.bolts.HBaseBolt;
import com.microsoft.ted.storm.bolts.JsonParserBolt;

import java.util.Properties;

public class StormTopologyCluster
{
    private EventHubSpoutConfig spoutConfig = null;
    private int numWorkers = 0;
    protected void readEHConfig(String[] args) throws Exception {

        //get the config
        Properties properties = new Properties();
        properties.load(this.getClass().getClassLoader().getResourceAsStream("servicebus.properties"));

        String username = properties.getProperty("username");
        String password = properties.getProperty("password");
        String namespaceName = properties.getProperty("namespace");
        String entityPath = properties.getProperty("eventhub");
        String zkEndpointAddress = properties.getProperty("zookeeper.connectionstring");
        int partitionCount = Integer.parseInt(properties.getProperty("partitions"));
        int checkpointIntervalInSeconds = Integer.parseInt(properties.getProperty("eventhubspout.checkpoint.interval"));
        int receiverCredits = Integer.parseInt(properties.getProperty("eventhub.receiver.credits"));

        spoutConfig = new EventHubSpoutConfig(username, password,
                namespaceName, entityPath, partitionCount, zkEndpointAddress,
                checkpointIntervalInSeconds, receiverCredits);

        //set the number of workers to be the same as partition number.
        //the idea is to have a spout and a partial count bolt co-exist in one
        //worker to avoid shuffling messages across workers in storm cluster.
        numWorkers = spoutConfig.getPartitionCount();

        if(args.length > 0) {
            //set topology name so that sample Trident topology can use it as stream name.
            spoutConfig.setTopologyName(args[0]);
        }
    }

    // Create the spout using the configuration
    protected EventHubSpout createEventHubSpout() {
        EventHubSpout eventHubSpout = new EventHubSpout(spoutConfig);
        return eventHubSpout;
    }

    // Build the topology
    protected StormTopology buildTopology(EventHubSpout eventHubSpout) {
        TopologyBuilder topologyBuilder = new TopologyBuilder();

        // Name the spout 'EventHubsSpout', and set it to create
        // as many as we have partition counts in the config file
        topologyBuilder.setSpout("EventHub", eventHubSpout, spoutConfig.getPartitionCount())
                .setNumTasks(spoutConfig.getPartitionCount());

        // Create the parser bolt, which subscribes to the stream from EventHub
        topologyBuilder.setBolt("JsonParserBolt", new JsonParserBolt(), spoutConfig.getPartitionCount())
                .localOrShuffleGrouping("EventHub").setNumTasks(spoutConfig.getPartitionCount());

        //Create the aug bolt to augment the data
        topologyBuilder.setBolt("DateToMinutesBolt", new DateToMinutesBolt(), spoutConfig.getPartitionCount())
                .fieldsGrouping("JsonParserBolt", "readings", new Fields("messagetype", "deviceId", "timestamp", "roomNumber", "reading")).setNumTasks(spoutConfig.getPartitionCount());

        topologyBuilder.setBolt("DeriveKeyBolt", new DeriveKeyBolt(), spoutConfig.getPartitionCount())
                .fieldsGrouping("DateToMinutesBolt", "readings", new Fields("messagetype", "deviceId", "timestamp", "roomNumber", "reading")).setNumTasks(spoutConfig.getPartitionCount());

        //Create the Redis Storage bolt
        topologyBuilder.setBolt("HBaseBolt", new HBaseBolt(), spoutConfig.getPartitionCount())
                .fieldsGrouping("DeriveKeyBolt", "readings", new Fields("key", "value")).setNumTasks(spoutConfig.getPartitionCount()*2);

        return topologyBuilder.createTopology();
    }

    protected void submitTopology(String[] args, StormTopology topology, Config config) throws Exception {
        // Config config = new Config();
        config.setDebug(false);

        //Enable metrics
        config.registerMetricsConsumer(backtype.storm.metric.LoggingMetricsConsumer.class, 1);

        // Is this running locally, or on an HDInsight cluster?
        if (args != null && args.length > 0) {
            config.setNumWorkers(numWorkers);
            StormSubmitter.submitTopology(args[0], config, topology);
        } else {
            config.setMaxTaskParallelism(2);

            LocalCluster localCluster = new LocalCluster();
            localCluster.submitTopology("test", config, topology);

            Thread.sleep(5000000);

            localCluster.shutdown();
        }
    }

    // Loads the configuration, creates the spout, builds the topology,
    // and then submits it
    protected void runScenario(String[] args) throws Exception{
        readEHConfig(args);
        Config config = new Config();

        EventHubSpout eventHubSpout = createEventHubSpout();
        StormTopology topology = buildTopology(eventHubSpout);
        submitTopology(args, topology, config);
    }

    public static void main(String[] args) throws Exception {
        StormTopologyCluster scenario = new StormTopologyCluster();
        scenario.runScenario(args);
    }

}
