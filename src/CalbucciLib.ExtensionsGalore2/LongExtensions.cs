using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.ExtensionsGalore
{
	public static class LongExtensions
	{
		private static readonly ulong[] _base62CharValue = new ulong[1 + (int)'z'];
		private const ulong _base62OutOfRange = ulong.MaxValue;
		private static readonly string[] _metricPrefix = new[] { "", "k", "M", "G", "T", "P", "E", "Z", "Y" };

		private static readonly string[] _literalDigits = new[]
		{
			"zero",
			"one",
			"two",
			"three",
			"four",
			"five",
			"six",
			"seven",
			"eight",
			"nine",
			"ten",
			"eleven",
			"twelve",
			"thirteen",
			"fourteen",
			"fifteen",
			"sixteen",
			"seventeen",
			"eighteen",
			"nineteen"
		};

        private static readonly string[] _literalTens = new string[]
		{
			"",
			"ten",
			"twenty",
			"thirty",
			"forty",
			"fifty",
			"sixty",
			"seventy",
			"eighty",
			"ninety"
		};

		private static readonly string[] _scaleNames = new[]
		{
			"thousand",
			"million",
			"billion",
			"trillion",
			"quadrillion",
			"quintillion",
		};


		static LongExtensions()
		{
			for (int i = 0; i < _base62CharValue.Length; i++)
			{
				_base62CharValue[i] = _base62OutOfRange; // out-of-range;				
			}

			for (int i = 0; i < ByteArrayExtensions.Base62Index.Length; i++)
			{
				char c = ByteArrayExtensions.Base62Index[i];
				_base62CharValue[(int)c] = (ulong)i;
			}

		}

		public static long FromBase62(string? base62)
		{
			if(string.IsNullOrWhiteSpace(base62))
				return 0;

			ulong result = 0;

			//skip whitespace
			var i = base62.IndexOfNonWhitespace();

			var max = Math.Min(i + 13, base62.Length);

			for (; i < max; i++)
			{
				var c = base62[i];
				if (c >= _base62CharValue.Length)
					break;

				var cv = _base62CharValue[c];
				if (cv == _base62OutOfRange)
					return 0;

				result = result * 62 + cv;
			}

			return (long)result;

		}

		public static int CountBits(this long l)
		{
			var iLow = (int) (l & 0xFFFFFFFF);
			var iHigh = (int) (l >> 32);

			return iLow.CountBits() + iHigh.CountBits();
		}

		public static string? ToBase62(this long l)
		{
			var num = (ulong)l;

			if (num < (ulong)ByteArrayExtensions.Base62Index.Length)
				return ByteArrayExtensions.Base62Index[(int)num].ToString();

			var result = new Stack<char>();
			while (num != 0)
			{
				result.Push(ByteArrayExtensions.Base62Index[(int)(num % 62)]);
				num /= 62;
			}
			return new string(result.ToArray());

		}

	    public static string? ToRoundNumber(this long l, int maxDecimalPlaces = 2)
	    {
            if (l == 0)
                return "0";

            if (maxDecimalPlaces < 0)
                maxDecimalPlaces = 0;
            else if (maxDecimalPlaces > 2)
                maxDecimalPlaces = 2;

            string? prefixMinus = null;
            if (l < 0)
            {
                prefixMinus = "- ";
                l = -l;
            }
            if (l < 1000)
                return prefixMinus + l.ToString();

            double d = (l / 1000.0);
            int metricPos = 1; // kilo

            while (d > 1000)
            {
                metricPos++;
                d /= 1000.0;
            }

            string number;
            if (d > 100 || maxDecimalPlaces == 0)
            {
                number = d.ToString("F0");
            }
            else if (d > 10 || maxDecimalPlaces == 1) // 1-digit
            {
                number = d.ToString("F1");
                if (number.EndsWith(".0"))
                    number = number.Substring(0, number.Length - 2);
            }
            else
            {
                number = d.ToString("F2");
                if (number.EndsWith(".00"))
                    number = number.Substring(0, number.Length - 3);
                else if (number.EndsWith("0"))
                    number = number.Substring(0, number.Length - 1);
            }
            //2-digit
            return prefixMinus + number + _metricPrefix[metricPos];
        }

		public static string? ToRoundedMemorySize(this long l, int maxDecimalPlaces = 2)
		{
			if (l == 0)
				return "0";

			if (maxDecimalPlaces < 0)
				maxDecimalPlaces = 0;
			else if (maxDecimalPlaces > 2)
				maxDecimalPlaces = 2;

			string? prefixMinus = null;
			if (l < 0)
			{
				prefixMinus = "- ";
				l = -l;
			}
			if (l < 1024)
				return prefixMinus + l.ToString();

			double d = (l / 1024.0);
			int metricPos = 1; // kilo

			while (d > 1024)
			{
				metricPos++;
                d = d / 1024.0;
			}

			string number;
			if (d > 100 || maxDecimalPlaces == 0)
			{
				number = d.ToString("F0");
			}
			else if (d > 10 || maxDecimalPlaces == 1) // 1-digit
			{
				number = d.ToString("F1");
				if (number.EndsWith(".0"))
					number = number.Substring(0, number.Length - 2);
			}
			else
			{
				number = d.ToString("F2");
				if (number.EndsWith(".00"))
					number = number.Substring(0, number.Length - 3);
				else if (number.EndsWith("0"))
					number = number.Substring(0, number.Length - 1);
			}
			//2-digit
			return prefixMinus + number + _metricPrefix[metricPos];
		}

		public static string? ToLiteral(this long l)
		{
			if (l >= 0 && l < _literalDigits.Length)
				return _literalDigits[l];

			StringBuilder sb = new StringBuilder();
			if (l < 0)
			{
				sb.Append("minus ");
				l = -l;
			}

			long scale = 1000000000000000000; // 18-zeroes (one quintillion)
			for (int i = 5; i >= 0; i--)
			{
				if (l < scale)
				{
					scale = scale/1000;
					continue;
				}
				long block = l/scale;
				l = l%scale;
				scale = scale/1000;
				if (block == 0)
					continue;

				sb.AppendFormat("{0} {1} ", Literal999(block), _scaleNames[i]);
			}

			if (l != 0)
				sb.Append(Literal999(l));
			else
			{
				sb.Remove(sb.Length - 1, 1);
			}
			return sb.ToString();
		}

		private static string? Literal999(long i)
		{
			if (i < _literalDigits.Length)
				return _literalDigits[i];

			List<string> words = new List<string>();
			if (i >= 100)
			{
				long cent = i / 100;
				words.Add(_literalDigits[cent] + " hundred");
				i = i % 100;
			}

			if (i >= 20)
			{
				long dec = i / 10;
				var tens = _literalTens[dec]; 
				i = i % 10;
				if (i != 0)
				{
					// using Associated Press guidelines
					tens += "-" + _literalDigits[i];
				}
				words.Add(tens);
			}
			else
			{
				if (i != 0)
				{
					words.Add(_literalDigits[i]);
				}
			}
			return string.Join(" ", words);
		}

	}
}
