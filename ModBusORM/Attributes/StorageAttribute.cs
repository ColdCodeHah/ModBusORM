using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StorageAttribute: Attribute
    {
        public StorageAttribute(string name,int addr)
        {
            Name = name;
            Addr = addr;
        }
        public int Addr { get; set; }
        public string Name { get; set; }
    }
}
