using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.ExtensionsGalore2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CalbucciLib.ExtensionsGalore2.Tests
{
	[TestClass()]
	public class ByteArrayExtensionsTests
	{
		[TestMethod()]
		public void ToBase64Test()
		{
			Random r = new Random(1);
			for (int i = 1; i < 64; i++)
			{
				var bytes = new byte[i]; 
				r.NextBytes(bytes);

				var base64 = bytes.ToBase64();
				var bytes2 = base64.ToBytesFromBase64();

				Assert.IsTrue(bytes.IsEqual(bytes2));
			}


		}

		[TestMethod()]
		public void ToBase64_Empty_Test()
		{
			var bytes = new byte[0];

			var base64 = bytes.ToBase64();
			Assert.IsTrue(bytes.IsEqual(base64.ToBytesFromBase64()));
		}

		[TestMethod()]
		public void ToBase62Test()
		{
			Random r = new Random(1);
			for (int i = 1; i < 64; i++)
			{
				var bytes = new byte[i];
				r.NextBytes(bytes);

				var base62 = bytes.ToBase62();
				var bytes2 = base62.ToBytesFromBase62();

				Assert.IsTrue(bytes.IsEqual(bytes2));
			}



		}

		[TestMethod()]
		public void ToBase62_Empty_Test()
		{
			var bytes = new byte[0];

			var base62 = bytes.ToBase62();
			Assert.IsTrue(bytes.IsEqual(base62.ToBytesFromBase64()));
		}

		[TestMethod()]
		public void ToHexEncodingTest()
		{
			Random r = new Random(1);
			for (int i = 1; i < 64; i++)
			{
				var bytes = new byte[i];
				r.NextBytes(bytes);

				var hex = bytes.ToHexEncoding();
				var bytes2 = hex.ToBytesFromHex();

				Assert.IsTrue(bytes.IsEqual(bytes2), "Size: " + i);
			}
		}

		[TestMethod()]
		public void ToHexEncodingUpper_Test()
		{
			Random r = new Random(1);
			for (int i = 1; i < 64; i++)
			{
				var bytes = new byte[i];
				r.NextBytes(bytes);

				var hex = bytes.ToHexEncoding().ToUpper();
				var bytes2 = hex.ToBytesFromHex();

				Assert.IsTrue(bytes.IsEqual(bytes2), "Size: " + i);
			}
		}

		[TestMethod()]
		public void ToHexEncoding_Empty_Test()
		{
			var bytes = new byte[0];

			var hex = bytes.ToHexEncoding();
			Assert.IsTrue(bytes.IsEqual(hex.ToBytesFromHex()));
		}


		[TestMethod()]
		public void ToStringFromUTF8Test()
		{
			string?[] tests = new[]
			{
				null,
				"",
				"a",
				"ação",
				"\u1234\u4567"
			};

			foreach (var test in tests)
			{
				var utf8Bytes = ByteArrayExtensions.ToUTF8(test);
				var actual = utf8Bytes.ToStringFromUTF8();

				Assert.AreEqual(test, actual);
			}
		}

	}
}
