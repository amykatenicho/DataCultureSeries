package com.microsoft.ted.storm.Messages;

import com.google.gson.annotations.SerializedName;

/**
 * Created by david on 29/01/15.
 */
public class HumidityMessage extends Message {

    public String type = "Humidity";

    public String getType() {
        return type;
    }

    @SerializedName("Humidity")
    public float humidity;

    public float getHumidity() {
        return humidity;
    }

    public void setHumidity(float humidity) {
        this.humidity = humidity;
    }
}
