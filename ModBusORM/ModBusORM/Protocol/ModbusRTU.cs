using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public class ModbusRTU : IProtocol
    {
        public bool CheckReceiveData(byte[] data)
        {
            return ModBusHelper.CheckCRC(data);
        }

        public byte[] GetCutReceiveData(byte[] byRead)
        {
            return ConvertHelper.GetArrayByPos(byRead, 3, byRead.Length - 5);
        }

        public byte[] GetRead03Data(byte addrNo, int beginPos, string elementCount, byte crcType,ref ushort tcpFlag)
        {
            return ModBusHelper.GetRead03Data(addrNo, beginPos, elementCount, crcType);
        }

        public byte[] GetWrite06Data(byte addrNo, int beginPos, string value, ModBusHelper.DataType dataType, byte byteOrder, byte crcType,ref ushort tcpFlag)
        {
            return ModBusHelper.GetWrite06Data(addrNo, beginPos, value, dataType, byteOrder, crcType);
        }

        public byte[] GetWrite10Data(byte addrNo, int beginPos, string elementCount, byte dataLength, object[][] value, byte byteOrder, byte crcType,ref ushort tcpFlag)
        {
            return ModBusHelper.GetWrite10Data(addrNo, beginPos, elementCount, dataLength, value, byteOrder, crcType);
        }

        public byte[] ReadReturnRead(ICom com, int byteSize)
        {
            return com.Read(byteSize + 5);            
        }

        public byte[] ReadReturnWrite(ICom com)
        {
            return com.Read(8);
        }

        public T ToSingleField<T>(byte[] byRead, byte byteOrder, int byteSize)
        {
            return (T)ConvertHelper.BytesToStuct(ModBusHelper.GetDataByte(byRead, byteOrder, 3, byteSize), typeof(T));
        }

        public object ToWriteReturn(byte[] byData, byte byteOrder, Type type)
        {
            return ConvertHelper.BytesToStuct(ModBusHelper.GetDataByte(byData, byteOrder, 4, 2), type);
        }
    }
}
