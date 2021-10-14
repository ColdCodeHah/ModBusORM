using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ModBusORM
{
    public static class TypeExtension
    {
        public static int GetByteSize(this Type type)
        {
            return Marshal.SizeOf(type);
        }

        public static List<ModBusSendTag> SendPackaging(this Type type, int packLength)
        {            
            var props = type.GetProperties();
            List<ModBusSendTag> packSend = new List<ModBusSendTag>();
            if (props.Length > 0)
            {
                //起始寄存器位置
                var attrStar = (StorageAttribute)props[0].GetCustomAttributes(typeof(StorageAttribute), true)[0];
                var packStart = attrStar.Addr;
                packSend.Add(new ModBusSendTag { Start = packStart});

                for (var i = 0; i < props.Length; i++)
                {
                    var attr = (StorageAttribute)props[i].GetCustomAttributes(typeof(StorageAttribute), true)[0];                    
                    var byteSize = props[i].PropertyType.GetByteSize();
                    var storageSize = (int)Math.Ceiling((float)byteSize / 2);
                    if (attr.Addr - packSend.Last().Start + 1 + storageSize > packLength)
                    {
                        packSend.Add(new ModBusSendTag { Start = attr.Addr });
                    }
                    packSend.Last().StorageSize = attr.Addr-packSend.Last().Start+storageSize;
                    packSend.Last().FieldCount += 1;
                    
                }
            }
            return packSend;
        }

        public static List<StorageAddr> SerializeStorage(this Type type)
        {
            var props = type.GetProperties();
            List<StorageAddr> lstStorage = new List<StorageAddr>();
            if (props.Length > 0)
            {
                for (var i = 0; i < props.Length; i++)
                {
                    var attr = (StorageAttribute)props[i].GetCustomAttributes(typeof(StorageAttribute), true)[0];
                    var storage = new StorageAddr()
                    {
                        FieldName = props[i].Name,
                        Addr = attr.Addr,
                        Title = attr.Name,
                        FieldLength = props[i].PropertyType.GetByteSize(),
                        FieldType = props[i].PropertyType,
                    };
                    lstStorage.Add(storage);
                }
            }
            return lstStorage;
        }

    }
}
