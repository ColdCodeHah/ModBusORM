using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public class StorageAddr
    {
        public string FieldName { get; set; }

        public int Addr { get; set; }

        public string Title { get; set; }

        public int FieldLength { get; set; }

        public Type FieldType { get; set; }
        public bool IgnoreRead { get; set; }
    }
}
