using Newtonsoft.Json;
using System;
using System.Text;

namespace Birdie.Service
{
    public static class Util
    {
        public static byte[] GetJsonBytes<T>(T obj)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static ArraySegment<byte> GetArraySegment(byte[] bytes)
        {
            return new ArraySegment<byte>(bytes, 0, bytes.Length);
        }

        public static ArraySegment<byte> GetJsonAsArraySegment<T>(T obj)
        {
            return GetArraySegment(GetJsonBytes(obj));
        }
    }
}
