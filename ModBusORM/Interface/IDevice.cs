﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public interface IDevice
    {
        void Register(ModbusConnection con);
    }
}
