using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPro
{
    static class Crypt
    {
        public static string Encrypt(string str, string key)
        {
            byte[] bstr = Encoding.UTF8.GetBytes(str);
            byte[] bkey = Encoding.UTF8.GetBytes(key);
            byte[] bres = new byte[bstr.Length];

            for (int i = 0; i < bstr.Length; i++)
            {
                bres[i] = (byte)(bstr[i] ^ bkey[i % bkey.Length]);
            }

            return Encoding.UTF8.GetString(bres);
        }

        public static string Decrypt(string str, string key)
        {
            return Encrypt(str, key);
        }
    }
}
