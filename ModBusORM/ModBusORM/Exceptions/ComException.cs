using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public class ComException: ApplicationException
    {

        private string _msg = "";

        public ComException(string msg="")
        {
            _msg = msg;
        }

        public override string Message => _msg;
    }
}
