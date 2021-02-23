using ModBusORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sample
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct Device1
    {
        [Storage("材质系数", 1001)]
        public float One { get; set; }
        [Storage("初始厚度", 1003)]
        public float Two { get; set; }

        [Storage("年", 2001)]
        public uint Year { get; set; }
        [Storage("月", 2003)]
        public uint Month { get; set; }
        [Storage("日", 2005)]
        public uint Day { get; set; }
        [Storage("时", 2007)]
        public uint Hour { get; set; }
        [Storage("分", 2009)]
        public uint Minute { get; set; }
        [Storage("秒", 2011)]
        public uint Second { get; set; }

    }
}
