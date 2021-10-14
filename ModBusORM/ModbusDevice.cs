using ModBusORM.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;

namespace ModBusORM
{
    public class ModbusDevice<TStorage>: IDevice where TStorage:struct
    {
        /// <summary>
        /// 设备地址位
        /// </summary>
        public byte DeviceAddress=1;

        /// <summary>
        /// 字节顺序 //0--0123 1-1032  2-2301 3-3210
        /// </summary>
        public byte ByteOrder=0;

        /// <summary>
        /// CRC类型 0 1
        /// </summary>
        public byte CRCType=0;        

        private ushort _tcpFlag = 0;

        private ModbusConnection _con;

        private IProtocol _iProtocol;

        void IDevice.Register(ModbusConnection con)
        {
            _con = con;
            switch (con.ComType)
            {
                case ComType.SerialPort:_iProtocol = new ModbusRTU();break;
                case ComType.IPPort:_iProtocol = new ModbusTCP();break;
            }
        }

        public ModbusDevice()
        {
            
        }

        /// <summary>
        /// 读取指定寄存器，阻塞的
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="dev"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public TResult Read<TResult>(Expression<Func<TStorage, TResult>> positions)
        {
            ready();
            var pro = positions.GetProperty();
            var attr = pro.GetAttribute<StorageAttribute>();
            if (attr != null)
            {
                var byteSize = typeof(TResult).GetByteSize();
                var storageSize = Math.Ceiling((float)byteSize / 2);

                byte[] bySend = _iProtocol.GetRead03Data(DeviceAddress,attr.Addr,storageSize.ToString(),CRCType,ref _tcpFlag);

                _con.Com.Write(bySend);

                byte[] byRead = _iProtocol.ReadReturnRead(_con.Com,byteSize);

                if(_iProtocol.CheckReceiveData(byRead))
                {
                    return _iProtocol.ToSingleField<TResult>(byRead,ByteOrder,byteSize);
                }
            }
            else
            {
                throw new ComException("结构体属性未指定StorageAttribute特性");
            }
            throw new ComException("未读取到数据");
        }

        /// <summary>
        /// 读取地址表所有寄存器，阻塞的
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public TStorage Read()
        {
            ready();
            var type = typeof(TStorage);
            List<byte> packReceive = new List<byte>();
            //以一次读取120个字
            var packLength = 120;
            var packSends = type.SendPackaging(packLength);
            var storages = type.SerializeStorage();
            if (packSends?.Count > 0)
            {
                var storageOffset = 0;
                foreach (var s in packSends)
                {
                    byte[] bySend = _iProtocol.GetRead03Data(DeviceAddress,s.Start,s.StorageSize.ToString(),CRCType,ref _tcpFlag);
                    _con.Com.Write(bySend);

                    byte[] byRead = _iProtocol.ReadReturnRead(_con.Com,s.StorageSize*2);
                    if (_iProtocol.CheckReceiveData(byRead))
                    {
                        var byValue = _iProtocol.GetCutReceiveData(byRead);
                        List<byte> lstValue = new List<byte>();
                        var startAddr = storages[storageOffset].Addr;
                        for(var i = storageOffset; i < s.FieldCount+storageOffset; i++)
                        {
                            var offset = (storages[i].Addr - startAddr) * 2;
                            lstValue.AddRange(ConvertHelper.GetArrayByPos(byValue,offset,storages[i].FieldLength));
                        }
                        storageOffset += s.FieldCount;
                        packReceive.AddRange(lstValue);
                    }

                    Thread.Sleep(5);
                }
                if (packReceive.Count == type.GetByteSize())
                {
                    packReceive = ModBusHelper.ExchangeOrder(packReceive, 0, ByteOrder, type);
                    byte[] byRe = packReceive.ToArray();
                    return (TStorage)ConvertHelper.BytesToStuct(byRe, type);
                }
            }

            throw new ComException("未读取到数据");
        }

        /// <summary>
        /// 指定寄存器写入操作，阻塞的
        /// </summary>
        /// <param name="dev"></param>
        /// <param name="positions"></param>
        /// <returns>单寄存器返回写入成功的值，多寄存器返回写入成功寄存器的个数，失败返回null</returns>
        public object Write<TResult>(TStorage storage, Expression<Func<TStorage, TResult>> positions)
        {
            var prop = positions.GetProperty();
            var value = prop.GetValue(storage, null);
            return _write(prop, value);
        }

        /// <summary>
        /// 写单个参数，指定参数值
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        public object Write<TResult>(object value, Expression<Func<TStorage, TResult>> positions)
        {
            var prop = positions.GetProperty();
            return _write(prop, value);
        }

        public object Write(object value,string paraName)
        {
            var prop = typeof(TStorage).GetProperty(paraName);
            if (prop != null)
                return _write(prop, value);
            throw new ComException($"结构体不存在{paraName}属性");
        }

        private object _write(PropertyInfo prop,object value)
        {
            ready();
            object returnValue = null;
            var attr = prop.GetAttribute<StorageAttribute>();
            if (attr == null)
                throw new ComException("结构体属性未指定StorageAttribute特性");

            var byteSize = prop.PropertyType.GetByteSize();
            var storageSize = (int)Math.Ceiling((float)byteSize / 2);

            byte[] bySend = null;
            if (storageSize < 2)//写单个寄存器
            {
                bySend = _iProtocol.GetWrite06Data(DeviceAddress, attr.Addr, value.ToString(), ModBusHelper.convertDataType(prop.PropertyType), ByteOrder, CRCType, ref _tcpFlag);
            }
            else
            {
                object[][] v = new object[1][];
                v[0] = new object[] { ModBusHelper.convertDataType(prop.PropertyType), value };
                bySend = _iProtocol.GetWrite10Data(DeviceAddress, attr.Addr, storageSize.ToString(), (byte)byteSize, v, ByteOrder, CRCType, ref _tcpFlag);
            }
            _con.Com.Write(bySend);
            byte[] byRe = _iProtocol.ReadReturnWrite(_con.Com);
            if (_iProtocol.CheckReceiveData(byRe))
            {
                if (storageSize < 2)
                {
                    returnValue = _iProtocol.ToWriteReturn(byRe, ByteOrder, prop.PropertyType);
                }
                else
                {
                    returnValue = _iProtocol.ToWriteReturn(byRe, 1, typeof(UInt16));
                }
            }
            return returnValue;
        }

        private void ready()
        {
            if (_con==null || _iProtocol == null)
                throw new ComException("设备未注册");
        }

    }
}
