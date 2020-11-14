using Binary_Engine.HexBox;
using System;
using System.Collections.Generic;

namespace Binary_Engine.Utilities
{
    class SignatureScan
    {
        public List<byte> ByteArray { get; }
        public List<byte> Mask { get; }
        public String Pattern { get; }
        public int Result { set; get; }
        public int PatternSize { get; }

        private IByteProvider Provider;

        public SignatureScan(String pattern, IByteProvider provider, int result = 1)
        {
            ByteArray = new List<byte>();
            Mask = new List<byte>();

            Pattern = pattern;
            Provider = provider;
            Result = result;
            PatternSize = 0;

            if (!String.IsNullOrEmpty(Pattern))
            {
                Pattern = Pattern.Replace(" ? ", " ?? ");
                
                while (Pattern.EndsWith(" ") || Pattern.EndsWith("?"))
                {
                    Pattern = Pattern.Substring(0, Pattern.Length - 1);
                }

                {
                    String data = Pattern;
                    data = data.Replace(" ", "");

                    if (data.Length % 2 == 0 && data.Length > 0)
                    {
                        PatternSize = data.Length / 2;

                        for (int i = 0; i < data.Length; i += 2)
                        {
                            if (data[i] == '?' && data[i + 1] == '?')
                            {
                                Mask.Add(1);
                                ByteArray.Add(0);
                            }
                            else
                            {
                                Mask.Add(0);
                                ByteArray.Add(Convert.ToByte(data[i].ToString() + data[i + 1].ToString(), 16));
                            }
                        }

                    }
                }
            }
        }

        public long Address()
        {
            long k = 1;
            for (long i = 0; i < Provider.Length; ++i)
            {
                int j = 0;
                for (; j < PatternSize; ++j)
                {
                    if (Mask[j] == 0x01)
                    {
                        continue;
                    }
                    if ((Provider.ReadByte(i + j) ^ ByteArray[j]) != 0)
                    {
                        break;
                    }

                }

                if (j == PatternSize)
                {
                    if (k == Result)
                    {
                        return i;
                    }
                    else
                    {
                        ++k;
                        continue;
                    }
                }
            }

            return -1;
        }
    }
}
