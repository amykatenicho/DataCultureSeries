using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNet.Highcharts;
using DotNet.Highcharts.Helpers;
using DotNet.Highcharts.Options;

namespace DocumentDBWebApp
{
    public class HighchartsHelper
    {
        private IEnumerable<ChartEntry> ProcessLightData(IEnumerable<LightDocument> documents)
        {
            var asList = documents.ToList();

            //ensure we can group on time
            foreach (var item in asList)
            {
                item.Timestamp = DateTime.Parse(item.Timestamp.ToString("yyyy-MM-dd hh:mm:00"));
            }

            return (from room in asList.Select(a => a.RoomNumber).ToList().Distinct().ToList()
                    let keys = asList.Select(a => a.Timestamp.ToString()).Distinct().ToArray()
                    let data = asList.Where(a => a.RoomNumber == room).Select(a => a).GroupBy(a => a.Timestamp).Select(g => g.Average(p => p.Lumens)).Select(a => (object)a.ToString()).ToArray()
                    select new ChartEntry()
                    {
                        RoomNumber = room,
                        Catagories = keys,
                        Series = data
                    }).ToList();
        }

        private IEnumerable<ChartEntry> ProcessEnergyData(IEnumerable<EnergyDocument> documents)
        {
            var asList = documents.ToList();

            //ensure we can group on time
            foreach (var item in asList)
            {
                item.Timestamp = DateTime.Parse(item.Timestamp.ToString("yyyy-MM-dd hh:mm:00"));
            }

            return (from room in asList.Select(a => a.RoomNumber).ToList().Distinct().ToList()
                    let keys = asList.Select(a => a.Timestamp.ToString()).Distinct().ToArray()
                    let data = asList.Where(a => a.RoomNumber == room).Select(a => a).GroupBy(a => a.Timestamp).Select(g => g.Average(p => p.Kwh)).Select(a => (object)a.ToString()).ToArray()
                    select new ChartEntry()
                    {
                        RoomNumber = room,
                        Catagories = keys,
                        Series = data
                    }).ToList();
        }

        private IEnumerable<ChartEntry> ProcessHumidityData(IEnumerable<HumidityDocument> documents)
        {
            var asList = documents.ToList();

            //ensure we can group on time
            foreach (var item in asList)
            {
                item.Timestamp = DateTime.Parse(item.Timestamp.ToString("yyyy-MM-dd hh:mm:00"));
            }

            return (from room in asList.Select(a => a.RoomNumber).ToList().Distinct().ToList()
                    let keys = asList.Select(a => a.Timestamp.ToString()).Distinct().ToArray()
                    let data = asList.Where(a => a.RoomNumber == room).Select(a => a).GroupBy(a => a.Timestamp).Select(g => g.Average(p => p.Humidity)).Select(a => (object)a.ToString()).ToArray()
                    select new ChartEntry()
                    {
                        RoomNumber = room,
                        Catagories = keys,
                        Series = data
                    }).ToList();
        }

        private IEnumerable<ChartEntry> ProcessTemperatureData(IEnumerable<TemperatureDocument> documents)
        {
            var asList = documents.ToList();

            //ensure we can group on time
            foreach (var item in asList)
            {
                item.Timestamp = DateTime.Parse(item.Timestamp.ToString("yyyy-MM-dd hh:mm"));
            }

            return (from room in asList.Select(a => a.RoomNumber).ToList().Distinct().ToList()
                    let keys = asList.Select(a => a.Timestamp.ToString()).Distinct().ToArray()
                    let data = asList.Where(a => a.RoomNumber == room).Select(a => a).GroupBy(a => a.Timestamp).Select(g => g.Average(p => p.Temperature)).Select(a => (object)a.ToString()).ToArray()
                    select new ChartEntry()
                    {
                        RoomNumber = room,
                        Catagories = keys,
                        Series = data
                    }).ToList();
        }

        private string[] GetCatagories(IEnumerable<ChartEntry> entries)
        {
            var catagories = new List<String>();
            foreach (var entry in entries)
            {
                catagories.AddRange(entry.Catagories);
            }

            return catagories.Distinct().ToArray();
        }

        private Series[] GetSeriesData(IEnumerable<ChartEntry> entries)
        {
            return entries.Select(entry => new Series()
            {
                Name = entry.RoomNumber.ToString(),
                Id = entry.RoomNumber.ToString(),
                Data = new Data(entry.Series)
            }).ToArray();
        }

        public Highcharts GetLightChart()
        {
            var reader = new DocumentDBDataReader();
            var lightData = reader.GetLightData().ToList();

            var processedData = ProcessLightData(lightData).ToList();

            var catagories = GetCatagories(processedData);
            var series = GetSeriesData(processedData);

            return new Highcharts("light").SetTitle(new Title()
            {
                Text = "Lumens Per Minute"
            }).SetXAxis(new XAxis()
            {
                Categories = catagories
            }).SetSeries(series);
        }


        public Highcharts GetTemperatureChart()
        {
            var reader = new DocumentDBDataReader();
            var tempData = reader.GetTemperatureData().ToList();

            var processedData = ProcessTemperatureData(tempData).ToList();

            var catagories = GetCatagories(processedData);
            var series = GetSeriesData(processedData);

            return new Highcharts("Temperature").SetTitle(new Title()
            {
                Text = "Temperature Per Minute"
            }).SetXAxis(new XAxis()
            {
                Categories = catagories
            }).SetSeries(series);
        }

        public Highcharts GetHumidityChart()
        {
            var reader = new DocumentDBDataReader();
            var humidityData = reader.GetHumidityData().ToList();

            var processedData = ProcessHumidityData(humidityData).ToList();

            var catagories = GetCatagories(processedData);
            var series = GetSeriesData(processedData);

            return new Highcharts("Humidity").SetTitle(new Title()
            {
                Text = "Humidity Per Minute"
            }).SetXAxis(new XAxis()
            {
                Categories = catagories
            }).SetSeries(series);
        }

        public Highcharts GetEnergyChart()
        {
            var reader = new DocumentDBDataReader();
            var energyData = reader.GetEnergyData().ToList();

            var processedData = ProcessEnergyData(energyData).ToList();

            var catagories = GetCatagories(processedData);
            var series = GetSeriesData(processedData);

            return new Highcharts("Energy").SetTitle(new Title()
            {
                Text = "Energy Per Minute"
            }).SetXAxis(new XAxis()
            {
                Categories = catagories
            }).SetSeries(series);
        }

        public enum DocumentTypes
        {
            Humidity,
            Temperature,
            Light,
            Energy
        }

        public class ChartEntry
        {
            public int RoomNumber { get; set; }
            public string[] Catagories { get; set; }
            public object[] Series { get; set; }
        }
    }
}