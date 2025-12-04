using System;
using System.IO;
using System.Runtime.Serialization.Json;

namespace LlamaSoft.NET.Extensions
{
    public static class Objects
    {
        #region Serialize Methods
        public static object Serialize(this object objecttoserialize, FileInfo targetfile)
        {
            DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(objecttoserialize.GetType());
            using (FileStream data_stream = new FileStream(targetfile.FullName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                json_serializer.WriteObject(data_stream, objecttoserialize);
            }
            return objecttoserialize;
        }
        public static object DeSerialize(this object objecttodeserialize, string targetfile)
        {
            return DeSerialize(objecttodeserialize, new FileInfo(targetfile));
        }
        public static object DeSerialize(this object objecttodeserialize, FileInfo targetfile)
        {
            object result = null;
            if (!File.Exists(targetfile.FullName))
            {
                throw new SerializationException(string.Format(@"Cannot deserialize, {0} file does not exist.", targetfile.FullName));
            }
            DataContractJsonSerializer json_serializer = new DataContractJsonSerializer(objecttodeserialize.GetType());
            using (FileStream data_stream = new FileStream(targetfile.FullName, FileMode.Open, FileAccess.Read))
            {
                result = json_serializer.ReadObject(data_stream);
            }
            return result;
        }
        #endregion

        #region Numeric Methods
        public static ushort GetIntFloor(this object decimal_value)
        {
            string object_type = decimal_value.GetType().Name;
            if (object_type == "float" || object_type == "double" || object_type == "decimal")
            {
                return Convert.ToUInt16(Math.Floor((decimal)decimal_value));
            }
            return Convert.ToUInt16(decimal_value);
        }
        public static ushort GetIntCeiling(this object decimal_value)
        {
            string object_type = decimal_value.GetType().Name;
            if (object_type == "float" || object_type == "double" || object_type == "decimal")
            {
                return Convert.ToUInt16(Math.Ceiling((decimal)decimal_value));
            }
            return Convert.ToUInt16(decimal_value);
        }
        #endregion
    }
}
