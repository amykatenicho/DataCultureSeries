package com.microsoft.ted.storm.Messages;

import com.google.gson.annotations.SerializedName;

/**
 * Created by david on 29/01/15.
 */
public class EnergyMessage extends Message {

    public String type = "Energy";

    public String getType() {
        return type;
    }

    @SerializedName("Kwh")
    public float kwh;

    public float getKwh() {
        return kwh;
    }

    public void setKwh(float kwh) {
        this.kwh = kwh;
    }
}
