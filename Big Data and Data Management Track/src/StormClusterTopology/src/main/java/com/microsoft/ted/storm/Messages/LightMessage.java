package com.microsoft.ted.storm.Messages;

import com.google.gson.annotations.SerializedName;

/**
 * Created by david on 29/01/15.
 */
public class LightMessage extends Message {

    public String type = "Light";

    public String getType() {
        return type;
    }

    @SerializedName("Lumens")
    public float lumens;

    public float getLumens() {
        return lumens;
    }

    public void setLumens(float lumens) {
        this.lumens = lumens;
    }
}
