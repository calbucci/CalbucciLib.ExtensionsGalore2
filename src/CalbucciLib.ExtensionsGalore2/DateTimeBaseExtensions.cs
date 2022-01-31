using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.ExtensionsGalore2
{
	public static class DateTimeBaseExtensions
	{
		private static readonly string[] _AMDesignators =
		{
			"a", "am", "a.m.", "a.m", "ص", "上午", "dop.", "dop", "πμ", "de.",
			"de", "午前", "오전", "pd", "el", "ق.ظ", "sa", "पूर्वाह्न", "r.n.", "rn", "পুর্বাহ্ন", "ਸਵੇਰ", "પૂર્વ મધ્યાહ્ન", "காலை",
			"పూర్వాహ్న", "ಪೂರ್ವಾಹ್ನ", "ৰাতিপু", "म.पू.", "སྔ་དྲོ", "ព្រឹក", "ເຊົ້າ", "ܩ.ܛ", "පෙ.ව.", "ጡዋት", "विहानी", "غ.م", "މކ",
			"safe", "owuro", "ututu", "ꂵꆪꈌꈐ", "چۈشتىن بۇرۇن", "saa moya z.m.", "m"
		};

		private static readonly string[] _PMDesignators =
		{
			"p", "pm", "p.m.", "p.m", "م", "下午", "odp.", "odp", "μμ", "du.",
			"du", "午後", "오후", "md", "pl", "ب.ظ", "ch", "अपराह्न", "i.n.", "in", "অপরাহ্ন", "ਸ਼ਾਮ", "ઉત્તર મધ્યાહ્ન", "மாலை",
			"అపరాహ్న", "ಅಪರಾಹ್ನ", "আবেলি", "म.नं.", "ཕྱི་དྲོ", "ល្ងាច", "ແລງ", "ܒ.ܛ", "ප.ව.", "ከሰዓት", "बेलुकी", "غ.و", "މފ",
			"yamma", "ale", "efifie", "ꂵꆪꈌꉈ", "چۈشتىن كېيىن", "saa moya z.n.", "f"
		};

		private static readonly DateTime _unixEpoch = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Unspecified);
		private static readonly DateTime _unixEpochLocal = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
		private static readonly DateTime _unixEpochUtc = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static Calendar _defaultCalendar;

		static DateTimeBaseExtensions()
		{
			DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
			_defaultCalendar = dtfi.Calendar;
		}

		public static string[] AMDesignators => _AMDesignators;
		public static string[] PMDesignators => _PMDesignators;
		public static DateTime UnixEpoch => _unixEpoch;
		public static DateTime UnixEpochLocal => _unixEpochLocal;
		public static DateTime UnixEpochUtc => _unixEpochUtc;

		public static Calendar DefaultCalendar => _defaultCalendar;
	}
}