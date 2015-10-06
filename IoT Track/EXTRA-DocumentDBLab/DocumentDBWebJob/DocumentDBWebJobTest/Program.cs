using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentDBWebJob;

namespace DocumentDBWebJobTest
{
    class Program
    {
        static void Main(string[] args)
        {
          var writer = new DocumentDBWriter();
          var task = writer.WriteDocument(MessageType.Energy,
                "{\"Kwh\":250819.0,\"Deviceid\":\"energy0\",\"Timestamp\":\"2015-01-20T15:00:33.2764744Z\",\"RoomNumber\":0}");

          Task.WaitAll(task); // block while the task completes

          var result = task.Result;
        }
    }
}
