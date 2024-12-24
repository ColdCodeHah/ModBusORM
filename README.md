# ModBusORM
使用Modbus协议（支持RTU和TCP）对4区输出寄存器进行读写操作，将寄存器表映射成对象，像操作数据库表一样操作寄存器表地址。

就如同数据库ORM一样，以操作对象的方式处理Modbus4区输出寄存器地址表。

## 使用方法

1、新建寄存器地址表结构体
```C#
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct Device1
{
    [Storage("材质系数", 1001)]        
    public float One { get; set; }
    [Storage("初始厚度", 1003)]
    public float Two { get; set; }

    [Storage("年", 2001)]
    public uint Year { get; set; }
    [Storage("月", 2003)]
    public uint Month { get; set; }
    [Storage("日", 2005)]
    public uint Day { get; set; }
    [Storage("时", 2007)]
    public uint Hour { get; set; }
    [Storage("分", 2009)]
    public uint Minute { get; set; }
    [Storage("秒", 2011)]
    public uint Second { get; set; }


}
```
2、创建连接
```C#
ModbusConnection con = new ModbusConnection
{
    ComNo = "COM2",
    ComType=ComType.SerialPort
};
con.Open();
```

3、创建设备，一个连接可以有多个设备，为设备分配地址表
```C#
ModbusDevice<Device1> dev = new ModbusDevice<Device1>();
```

4、将设备注册到连接
```C#
con.Register(dev);
```

```C#
//读所有寄存器地址
var device1 = dev.Read();
//读单个字段
var year = dev.Read(x => x.Year);
//写入寄存器
dev.Write(new Device1 { Year = 2021 }, x => x.Year);
```
