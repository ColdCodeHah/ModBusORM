using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

namespace ModBusORM
{
    public class ModBusHelper
    {

        public enum ModBusCode : byte
        {
            Read,

            Write06,

            Write16
        }

        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataType : byte
        {
            /// <summary>
            /// 浮点型
            /// </summary>
            FloatType,

            /// <summary>
            /// 有符号整形32位
            /// </summary>
            IntType,

            /// <summary>
            /// 无符号整形32位
            /// </summary>
            UIntType,


            /// <summary>
            /// 有符号整形16位
            /// </summary>
            Int16Type,

            /// <summary>
            /// 无符号整形16位
            /// </summary>
            UInt16Type,


            /// <summary>
            /// 无符号Byte类型
            /// </summary>
            UByteType,

        }

        /// <summary>
        /// 获得读03功能码命令数组
        /// </summary>
        /// <param name="addressNo">地址位0-255</param>
        /// <param name="beginPos">开始位0-65535</param>
        /// <param name="elementCount">寄存器个数0-65535</param>
        /// <param name="crcType">crc位顺序，0低到高，1高到低</param>
        /// <returns></returns>
        public static byte[] GetRead03Data(byte addressNo, int beginPos, string elementCount, byte crcType)
        {
            byte[] data = new byte[8];
            data[0] = addressNo;//地址位
            byte[] elementCountData = null;
            byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
            data[1] = 3;//功能码 
            elementCountData = BitConverter.GetBytes(Convert.ToUInt16(elementCount));
            data[2] = beginData[1];
            data[3] = beginData[0];
            data[4] = elementCountData[1];
            data[5] = elementCountData[0];
            byte[] crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(data, 0, 6), crcType);//CRC校验
            data[6] = crcBytes[0];
            data[7] = crcBytes[1];
            return data;
        }

        /// <summary>
        /// 获取读ModbusTCP03功能码命令
        /// </summary>
        /// <param name="addressNo"></param>
        /// <param name="beginPos"></param>
        /// <param name="elementCount"></param>
        /// <param name="flag">报文头标识</param>
        /// <returns></returns>
        public static byte[] GetTCPRead03Data(byte addressNo, int beginPos, string elementCount, UInt16 flag)
        {
            byte[] data = new byte[12];
            byte[] h1 = BitConverter.GetBytes(flag);
            byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
            byte[] elementCountData = BitConverter.GetBytes(Convert.ToUInt16(elementCount));
            data[0] = h1[1];
            data[1] = h1[0];
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x06;
            data[6] = addressNo;
            data[7] = 0x03;
            data[8] = beginData[1];
            data[9] = beginData[0];
            data[10] = elementCountData[1];
            data[11] = elementCountData[0];
            return data;
        }

