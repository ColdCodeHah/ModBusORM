using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ModBusORM
{

    /// <summary>
    /// 处理数据类型转换，数制转换、编码转换相关的类
    /// </summary>    
    public sealed class ConvertHelper
    {
        #region 补足位数
        /// <summary>
        /// 指定字符串的固定长度，如果字符串小于固定长度，
        /// 则在字符串的前面补足零，可设置的固定长度最大为9位
        /// </summary>
        /// <param name="text">原始字符串</param>
        /// <param name="limitedLength">字符串的固定长度</param>
        public static string RepairZero(string text, int limitedLength)
        {
            //补足0的字符串
            string temp = "";

            //补足0
            for (int i = 0; i < limitedLength - text.Length; i++)
            {
                temp += "0";
            }

            //连接text
            temp += text;

            //返回补足0的字符串
            return temp;
        }
        #endregion

        #region 各进制数间转换
        /// <summary>
        /// 实现各进制数间的转换。ConvertBase("15",10,16)表示将十进制数15转换为16进制的数。
        /// </summary>
        /// <param name="value">要转换的值,即原值</param>
        /// <param name="from">原值的进制,只能是2,8,10,16四个值。</param>
        /// <param name="to">要转换到的目标进制，只能是2,8,10,16四个值。</param>
        public static string ConvertBase(string value, int from, int to)
        {
            try
            {
                int intValue = Convert.ToInt32(value, from);  //先转成10进制
                string result = Convert.ToString(intValue, to);  //再转成目标进制
                if (to == 2)
                {
                    int resultLength = result.Length;  //获取二进制的长度
                    switch (resultLength)
                    {
                        case 7:
                            result = "0" + result;
                            break;
                        case 6:
                            result = "00" + result;
                            break;
                        case 5:
                            result = "000" + result;
                            break;
                        case 4:
                            result = "0000" + result;
                            break;
                        case 3:
                            result = "00000" + result;
                            break;
                    }
                }
                return result;
            }
            catch
            {

                //LogHelper.WriteTraceLog(TraceLogLevel.Error, ex.Message);
                return "0";
            }
        }

        /// <summary>
        /// 十进制转十六进制
        /// </summary>
        /// <returns></returns>
        public static string DecimalToHex(int iDecimal)
        {
            return Convert.ToString(iDecimal, 16);
        }

        /// <summary>
        /// 十进制转二进制
        /// </summary>
        /// <param name="iDecimal">十进制数</param>
        /// <param name="bLength">位数</param>
        /// <returns>二进制</returns>
        public static string DecimalToBinary(int iDecimal, byte bLength)
        {

            string strBinary = Convert.ToString(iDecimal, 2);
            if (bLength == 0)
            {
                return strBinary;
            }
            else
            {
                int iLength = Math.Abs(bLength - strBinary.Length);
                if (iLength != 0)
                {
                    for (int i = 0; i <= iLength - 1; i++)
                    {
                        strBinary = "0" + strBinary;
                    }
                }
                return strBinary;
            }

        }

        /// <summary>
        /// 十六进制转十进制
        /// </summary>
        /// <param name="strHex"></param>
        /// <returns></returns>
        public static int HexToDecimal(string strHex)
        {
            return int.Parse(strHex.Replace(" ", ""), System.Globalization.NumberStyles.HexNumber);
        }

        /// <summary>
        /// 二进制转十进制
        /// </summary>
        /// <param name="strBinary"></param>
        /// <returns></returns>
        public static int BinaryToDecimal(string strBinary)
        {
            return Convert.ToInt32(strBinary, 2);

        }

        /// <summary>
        /// Byte转二进制字符串
        /// </summary>
        /// <returns></returns>
        public static string ByteToBinaryString(byte value)
        {
            string s = Convert.ToString(value, 2);
            while (s.Length < 8)
            {
                s = "0" + s;
            }
            return s;
        }

        #endregion

        #region 使用指定字符集将string转换成byte[]
        /// <summary>
        /// 使用指定字符集将string转换成byte[]
        /// </summary>
        /// <param name="text">要转换的字符串</param>
        /// <param name="encoding">字符编码</param>
        public static byte[] StringToBytes(string text, Encoding encoding)
        {
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// 16进制字符串转换成字节数组
        /// </summary>
        /// <returns></returns>
        public static byte[] HexStringToByteArray(string s, int iLength)
        {
            if (s.Length % 2 != 0)
            {
                s = "0" + s;
            }
            s = s.Replace(" ", "");
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < s.Length; i += 2)
            {
                buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
            }

            byte[] bytesResult = new byte[iLength];
            if (buffer.Length < iLength)
            {
                for (int i = 0; i <= (iLength - buffer.Length) - 1; i++)
                {
                    bytesResult[i] = 0x0;
                }
                for (int i = iLength - buffer.Length; i <= iLength - 1; i++)
                {
                    bytesResult[i] = buffer[i - (iLength - buffer.Length)];
                }
                return bytesResult;
            }
            else
            {
                return buffer;
            }
        }
        #endregion

        #region 使用指定字符集将byte[]转换成string
        /// <summary>
        /// 使用指定字符集将byte[]转换成string
        /// </summary>
        /// <param name="bytes">要转换的字节数组</param>
        /// <param name="encoding">字符编码</param>
        public static string BytesToString(byte[] bytes, Encoding encoding)
        {
            return encoding.GetString(bytes);
        }

        /// <summary> 
        /// 字节数组转换成16进制字符串
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
            {
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0').PadRight(3, ' '));
            }
            return sb.ToString().Trim().ToUpper();
        }


        #endregion

        #region 将byte[]转换成int
        /// <summary>
        /// 将byte[]转换成int
        /// </summary>
        /// <param name="data">需要转换成整数的byte数组</param>
        public static int BytesToInt32(byte[] data)
        {
            //如果传入的字节数组长度小于4,则返回0
            if (data.Length < 4)
            {
                return 0;
            }

            //定义要返回的整数
            int num = 0;

            //如果传入的字节数组长度大于4,需要进行处理
            if (data.Length >= 4)
            {
                //创建一个临时缓冲区
                byte[] tempBuffer = new byte[4];

                //将传入的字节数组的前4个字节复制到临时缓冲区
                Buffer.BlockCopy(data, 0, tempBuffer, 0, 4);

                //将临时缓冲区的值转换成整数，并赋给num
                num = BitConverter.ToInt32(tempBuffer, 0);
            }

            //返回整数
            return num;
        }
        #endregion

        /// <summary>
        /// 十进制转BCD
        /// </summary>
        /// <param name="iDecimal"></param>
        /// <param name="bLength"></param>
        /// <param name="bType">0压缩   1非压缩</param>
        /// <returns></returns>
        public static string DecimalToBCD(int iDecimal, byte bType)
        {
            byte bBcdType = 4;
            if (bType == 0)
            {
                //压缩BCD
                bBcdType = 4;
            }
            else
            {
                //非压缩BCD
                bBcdType = 8;
            }
            string strDecimal = iDecimal.ToString();
            string strBCD = "";

            for (int i = 0; i <= strDecimal.Length - 1; i++)
            {
                strBCD = strBCD + DecimalToBinary(int.Parse(strDecimal[i].ToString()), bBcdType);
            }
            return strBCD;
        }

        /// <summary>
        /// BCD转十进制
        /// </summary>
        /// <param name="strBcd"></param>
        /// <param name="bType"></param>
        /// <returns></returns>
        public static int BCDToDecimal(string strBcd, byte bType)
        {
            byte bBcdType = 4;
            if (bType == 0)
            {
                //压缩BCD
                bBcdType = 4;
            }
            else
            {
                //非压缩BCD
                bBcdType = 8;
            }
            string strBCD = "";
            for (int i = 0; i <= strBcd.Length - 1; i++)
            {
                if ((i + 1) % bBcdType == 0)
                {
                    strBCD = strBCD + BinaryToDecimal(strBcd.Substring(i - bBcdType + 1, bBcdType)).ToString();
                }
            }
            return int.Parse(strBCD.ToString());
        }



        ///<summary>
        ///CRC校验
        ///</summary>
        public static byte[] CRC16(byte[] data, int crcType)
        {
            if (data.Length == 0)
                throw new Exception("调用CRC16校验算法,（低字节在前，高字节在后）时发生异常，异常信息：被校验的数组长度为0。");
            byte[] temdata = new byte[data.Length + 2];
            int xda, xdapoly;
            byte i, j, xdabit;
            xda = 0xFFFF;
            xdapoly = 0xA001;
            for (i = 0; i < data.Length; i++)
            {
                xda ^= data[i];
                for (j = 0; j < 8; j++)
                {
                    xdabit = (byte)(xda & 0x01);
                    xda >>= 1;
                    if (xdabit == 1)
                        xda ^= xdapoly;
                }
            }
            if (crcType == 0)  //低--高
            {
                temdata = new byte[2] { (byte)(xda & 0xFF), (byte)(xda >> 8) };
            }
            else  //高--低
            {
                temdata = new byte[2] { (byte)(xda >> 8), (byte)(xda & 0xFF) };
            }
            return temdata;
        }


        /// <summary>
        /// 获取数组
        /// </summary>
        /// <param name="bytes">需要截取的数组</param>
        /// <param name="beginPos">开始位</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        public static byte[] GetArrayByPos(byte[] bytes, int beginPos, int length)
        {
            try
            {
                byte[] data = new byte[length];

                for (int i = 0; i < length; i++)
                {
                    data[i] = bytes[beginPos + i];
                }
                return data;
            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsInt(string value)
        {
            int i;
            return int.TryParse(value, out i);
        }


        /// <summary>
        /// 是否是正整数
        /// </summary>
        public static bool IsUInt(string value)
        {
            uint uI;
            return uint.TryParse(value, out uI);
        }


        /// <summary>
        /// 是否是Byte类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsByte(string value)
        {
            byte b;
            return byte.TryParse(value, out b);
        }

        /// <summary>
        /// 是否是浮点型
        /// </summary>
        public static bool IsFloat(string value)
        {
            float f;
            return float.TryParse(value, out f);
        }

        /// <summary>
        /// 是否是小数类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDecimal(string value)
        {
            decimal d;
            return decimal.TryParse(value, out d);
        }

        /// <summary>
        /// 阿拉伯数字转汉字
        /// </summary>
        /// <param name="type">0 一二  1壹贰</param>
        /// <returns></returns>
        public static string NumberToChinese(int n, byte type=0)
        {
            string num = "0123456789";
            string[] type0 = new string[] { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] type1 = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string strn = n.ToString();
            int len = strn.Length;
            string r = "";
            char[] c = strn.ToArray();
            switch (len)
            {
                case 1:
                    {
                        if (type == 0)
                            r= type0[num.IndexOf(c[0])];
                        else
                            r= type1[num.IndexOf(c[0])];
                        break;
                    }
                case 2:
                    {
                        if (type == 0)
                        {
                            if(!c[0].Equals('1'))
                                r = type0[num.IndexOf(c[0])];
                            r += "十";
                            r+= type0[num.IndexOf(c[1])];
                        }
                        else
                        {
                            if (!c[0].Equals('1'))
                                r = type1[num.IndexOf(c[0])];
                            r += "拾";
                            r += type1[num.IndexOf(c[1])];
                        }
                        break;
                    }
                case 3:
                    {
                        if (type == 0)
                        {
                            r = type0[num.IndexOf(c[0])];
                            r += "百";
                            r += type0[num.IndexOf(c[1])];
                            r += "十";
                            r+= type0[num.IndexOf(c[2])];                            
                        }
                        else
                        {
                            r = type1[num.IndexOf(c[0])];
                            r += "佰";
                            r += type1[num.IndexOf(c[1])];
                            r += "拾";
                            r += type1[num.IndexOf(c[2])];
                        }
                        break;
                    }
                case 4:
                    {
                        if (type == 0)
                        {
                            r = type0[num.IndexOf(c[0])];
                            r += "千";
                            r += type0[num.IndexOf(c[1])];
                            r += "百";
                            r += type0[num.IndexOf(c[2])];
                            r += "十";
                            r += type0[num.IndexOf(c[3])];
                        }
                        else
                        {
                            r = type1[num.IndexOf(c[0])];
                            r += "千";
                            r += type1[num.IndexOf(c[1])];
                            r += "百";
                            r += type1[num.IndexOf(c[2])];
                            r += "十";
                            r += type1[num.IndexOf(c[3])];
                        }
                        break;
                    }
                case 5:
                    {
                        if (type == 0)
                        {
                            r = type0[num.IndexOf(c[0])];
                            r += "万";
                            r += type0[num.IndexOf(c[1])];
                            r += "千";
                            r += type0[num.IndexOf(c[2])];
                            r += "百";
                            r += type0[num.IndexOf(c[3])];
                            r += "十";
                            r += type0[num.IndexOf(c[4])];
                        }
                        else
                        {
                            r = type1[num.IndexOf(c[0])];
                            r += "万";
                            r += type1[num.IndexOf(c[1])];
                            r += "千";
                            r += type1[num.IndexOf(c[2])];
                            r += "百";
                            r += type1[num.IndexOf(c[3])];
                            r += "十";
                            r += type1[num.IndexOf(c[4])];
                        }
                        break;
                    }
            }
            return r;
        }

        /// <summary>
        /// 阿拉伯数字字符转汉字
        /// </summary>
        /// <param name="type">0 一二  1壹贰</param>
        /// <returns></returns>
        public static string NumberStrToChinese(int n, byte type = 0)
        {
            string num = "0123456789";
            string[] type0 = new string[] { "〇", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string[] type1 = new string[] { "零", "壹", "贰", "叁", "肆", "伍", "陆", "柒", "捌", "玖" };
            string strn = n.ToString();
            int len = strn.Length;
            string r = "";
            char[] c = strn.ToArray();
            for(int i = 0; i < c.Length; i++)
            {
                if (type == 0)                
                    r += type0[num.IndexOf(c[i])];     
                else
                    r += type1[num.IndexOf(c[i])];
            }
            return r;
        }

        /// <summary>
        /// 判断是否null 转为string
        /// </summary>
        /// <param name="strTxt"></param>
        /// <returns>如传入值为null 返回为"" 否则返回为obj.string</returns>
        public static string IsNullToString(object strTxt)
        {
            return strTxt == null ? "" : strTxt.ToString().Trim();
        }

        /// <summary>
        /// 字节数组转结构体
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        /// <summary>
        /// 结构体转byte数组
        /// </summary>
        /// <param name="objstuct">结构体</param>
        /// <returns>byte数组</returns>
        public static byte[] StuctToBytes(object objstuct)
        {
            //得到结构体大小
            int size = Marshal.SizeOf(objstuct);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体copy到分配好的内存空间内
            Marshal.StructureToPtr(objstuct, structPtr, false);
            //从内存空间copy到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return bytes;
        }
    }
}
