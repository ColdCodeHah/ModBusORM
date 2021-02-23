using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ModBusORM
{
    public class IPPort : Socket,ICom
    {
        private ModbusConnection _con;
        public IPPort(ModbusConnection con):base(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        {
            _con = con;
            if (con.TimeOut != 0)
                TimeOut = con.TimeOut;
            base.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, TimeOut);
        }

        private int _timeOut = 1000;
        public int TimeOut { get => _timeOut; set => _timeOut = value; }

        private string _comName = "";
        public string ComName { get => _comName; set => _comName = value; }


        public new bool Close()
        {
            try
            {
                base.Close();                
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Open()
        {
            try
            {
                base.Connect(IPAddress.Parse(_con.IP), _con.Port);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public byte[] Read(int length)
        {
            byte[] by = new byte[length];
            try
            {
                if (base.Connected)
                {
                    base.Receive(by);
                }
                return by;
            }
            catch
            {
                return null;
            }
        }


        public void Write(byte[] buffer)
        {
            try
            {
                if (base.Connected)
                {
                    base.Send(buffer);
                }
            }
            catch
            {

            }
        }

    }
}
