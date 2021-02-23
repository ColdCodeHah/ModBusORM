using ModBusORM;
using Program;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var con = new ModbusConnection()
            {
                ComNo = "COM2",                
            };
            var blOpen = con.Open();
            Console.WriteLine(blOpen);

            var dev = new ModbusDevice<Device1> { ByteOrder = 0, DeviceAddress = 1 };
            con.Register(dev);              

            try
            {
                var flow = dev.Read(x => x.Year);
                Console.WriteLine(flow);
            }
            catch (ComException ex)
            {
                Console.WriteLine(ex.Message);
            } 

            while (true)
            {
                if (!float.TryParse(Console.ReadLine(), out float f)) continue;
                var entity = new Device1 { Two = f };

                var result = dev.Write(entity,x=>x.Two);

                Console.WriteLine(result?.ToString()); 

                entity = dev.Read();
                Console.WriteLine($"{entity.Year}-{entity.Month}-{entity.Day} {entity.Hour}:{entity.Minute}:{entity.Second}");                
            }

            
        }
    }
}
