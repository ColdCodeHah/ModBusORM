using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusORM
{
    public class ModbusTCP : IProtocol
    {
        public bool CheckReceiveData(byte[] data)
        {
            return ModBusHelper.CheckTCPLength(data);
        }

        public byte[] GetCutReceiveData(byte[] byRead)
        {
            return ConvertHelper.GetArrayByPos(byRead, 9, byRead.Length - 9);
        }

        public byte[] GetRead03Data(byte addrNo, int beginPos, string elementCount, byte crcType,ref ushort tcpFlag)
        {
            tcpFlag++;
            return ModBusHelper.GetTCPRead03Data(addrNo, beginPos, elementCount, tcpFlag);
        }

        public byte[] GetWrite06Data(byte addrNo, int beginPos, string value, ModBusHelper.DataType dataType, byte byteOrder, byte crcType,ref ushort tcpFlag)
        {
            tcpFlag++;
            return ModBusHelper.GetTCPWrite06Data(addrNo, beginPos, value, dataType, byteOrder, tcpFlag);
        }

        public byte[] GetWrite10Data(byte addrNo, int beginPos, string elementCount, byte dataLength, object[][] value, byte byteOrder, byte crcType,ref ushort tcpFlag)
        {
            tcpFlag++;
            return ModBusHelper.GetTCPWrite10Data(addrNo, beginPos, elementCount, dataLength, value, byteOrder, tcpFlag);            
        }

        public byte[] ReadReturnRead(ICom com, int byteSize)
        {
            return com.Read(byteSize + 9);
        }

        public byte[] ReadReturnWrite(ICom com)
        {
            return com.Read(12);
        }

        public T ToSingleField<T>(byte[] byRead, byte byteOrder, int byteSize)
        {
            return (T)ConvertHelper.BytesToStuct(ModBusHelper.GetDataByte(byRead, byteOrder, 9, byteSize), typeof(T));
        }

        public object ToWriteReturn(byte[] byData, byte byteOrder, Type type)
        {
            return ConvertHelper.BytesToStuct(ModBusHelper.GetDataByte(byData, byteOrder, 10, 2), type);
        }
    }
}
