using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static ModBusORM.ModBusHelper;

namespace ModBusORM
{
    public interface IProtocol
    {
        byte[] GetRead03Data(byte addrNo, int beginPos, string elementCount, byte crcType,ref ushort tcpFlag);

        byte[] ReadReturnRead(ICom com, int byteSize);

        byte[] ReadReturnWrite(ICom com);

        bool CheckReceiveData(byte[] data);

        T ToSingleField<T>(byte[] byRead,byte byteOrder,int byteSize);

        object ToWriteReturn(byte[] byData,byte byteOrder,Type type);

        byte[] GetCutReceiveData(byte[] byRead);

        byte[] GetWrite06Data(byte addrNo, int beginPos, string value, DataType dataType, byte byteOrder, byte crcType,ref ushort tcpFlag);

        byte[] GetWrite10Data(byte addrNo, int beginPos, string elementCount, byte dataLength, object[][] value, byte byteOrder, byte crcType,ref ushort tcpFlag);

    }
}
