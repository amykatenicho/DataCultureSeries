package com.microsoft.ted.storm.bolts;

import backtype.storm.topology.BasicOutputCollector;
import backtype.storm.topology.OutputFieldsDeclarer;
import backtype.storm.topology.base.BaseBasicBolt;
import backtype.storm.tuple.Fields;
import backtype.storm.tuple.Tuple;
import backtype.storm.tuple.Values;
import org.joda.time.DateTime;

/**
 * Created by david on 03/02/15.
 */
public class DateToMinutesBolt extends BaseBasicBolt {
    @Override
    public void execute(Tuple input, BasicOutputCollector collector) {
        String timestamp = input.getStringByField("timestamp");

        timestamp = DateTime.parse(timestamp).toString("yyyy-MM-dd'T'hh:mm") + ":00.0000000Z";

        collector.emit("readings", new Values(input.getStringByField("messagetype"), input.getStringByField("deviceId"), timestamp, input.getIntegerByField("roomNumber"), input.getFloatByField("reading")));

    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {
        declarer.declareStream("readings", new Fields("messagetype", "deviceId", "timestamp", "roomNumber", "reading"));
    }
}
