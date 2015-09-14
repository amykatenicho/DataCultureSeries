package com.microsoft.ted.storm.bolts;

import backtype.storm.topology.BasicOutputCollector;
import backtype.storm.topology.OutputFieldsDeclarer;
import backtype.storm.topology.base.BaseBasicBolt;
import backtype.storm.tuple.Fields;
import backtype.storm.tuple.Tuple;
import backtype.storm.tuple.Values;
import com.google.gson.Gson;
import com.microsoft.ted.storm.Messages.EnergyMessage;
import com.microsoft.ted.storm.Messages.HumidityMessage;
import com.microsoft.ted.storm.Messages.LightMessage;
import com.microsoft.ted.storm.Messages.TemperatureMessage;

/**
 * Created by david on 03/02/15.
 */
public class JsonParserBolt extends BaseBasicBolt {

    @Override
    public void execute(Tuple input, BasicOutputCollector collector) {
        String sentence = input.getString(0);
        Gson gson = new Gson();

        if(sentence.toLowerCase().contains("kwh"))
        {
            EnergyMessage msg = gson.fromJson(sentence, EnergyMessage.class);
            collector.emit(new Values(msg.getType(), msg.getDeviceId(), msg.getTimestamp(), msg.getRoomNumber(), msg.getKwh()));
        }else if(sentence.toLowerCase().contains("temperature"))
        {
            TemperatureMessage msg = gson.fromJson(sentence, TemperatureMessage.class);
            collector.emit(new Values(msg.getType(), msg.getDeviceId(), msg.getTimestamp(), msg.getRoomNumber(), msg.getTemperature()));
        }else if(sentence.toLowerCase().contains("lumens"))
        {
            LightMessage msg = gson.fromJson(sentence, LightMessage.class);
            collector.emit(new Values(msg.getType(), msg.getDeviceId(), msg.getTimestamp(), msg.getRoomNumber(), msg.getLumens()));
        }else if(sentence.toLowerCase().contains("humidity"))
        {
            HumidityMessage msg = gson.fromJson(sentence, HumidityMessage.class);
            collector.emit("readings", new Values(msg.getType(), msg.getDeviceId(), msg.getTimestamp(), msg.getRoomNumber(), msg.getHumidity()));
        }
    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {
        declarer.declareStream("readings", new Fields("messagetype", "deviceId", "timestamp", "roomNumber", "reading"));
    }
}
