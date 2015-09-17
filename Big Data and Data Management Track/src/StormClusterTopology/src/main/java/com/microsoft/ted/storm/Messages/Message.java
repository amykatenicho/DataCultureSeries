package com.microsoft.ted.storm.Messages;

import com.google.gson.annotations.SerializedName;

/**
 * Created by david on 29/01/15.
 */
public class Message {

    @SerializedName("Deviceid")
    public String deviceId;

    @SerializedName("Timestamp")
    public String timestamp;

    @SerializedName("RoomNumber")
    public int roomNumber;

    public String getDeviceId() {
        return deviceId;
    }

    public void setDeviceId(String deviceId) {
        this.deviceId = deviceId;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }

    public int getRoomNumber() {
        return roomNumber;
    }

    public void setRoomNumber(int roomNumber) {
        this.roomNumber = roomNumber;
    }


}



