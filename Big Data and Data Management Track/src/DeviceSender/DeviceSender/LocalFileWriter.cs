using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceSender
{
    public class LocalFileWriter
    {
        public bool WriteToFile(String filename, IEnumerable<String> lines)
        {
            try
            {
                File.AppendAllLines(filename, lines );
            }
            catch (Exception e)
            {
                Console.Out.WriteLine(e.Message);
                Console.Out.WriteLine(e.StackTrace);
                return false;
            }
            
            return true;
        }
    }
}
