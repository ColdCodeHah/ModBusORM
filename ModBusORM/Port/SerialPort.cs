using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;

namespace ModBusORM
{
    public class SerialPort : System.IO.Ports.SerialPort, ICom
    {
        private int _timeOut = 500;
        public int TimeOut { get => _timeOut; set => _timeOut = value; }

        private string _comName = "";
        public string ComName { get => _comName; set => _comName=value; }

        private int _tick = 5;

        public SerialPort(ModbusConnection con)
        {
            base.PortName = con.ComNo.Contains("COM") ? con.ComNo : "COM" + con.ComNo;//串口
            base.BaudRate = con.Baud;//波特率
            base.DataBits = con.DataBit;//数据位
            base.StopBits = (StopBits)con.StopBit;//停止位
            base.Parity = (Parity)con.ParityBit;//校验位        
            if (con.TimeOut != 0)
                TimeOut = con.TimeOut;
        }

        public byte[] Read(int length)
        {
            try
            {
                byte[] byData = new byte[length];                
                var waitCount = TimeOut / _tick;
                for (int i = 0; i < waitCount; i++)
                {
                    if (base.BytesToRead >= length)
                    {
                        base.Read(byData, 0, length);
                        //清空接收缓冲区
                        base.DiscardInBuffer();
                        return byData;
                    }
                    Thread.Sleep(_tick);
                }
                base.DiscardInBuffer();
                return null;
            }
            catch
            {
                base.DiscardInBuffer();
                return null;
            }
        }

        public void Write(byte[] buffer)
        {
            try
            {
                //清空发送缓冲区
                base.DiscardOutBuffer();
                base.Write(buffer, 0, buffer.Length);
            }
            catch (Exception ex)
            {
                base.DiscardOutBuffer();
                throw ex;
            }
        }

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

        public new bool Open()
        {
            try
            {
                base.Open();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
