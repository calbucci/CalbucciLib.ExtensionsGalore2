using System.Diagnostics.CodeAnalysis;
using System.Text;
using CalbucciLib.ExtensionsGalore.Helper;

namespace CalbucciLib.ExtensionsGalore
{
    public static class ByteArrayExtensions
    {
        internal static string Base62Index = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        private static readonly string[] _byteMap;
        private static readonly int[] _toByteMap;

        static ByteArrayExtensions()
        {
            _byteMap = new string[256];
            for (int i = 0; i < 256; i++)
                _byteMap[i] = i.ToString("x2");

            _toByteMap = new int['g'];
            for (int i = 0; i < _toByteMap.Length; i++)
                _toByteMap[i] = -1;

            for (char c = '0'; c <= '9'; c++)
            {
                _toByteMap[c] = (byte)(c - '0');
            }
            for (char c = 'a'; c <= 'f'; c++)
            {
                _toByteMap[c] = (byte)(10 + c - 'a');
                var c2 = char.ToUpper(c);
                _toByteMap[c2] = (byte)(10 + c2 - 'A');
            }
        }




        // ==========================================================================
        //
        //   Base62 Encoding
        //
        // ==========================================================================
        public static byte[]? FromBase62(string? base62)
        {
            if (base62 == null)
                return null;

            if (base62.Length == 0)
                return Array.Empty<byte>();

            int len1 = base62.Length - 1;

            int count = 0;

            var stream = new BitStream(base62.Length * 6 / 8);

            foreach (char c in base62)
            {
                // Look up coding table
                if (c < '0' || c > 'z')
                    return null;

                int index = Base62Index.IndexOf(c);

                // If end is reached
                if (count == len1)
                {
                    // Check if the ending is good
                    int mod = (int)(stream.Position % 8);
                    if (mod == 0)
                        return null;

                    if ((index >> (8 - mod)) > 0)
                        return null;

                    stream.Write(new byte[] { (byte)(index << mod) }, 0, 8 - mod);
                }
                else
                {
                    // If 60 or 61 then only write 5 bits to the stream, otherwise 6 bits.
                    if (index == 60)
                    {
                        stream.Write(new byte[] { 0xf0 }, 0, 5);
                    }
                    else if (index == 61)
                    {
                        stream.Write(new byte[] { 0xf8 }, 0, 5);
                    }
                    else
                    {
                        stream.Write(new byte[] { (byte)index }, 2, 6);
                    }
                }
                count++;
            }

            // Dump out the bytes
            var result = new byte[stream.Position / 8];
            stream.Seek(0, SeekOrigin.Begin);
            stream.Read(result, 0, result.Length * 8);

            return result;
        }

        [return: NotNullIfNotNull("bytes")]
        public static string? ToBase62(this byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";

            // https://github.com/renmengye/base62-csharp/
            var sb = new StringBuilder(bytes.Length * 3 / 2);

            var stream = new BitStream(bytes);         // Set up the BitStream
            var b = new byte[1];                          // Only read 6-bit at a time
            while (true)
            {
                b[0] = 0;
                int length = stream.Read(b, 0, 6);           // Try to read 6 bits
                if (length == 6)                                // Not reaching the end
                {
                    if (b[0] >> 3 == 0x1f)            // First 5-bit is 11111
                    {
                        sb.Append(Base62Index[61]);
                        stream.Seek(-1, SeekOrigin.Current);    // Leave the 6th bit to next group
                    }
                    else if (b[0] >> 3 == 0x1e)       // First 5-bit is 11110
                    {
                        sb.Append(Base62Index[60]);
                        stream.Seek(-1, SeekOrigin.Current);
                    }
                    else                                        // Encode 6-bit
                    {
                        sb.Append(Base62Index[b[0] >> 2]);
                    }
                }
                else if (length == 0)                           // Reached the end completely
                {
                    break;
                }
                else                                            // Reached the end with some bits left
                {
                    // Padding 0s to make the last bits to 6 bit
                    sb.Append(Base62Index[b[0] >> 8 - length]);
                    break;
                }
            }
            return sb.ToString();
        }

        // ==========================================================================
        //
        //    Base64 Encoding
        //
        // ==========================================================================
        [return: NotNullIfNotNull("bytes")]
        public static string? ToBase64(this byte[]? bytes)
        {
            if (bytes == null)
                return null;
            return Convert.ToBase64String(bytes);
        }

        [return: NotNullIfNotNull("base64")]
        public static byte[]? FromBase64(string? base64)
        {
            if (base64 == null)
                return null;

            return Convert.FromBase64String(base64);
        }


        // ==========================================================================
        //
        //    HexEncoding
        //
        // ==========================================================================
        [return: NotNullIfNotNull("bytes")]
        public static string? ToHexEncoding(this byte[]? bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return "";

            var result = new char[bytes.Length * 2];
            int resultPos = 0;
            for (int i = 0; i < bytes.Length; i++)
            {
                var encodedByte = _byteMap[bytes[i]];
                result[resultPos] = encodedByte[0];
                result[resultPos + 1] = encodedByte[1];
                resultPos += 2;
            }
            return new string(result);
        }

        public static byte[]? FromHexEncoding(string? hexEncoded)
        {
            if (string.IsNullOrWhiteSpace(hexEncoded))
                return null;

            hexEncoded = hexEncoded.Trim();

            var result = new byte[hexEncoded.Length / 2];

            int bpos = 0;
            for (int i = 0; i < hexEncoded.Length; i += 2)
            {
                var hc = hexEncoded[i];
                if (hc >= _toByteMap.Length)
                    return null;
                var hb = _toByteMap[hc];
                if (hb == -1)
                    return null;

                int lb = 0;
                if (i + 1 < hexEncoded.Length)
                {
                    var lc = hexEncoded[i + 1];
                    lb = _toByteMap[lc];
                }

                result[bpos++] = (byte)((hb << 4) + lb);
            }

            return result;
        }

        // ==========================================================================
        //
        //    UTF-8 Encoding
        //
        // ==========================================================================
        [return: NotNullIfNotNull("bytes")]
        public static string? ToStringFromUTF8(this byte[]? bytes)
        {
            if (bytes == null)
                return null;
            if (bytes.Length == 0)
                return "";
            return UTF8Encoding.UTF8.GetString(bytes);
        }

        [return: NotNullIfNotNull("str")]
        public static byte[]? ToUTF8(string? str)
        {
            if(str == null)
                return null;
            if (str.Length == 0)
                return Array.Empty<byte>();

            return UTF8Encoding.UTF8.GetBytes(str);
        }

        // ==========================================================================
        //
        //    Compare
        //
        // ==========================================================================
        public static bool IsEqual(this byte[]? bytes, byte[]? bytes2)
        {
            if (bytes == null || bytes.Length == 0)
            {
                if (bytes2 == null || bytes2.Length == 0)
                    return true;
                return false;
            }
            else if (bytes2 == null || bytes2.Length == 0)
                return false;

            if (bytes.Length != bytes2.Length)
                return false;

            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] != bytes2[i])
                    return false;
            }
            return true;
        }


    }
}
