using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.ExtensionsGalore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CalbucciLib.ExtensionsGalore.Tests
{
	[TestClass()]
	public class TimeZoneInfoExtensionsTests
	{
	
		[TestMethod()]
		public void ToIanaTimeZoneTest()
		{
			string[] tests = new[]
			{
				"Alaskan Standard Time", "America/Anchorage",
				"Pacific Standard Time", "America/Los_Angeles",
				"America/Los_Angeles", "America/Los_Angeles",
				"Etc/GMT+8", "Etc/GMT+8",
				"UTC-08", "Etc/GMT+8"
			};

			for (int i = 0; i < tests.Length; i+=2)
			{
				var expected = tests[i + 1];
				TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tests[i]);
				var actual = tzi.ToIanaId();
				Assert.AreEqual(expected, actual);
			}
			foreach (var tzi in TimeZoneInfo.GetSystemTimeZones())
			{
				var iana = tzi.ToIanaId();
				Assert.IsNotNull(iana, "{0} {1}", tzi.Id, tzi.BaseUtcOffset);
			}

		}

		[TestMethod()]
		public void ToWindowsIdTest()
		{
			string[] tests = new[]
			{
				"US/Alaska", "Alaskan Standard Time",
				"US/Pacific", "Pacific Standard Time",
				"Pacific Standard Time", "Pacific Standard Time",
				"Etc/GMT+8", "UTC-08",
				"UTC-08", "UTC-08"
			};

			for (int i = 0; i < tests.Length; i += 2)
			{
				var expected = tests[i + 1];
				TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById(tests[i]);
				var actual = tzi.ToWindowsId();
				Assert.AreEqual(expected, actual);
			}
		}

	}
}
