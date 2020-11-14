using System;
using System.Collections.Generic;
using System.Text;

namespace Binary_Engine.Utilities
{
    class BytesConverter
    {
        public byte[] Buffer { get; set; }
        public BytesConverter(byte[] buffer)
        {
            Buffer = buffer;
        }

        public String ToHexadecimalString(string separator = " ")
        {
            string data = BitConverter.ToString(Buffer);
            if (separator == "0x")
            {
                return "0x" + data.Replace("-", " 0x");
            }
            if (separator != "-")
            {
                data = data.Replace("-", separator);
            }
            if (separator == "\\x")
            {
                data = "\\x" + data;
            }
            return data;
        }

        public String ToHexadecimalString(int n)
        {
            if (Buffer.Length > 0 && Buffer.Length % n == 0)
            {
                string data = "";
                for (int i = 0; i < Buffer.Length; i += n)
                {
                    data += "0x";

                    for (int j = n - 1; j >= 0; --j)
                    {
                        data += Buffer[i + j].ToString("X2");
                    }

                    data += " ";
                }
                return data;
            }

            return String.Empty;
        }

        public String ToDecimalString()
        {
            if (Buffer.Length > 0)
            {
                string data = "";
                for (int i = 0; i < Buffer.Length; ++i)
                {
                    data += Buffer[i] + " ";
                }
                return data;
            }
            return String.Empty;
        }

        public static String ToBits(byte bits)
        {
            //eight bits = 1 byte
            string bit = "";
            for (int i = 7; i >= 0; --i)
            {
                bit += (bits & (1 << i)) != 0 ? "1" : "0";   
            }
            return bit;
        }

        public static byte[] StringToBytes(string hexadecimal_string)
        {
            StringBuilder data  = new StringBuilder(hexadecimal_string);
            List<byte> buffer = new List<byte>();

            data = data.Replace(" ", "");
            if (String.IsNullOrEmpty(data.ToString()) ||(data.Length % 2 != 0))
            {
                return buffer.ToArray();
            }

            Func<char, bool> isxdigit = (c) =>
            {
                return
                    '0' <= c && c <= '9' ||
                    'a' <= c && c <= 'f' ||
                    'A' <= c && c <= 'F';
            };

            Random random = new Random();
            string hexadecimal_characters = "0123456789ABCDEF";
            for (int i = 0; i < data.Length; ++i)
            {
                if (!isxdigit(data[i]))
                {
                    data[i] = hexadecimal_characters[random.Next(16)]; //0 <= n < 16
                }
            }

            for (int i = 0; i < data.Length; i += 2)
            {
                buffer.Add(Convert.ToByte(data[i].ToString() + data[i + 1].ToString(), 16));
            }

            return buffer.ToArray();
        }
    }
}
