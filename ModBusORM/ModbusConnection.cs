using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public class ModbusConnection
    {
        /// <summary>
        /// 通讯口类型
        /// </summary>
        public ComType ComType=ComType.SerialPort;

        /// <summary>
        /// 串口号--COM1
        /// </summary>
        public string ComNo="COM1";

        /// <summary>
        /// 波特率 38400
        /// </summary>
        public int Baud=38400;

        /// <summary>
        /// 数据位
        /// </summary>
        public int DataBit=8;

        /// <summary>
        /// 校验位
        /// </summary>
        public int ParityBit=0;

        /// <summary>
        /// 停止位
        /// </summary>
        public int StopBit=1;

        /// <summary>
        /// 用于网口通信的IP地址
        /// </summary>
        public string IP="127.0.0.1";

        /// <summary>
        /// 用于网口通信的端口号
        /// </summary>
        public int Port=502;

        /// <summary>
        /// 用于表示通信的接收超时
        /// </summary>
        public int TimeOut=500;

        public ICom Com;

        public bool Open()
        {
            _createCom();
            return Com.Open();
        }

        public bool Close()
        {
            return Com.Close();
        }

        public void Register(IDevice dev)
        {
            dev.Register(this);
        }

        private void _createCom()
        {
            switch (this.ComType)
            {
                case ComType.SerialPort:
                    {
                        Com = new SerialPort(this);
                        break;
                    }
                case ComType.IPPort:
                    {
                        Com = new IPPort(this);
                        break;
                    }
            }
        }



    }
}
