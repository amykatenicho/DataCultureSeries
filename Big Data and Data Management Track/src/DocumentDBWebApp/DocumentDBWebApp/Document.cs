using System;

namespace DocumentDBWebApp
{

    public class Document
    {
        public String id { get; set; }
        public string Deviceid { get; set; }
        public DateTime Timestamp { get; set; }
        public int RoomNumber { get; set; }
    }

    public class TemperatureDocument : Document
    {
        public float Temperature { get; set; }
    }

    public class HumidityDocument : Document
    {
        public float Humidity { get; set; }
    }

    public class EnergyDocument : Document
    {
        public float Kwh { get; set; }
    }

    public class LightDocument : Document
    {
        public float Lumens { get; set; }
    }
   
}
