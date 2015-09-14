package com.microsoft.ted.storm.Messages;

import com.google.gson.annotations.SerializedName;

/**
 * Created by david on 29/01/15.
 */
public class TemperatureMessage extends Message {



    public String type = "Temperature";

    public String getType() {
        return type;
    }

    @SerializedName("Temperature")
    public float temperature;

    public float getTemperature() {
        return temperature;
    }

    public void setTemperature(float temperature) {
        this.temperature = temperature;
    }
}
