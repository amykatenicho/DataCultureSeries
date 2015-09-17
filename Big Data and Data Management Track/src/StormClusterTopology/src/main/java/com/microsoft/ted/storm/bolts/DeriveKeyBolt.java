package com.microsoft.ted.storm.bolts;

import backtype.storm.topology.BasicOutputCollector;
import backtype.storm.topology.OutputFieldsDeclarer;
import backtype.storm.topology.base.BaseBasicBolt;
import backtype.storm.tuple.Fields;
import backtype.storm.tuple.Tuple;
import backtype.storm.tuple.Values;

/**
 * Created by david on 03/02/15.
 */
public class DeriveKeyBolt  extends BaseBasicBolt{
    @Override
    public void execute(Tuple input, BasicOutputCollector collector) {
        String key = String.format("%s,%s,%d", input.getStringByField("messagetype"), input.getStringByField("timestamp"), input.getIntegerByField("roomNumber"));

        collector.emit("readings", new Values(key, input.getFloatByField("reading")));
    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {
        declarer.declareStream("readings", new Fields("key", "value"));
    }
}