        /// <summary>
        /// 获得写06功能码命令数组
        /// </summary>
        /// <param name="addressNo">地址位0-255</param>
        /// <param name="beginPos">开始位0-65535</param>
        /// <param name="value">要写入的单个数据</param>
        /// <param name="dataType">要写入的数据类型</param>
        /// <param name="crcType">crc位顺序，0低到高，1高到低</param>
        /// <returns></returns>
        public static byte[] GetWrite06Data(byte addressNo, int beginPos, string value, DataType dataType, byte byteOrder, byte crcType)
        {
            byte[] data;
            if (dataType == DataType.Int16Type || dataType == DataType.UInt16Type)
            {
                data = new byte[8];
                data[0] = addressNo;//地址位
                data[1] = 6;//功能码 
                byte[] valueData = null;
                byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
                if (dataType == DataType.Int16Type)
                    valueData = BitConverter.GetBytes(Convert.ToInt16(value));
                else
                    valueData = BitConverter.GetBytes(Convert.ToUInt16(value));
                data[2] = beginData[1];
                data[3] = beginData[0];//字节顺序0-1
                if (byteOrder == 3)
                {
                    data[4] = valueData[1];
                    data[5] = valueData[0];
                }
                else
                {
                    data[4] = valueData[0];
                    data[5] = valueData[1];
                }
                byte[] crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(data, 0, 6), crcType);//CRC校验
                data[6] = crcBytes[0];
                data[7] = crcBytes[1];
            }
            else if (dataType == DataType.UByteType)
            {
                data = new byte[8];
                data[0] = addressNo;//地址位
                data[1] = 6;//功能码 
                byte[] valueData = null;
                byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
                valueData = BitConverter.GetBytes(Convert.ToByte(value));
                data[2] = beginData[1];
                data[3] = beginData[0];
                data[4] = 0;
                data[5] = valueData[0];
                byte[] crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(data, 0, 6), crcType);//CRC校验
                data[6] = crcBytes[0];
                data[7] = crcBytes[1];
            }
            else
            {
                data = new byte[10];
                data[0] = addressNo;//地址位
                data[1] = 6;//功能码 
                byte[] valueData = null;
                byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
                if (dataType == DataType.FloatType)
                    valueData = BitConverter.GetBytes(Convert.ToSingle(value));
                else if (dataType == DataType.IntType)
                    valueData = BitConverter.GetBytes(Convert.ToInt32(value));
                else
                    valueData = BitConverter.GetBytes(Convert.ToUInt32(value));
                data[2] = beginData[1];
                data[3] = beginData[0];
                data[4] = valueData[0];
                data[5] = valueData[1];
                data[6] = valueData[2];
                data[7] = valueData[3];
                byte[] crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(data, 0, 6), crcType);//CRC校验
                data[8] = crcBytes[0];
                data[9] = crcBytes[1];
            }
            return data;
        }

        /// <summary>
        /// 获得TCP写06功能码命令数组
        /// </summary>
        /// <param name="addressNo"></param>
        /// <param name="beginPos"></param>
        /// <param name="value"></param>
        /// <param name="dataType"></param>
        /// <param name="byteOrder"></param>
        /// <param name="flag">报文头标识</param>
        /// <returns></returns>
        public static byte[] GetTCPWrite06Data(byte addressNo, int beginPos, string value, DataType dataType, byte byteOrder, UInt16 flag)
        {
            byte[] data = null;
            byte[] h1 = BitConverter.GetBytes(flag);
            byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
            if (dataType == DataType.Int16Type || dataType == DataType.UInt16Type)
            {
                data = new byte[12];
                byte[] valueData = null;
                if (dataType == DataType.Int16Type)
                    valueData = BitConverter.GetBytes(Convert.ToInt16(value));
                else
                    valueData = BitConverter.GetBytes(Convert.ToUInt16(value));
                data[0] = h1[1];
                data[1] = h1[0];
                data[2] = 0x00;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x06;
                data[6] = addressNo;
                data[7] = 0x06;
                data[8] = beginData[1];
                data[9] = beginData[0];
                if (byteOrder == 1 || byteOrder == 3)
                {
                    data[10] = valueData[1];
                    data[11] = valueData[0];
                }
                else
                {
                    data[10] = valueData[0];
                    data[11] = valueData[1];
                }
            }
            else if (dataType == DataType.UByteType)
            {
                data = new byte[12];
                byte[] valueData = null;
                valueData = BitConverter.GetBytes(Convert.ToByte(value));
                data[0] = h1[1];
                data[1] = h1[0];
                data[2] = 0x00;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x06;
                data[6] = addressNo;
                data[7] = 0x06;
                data[8] = beginData[1];
                data[9] = beginData[0];
                data[10] = 0;
                data[11] = valueData[0];
            }
            return data;
        }


        /// <summary>
        /// 获得写10功能码命令数组
        /// </summary>
        /// <param name="addressNo">地址位0-255</param>
        /// <param name="beginPos">开始位0-65535</param>
        /// <param name="elementCount">寄存器数量(最多120个)</param>
        /// <param name="dataLength">要写入的字节数长度</param>
        /// <param name="value">要写入的所有数据及数据类型</param>
        /// <param name="crcType">crc位顺序，0低到高，1高到低</param>
        /// <returns></returns>
        public static byte[] GetWrite10Data(byte addressNo, int beginPos, string elementCount, byte dataLength, object[][] value, byte byteOrder, byte crcType)
        {
            byte[] data = new byte[9 + dataLength];
            data[0] = addressNo;//地址位
            data[1] = 16;
            byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
            data[2] = beginData[1];
            data[3] = beginData[0];

            byte[] elementCountData = BitConverter.GetBytes(Convert.ToUInt16(elementCount));
            data[4] = elementCountData[1];
            data[5] = elementCountData[0];

            data[6] = dataLength;
            List<byte> listValue = new List<byte>();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                if ((DataType)value[i][0] == DataType.UByteType)
                {
                    byte[] tdata = new byte[2];
                    tdata = BitConverter.GetBytes((byte)value[i][1]);
                    Array.Reverse(tdata);
                    listValue.AddRange(tdata);
                }
                else if ((DataType)value[i][0] == DataType.UInt16Type || (DataType)value[i][0] == DataType.Int16Type)
                {
                    byte[] tdata = new byte[2];
                    if ((DataType)value[i][0] == DataType.UInt16Type)
                        tdata = BitConverter.GetBytes((UInt16)value[i][1]);
                    else
                        tdata = BitConverter.GetBytes((Int16)value[i][1]);
                    Array.Reverse(tdata);
                    listValue.AddRange(tdata);
                }
                else if ((DataType)value[i][0] == DataType.FloatType || (DataType)value[i][0] == DataType.UIntType || (DataType)value[i][0] == DataType.IntType)
                {
                    byte[] tdata = new byte[4];
                    if ((DataType)value[i][0] == DataType.FloatType)
                    {
                        float f = float.Parse(value[i][1].ToString());
                        tdata = BitConverter.GetBytes(f);
                    }
                    else if ((DataType)value[i][0] == DataType.UIntType)
                        tdata = BitConverter.GetBytes((uint)value[i][1]);
                    else
                        tdata = BitConverter.GetBytes((int)value[i][1]);
                    tdata = GetDataByte(tdata, byteOrder, 0, 4);
                    listValue.AddRange(tdata);
                }
            }
            byte[] btValue = listValue.ToArray();
            for (int i = 0; i < btValue.Length; i++)
            {
                data[7 + i] = btValue[i];
            }

            byte[] crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(data, 0, 7 + btValue.Length), crcType);//CRC校验

            data[7 + btValue.Length] = crcBytes[0];
            data[8 + btValue.Length] = crcBytes[1];

            return data;
        }

        /// <summary>
        /// 获得TCP写10功能码命令数组
        /// </summary>
        /// <param name="addressNo"></param>
        /// <param name="beginPos"></param>
        /// <param name="elementCount"></param>
        /// <param name="dataLength"></param>
        /// <param name="value"></param>
        /// <param name="byteOrder"></param>
        /// <param name="flag">报文头标识</param>
        /// <returns></returns>
        public static byte[] GetTCPWrite10Data(byte addressNo, int beginPos, string elementCount, byte dataLength, object[][] value, byte byteOrder, UInt16 flag)
        {
            byte[] data = new byte[13 + dataLength];
            byte[] h1 = BitConverter.GetBytes(flag);
            byte[] beginData = BitConverter.GetBytes(Convert.ToUInt16(beginPos - 1));
            byte[] elementCountData = BitConverter.GetBytes(Convert.ToUInt16(elementCount));

            data[0] = h1[1];
            data[1] = h1[0];
            data[2] = 0x00;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x06;

            data[6] = addressNo;
            data[7] = 0x10;

            data[8] = beginData[1];
            data[9] = beginData[0];
            data[10] = elementCountData[1];
            data[11] = elementCountData[0];
            data[12] = dataLength;

            List<byte> listValue = new List<byte>();
            for (int i = 0; i < value.GetLength(0); i++)
            {
                if ((DataType)value[i][0] == DataType.UByteType)
                {
                    byte[] tdata = new byte[2];
                    tdata = BitConverter.GetBytes((byte)value[i][1]);
                    Array.Reverse(tdata);
                    listValue.AddRange(tdata);
                }
                else if ((DataType)value[i][0] == DataType.UInt16Type || (DataType)value[i][0] == DataType.Int16Type)
                {
                    byte[] tdata = new byte[2];
                    if ((DataType)value[i][0] == DataType.UInt16Type)
                        tdata = BitConverter.GetBytes((UInt16)value[i][1]);
                    else
                        tdata = BitConverter.GetBytes((Int16)value[i][1]);
                    Array.Reverse(tdata);
                    listValue.AddRange(tdata);
                }
                else if ((DataType)value[i][0] == DataType.FloatType || (DataType)value[i][0] == DataType.UIntType || (DataType)value[i][0] == DataType.IntType)
                {
                    byte[] tdata = new byte[4];
                    if ((DataType)value[i][0] == DataType.FloatType)
                    {
                        float f = float.Parse(value[i][1].ToString());
                        tdata = BitConverter.GetBytes(f);
                    }
                    else if ((DataType)value[i][0] == DataType.UIntType)
                        tdata = BitConverter.GetBytes((uint)value[i][1]);
                    else
                        tdata = BitConverter.GetBytes((int)value[i][1]);
                    tdata = GetDataByte(tdata, byteOrder, 0, 4);
                    listValue.AddRange(tdata);
                }
            }
            byte[] btValue = listValue.ToArray();
            for (int i = 0; i < btValue.Length; i++)
            {
                data[13 + i] = btValue[i];
            }
            return data;
        }


        /// <summary>
        /// 获取返回数据
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static byte[] GetReplyData(byte[] bytes, byte addressNo, byte crcType)
        {
            byte[] data = null;
            int iBeginPos = -1;//开始位
            for (int i = 0; i <= bytes.Length - 1; i++)
            {
                if ((bytes[i] == addressNo) && (bytes[i + 1] == 03 || bytes[i + 1] == 06 || bytes[i + 1] == 16 || bytes[i + 1] == 1))
                {
                    iBeginPos = i;
                    break;
                }
            }
            if (iBeginPos >= 0)
            {

                byte code = bytes[iBeginPos + 1];
                byte[] crcBytes = null;
                switch (code)
                {
                    case 3:
                        int dataLength = bytes[iBeginPos + 2];
                        crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(bytes, iBeginPos, dataLength + 3), crcType);
                        if ((crcBytes[0] == bytes[iBeginPos + dataLength + 3]) && (crcBytes[1] == bytes[iBeginPos + dataLength + 4]))
                        {
                            data = ConvertHelper.GetArrayByPos(bytes, iBeginPos, dataLength + 5);
                        }
                        break;
                    case 6:
                        crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(bytes, iBeginPos, 6), crcType);
                        if ((crcBytes[0] == bytes[iBeginPos + 6]) && (crcBytes[1] == bytes[iBeginPos + 7]))
                        {
                            data = ConvertHelper.GetArrayByPos(bytes, iBeginPos, 8);
                        }
                        break;
                    case 16:
                        crcBytes = ConvertHelper.CRC16(ConvertHelper.GetArrayByPos(bytes, iBeginPos, 6), crcType);
                        if ((crcBytes[0] == bytes[iBeginPos + 6]) && (crcBytes[1] == bytes[iBeginPos + 7]))
                        {
                            data = ConvertHelper.GetArrayByPos(bytes, iBeginPos, 8);
                        }
                        break;
                    default:
                        break;
                }

            }
            else
            {
                return null;
            }
            return data;
        }

        /// <summary>
        /// 根据字节顺序获取数组
        /// </summary>
        /// <param name="sbytes">源数据</param>
        /// <param name="byteOrder">字节顺序0-0123 1-1032 2-2301 3-3210</param>
        /// <returns>目标数据</returns>
        public static byte[] GetDataByte(byte[] sBytes, byte byteOrder = 0, int beginIndex = 0, int length = 4)
        {
            byte[] dBytes = null;
            if (length == 2)
            {
                dBytes = new byte[2];
                for (int i = 0; i < length; i++)
                {
                    dBytes[i] = sBytes[beginIndex + i];
                }
                switch (byteOrder)
                {
                    case 0://01
                        return dBytes;
                    case 1://10
                        return new byte[] { dBytes[1], dBytes[0] };
                    case 3:
                        return new byte[] { dBytes[1], dBytes[0] };
                    default:
                        return dBytes;
                }
            }
            if (length == 4)
            {
                dBytes = new byte[4];
                for (int i = 0; i < length; i++)
                {
                    dBytes[i] = sBytes[beginIndex + i];
                }
                switch (byteOrder)
                {
                    case 0:
                        return dBytes;
                    case 1:
                        return new byte[] { dBytes[1], dBytes[0], dBytes[3], dBytes[2] };
                    case 2:
                        return new byte[] { dBytes[2], dBytes[3], dBytes[0], dBytes[1] };
                    case 3:
                        return new byte[] { dBytes[3], dBytes[2], dBytes[1], dBytes[0] };
                }
            }
            throw new Exception("长度错误");
        }

        public static bool CheckTCPLength(byte[] by)
        {
            if (by == null || by.Length <= 0)
                return false;
            var length = BitConverter.ToUInt16(GetDataByte(by, 1, 4, 2), 0);
            if (length == by.Length - 6)
                return true;
            return false;
        }

        public static DataType convertDataType(Type t)
        {
            switch (t.Name)
            {
                default:
                case "Single":
                    return DataType.FloatType;
                case "UInt32":
                    return DataType.UIntType;
                case "Int32":
                    return DataType.IntType;
                case "Int16":
                    return DataType.Int16Type;
                case "UInt16":
                    return DataType.UInt16Type;
                case "Byte":
                    return DataType.UByteType;
            }
        }

        public static bool CheckCRC(byte[] by, int crcType = 0)
        {
            if (by == null || by.Length <= 0)
                return false;
            var crcwait = ConvertHelper.GetArrayByPos(by, by.Length - 2, 2);
            var bywait = ConvertHelper.GetArrayByPos(by, 0, by.Length - 2);
            var crc = ConvertHelper.CRC16(bywait, crcType);
            if (crc[0] == crcwait[0] && crc[1] == crcwait[1])
                return true;
            return false;
        }

        public static List<byte> ExchangeOrder(List<byte> by, ushort length, byte order, Type type = null)
        {
            List<byte> list = new List<byte>();
            try
            {
                byte[] arrBy = by.ToArray();
                if (type != null)
                {
                    var fields = type.GetProperties();
                    if (fields.Length > 0)
                    {
                        int i = 0;
                        foreach (var field in fields)
                        {
                            int size = Marshal.SizeOf(field.PropertyType);
                            byte[] temp = GetDataByte(arrBy, order, i, size);
                            list.AddRange(temp);
                            i += size;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < by.Count; i += length)
                    {
                        byte[] temp = GetDataByte(arrBy, order, i, length);
                        list.AddRange(temp);
                    }
                }
                return list;
            }
            catch
            {
                return list;
            }
        }

        /// <summary>
        /// 根据数据描述获取数据类型
        /// </summary>
        /// <param name="_dataDesc"></param>
        /// <returns></returns>
        public static Type GetTypeFromDataDesc(string _dataDesc)
        {
            switch (_dataDesc)
            {
                case "浮点型32位":
                    return typeof(float);
                case "有符号整型32位":
                    return typeof(int);
                case "无符号整型32位":
                    return typeof(uint);
                case "无符号整型16位":
                    return typeof(UInt16);
                case "有符号整型16位":
                    return typeof(Int16);
                case "无符号整型8位":
                    return typeof(byte);
                default:
                    return null;
            }
        }

    }

}
