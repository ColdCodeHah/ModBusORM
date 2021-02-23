using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public interface ICom
    {
        string ComName { get; set; }

        int TimeOut { get; set; }

        bool Open();

        bool Close();

        void Write(byte[] buffer);

        byte[] Read(int length);
    }

    
}
