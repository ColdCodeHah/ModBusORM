using ModBusORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Program
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct SampleEntity
    {
        [Storage("质量流量",247)]
        public float MassFlow { get; set; }
        [Storage("密度", 249)]
        public float Density { get; set; }
        [Storage("温度", 251)]
        public float Temperature { get; set; }
        [Storage("体积流量", 253)]
        public float VolumeFlow { get; set; }
        [Storage("备用", 255)]
        public float Empty1 { get; set; }
        [Storage("流量计压力", 257)]
        public float FlowPressure{ get; set; }
        [Storage("质量累计", 259)]
        public float MassTotal { get; set; }
        [Storage("体积累计", 260)]
        public float VolumeTotal { get; set; }
        [Storage("装置压力", 10035)]
        public float Pressure { get; set; }
    }   
}
