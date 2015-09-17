package com.microsoft.ted.storm.bolts;

import backtype.storm.topology.BasicOutputCollector;
import backtype.storm.topology.OutputFieldsDeclarer;
import backtype.storm.topology.base.BaseBasicBolt;
import backtype.storm.tuple.Tuple;
import org.apache.hadoop.conf.Configuration;
import org.apache.hadoop.hbase.HBaseConfiguration;
import org.apache.hadoop.hbase.client.Get;
import org.apache.hadoop.hbase.client.HTable;
import org.apache.hadoop.hbase.client.Put;
import org.apache.hadoop.hbase.client.Result;
import org.apache.hadoop.hbase.util.Bytes;

import java.io.IOException;

/**
 * Created by david on 03/02/15.
 */
public class HBaseBolt extends BaseBasicBolt {
    @Override
    public void execute(Tuple input, BasicOutputCollector collector) {

        String key = input.getStringByField("key");
        Float value = input.getFloatByField("value");
        try {
            //get the table
            Configuration conf = HBaseConfiguration.create();
            HTable table = new HTable(conf, "readings");

            //try get the row that relates to the key
            Get get = new Get(Bytes.toBytes(key));
            get.addFamily(Bytes.toBytes("count"));
            get.addFamily(Bytes.toBytes("sum"));
            get.addFamily(Bytes.toBytes("average"));

            Result result = table.get(get);

            //get the current values in hbase
            String count = "0";
            String sum = "0";

            if (result != null && !result.isEmpty()) {
                count = new String(result.getValue(Bytes.toBytes("count"), Bytes.toBytes("count")));
                sum = new String(result.getValue(Bytes.toBytes("sum"), Bytes.toBytes("sum")));
            }

            //calc the new values
            float countInt = Float.parseFloat(count) + 1;
            float sumFloat = Float.parseFloat(sum) + value;
            float averageFloat = sumFloat / countInt;

            //push them to hbase, if key exists it will be updated
            Put put = new Put(Bytes.toBytes(key));
            put.add(Bytes.toBytes("count"), Bytes.toBytes("count"), Bytes.toBytes(Float.toString(countInt)));
            put.add(Bytes.toBytes("sum"), Bytes.toBytes("sum"), Bytes.toBytes(Float.toString(sumFloat)));
            put.add(Bytes.toBytes("average"), Bytes.toBytes("average"), Bytes.toBytes(Float.toString(averageFloat)));

            table.put(put);
            table.flushCommits();
            table.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

    @Override
    public void declareOutputFields(OutputFieldsDeclarer declarer) {

    }
}
