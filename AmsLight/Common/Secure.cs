using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AmsLight.Common
{
    public static class Secure
    {
        public static string Encode(string encodeMe)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(encodeMe);
            return Convert.ToBase64String(encoded);
        }
        public static string Encode(int id)
        {
            byte[] encoded = System.Text.Encoding.UTF8.GetBytes(Convert.ToString(id));
            return Convert.ToBase64String(encoded);
        }

        public static int Decode(string decodeMe)
        {
            byte[] encoded = Convert.FromBase64String(decodeMe);
            return Convert.ToInt32(System.Text.Encoding.UTF8.GetString(encoded));
        }
    }
}