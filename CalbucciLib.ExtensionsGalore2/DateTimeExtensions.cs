﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.ExtensionsGalore;

namespace CalbucciLib.ExtensionsGalore
{
	public enum DateTimeSignificance
	{
		ToSeconds,
		ToMinutes,
		ToHours,
		ToDays,
		ToMonths,
		ToYears
	};

	public static class DateTimeExtensions 
	{

		// ==========================================================================
		//
		//    Parse
		//
		// ==========================================================================
		public static TimeSpan? ParseTime(string? time)
		{
			if (string.IsNullOrWhiteSpace(time))
				return null;

			var ts = time.Trim();
			if (ts.Length == 0)
				return null;

			var minutes = 0;
			var seconds = 0;

			var parts = time.Split(':');
			if (parts.Length > 3)
				return null;

			var hoursPart = parts[0];
			var pos = hoursPart.IndexOf(c => char.IsDigit(c) || c == '-');
			if (pos == -1)
				return null;

			var hours = hoursPart[pos..].ToInt();
			if (hours < 0 || hours > 23)
				return null;

			var isPM = DateTimeBaseExtensions.
				PMDesignators.Any(d => time.Contains(d, StringComparison.InvariantCultureIgnoreCase));

			if (isPM)
			{
				if (hours < 12)
					hours += 12;
			}

			if (parts.Length >= 2)
			{
				var minutesPart = parts[1];
				pos = minutesPart.IndexOf(c => char.IsDigit(c) || c == '-');
				if (pos == -1)
					return null;
				minutes = minutesPart[pos..].ToInt();
				if (minutes < 0 || minutes > 59)
					return null;

				if (parts.Length >= 3)
				{
					string secondsParts = parts[2];
					pos = secondsParts.IndexOf(c => char.IsDigit(c) || c == '-');
					if (pos == -1)
						return null;
					seconds = secondsParts[pos..].ToInt();
					if (seconds < 0 || seconds > 59)
						return null;
				}
			}

			return new TimeSpan(hours, minutes, seconds);
		}

		// ==========================================================================
		//
		//    Conversion
		//
		// ==========================================================================
		public static DateTime FromUnixTime(long unixTime, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			if (kind == DateTimeKind.Local)
				return DateTimeBaseExtensions.UnixEpochLocal.AddSeconds(unixTime);
			if (kind == DateTimeKind.Utc)
				return DateTimeBaseExtensions.UnixEpochUtc.AddSeconds(unixTime);
			return DateTimeBaseExtensions.UnixEpoch.AddSeconds(unixTime);
		}

		public static long ToUnixTime(this DateTime dateTime)
		{
			return (long)((dateTime - DateTimeBaseExtensions.UnixEpoch).TotalSeconds);
		}

		public static string? ToRelativeTime(this DateTime utcDateTime, DateTime? baseDate = null)
		{
			if (baseDate == null)
				baseDate = DateTime.UtcNow;

			var ts = baseDate.Value - utcDateTime;
			var delta = Math.Round(ts.TotalSeconds, 0);

			if (delta < -0.1)
			{
				// In the future
				delta = -delta;
				ts = -ts;

				if (delta < 60)
				{
					return ts.Seconds == 1 ? "in one second" : "in " + ts.Seconds + " seconds";
				}
				if (delta < 120)
				{
					return "in one minute";
				}
				if (delta < 3000) // 50 * 60
				{
					return "in " + ts.Minutes + " minutes";
				}
				if (delta < 5400) // 90 * 60
				{
					return "in one hour";
				}
				if (delta < 86400) // 24 * 60 * 60
				{
					return "in " + ts.Hours + " hours";
				}
				if (delta < 172800) // 48 * 60 * 60
				{
					return "in one day";
				}
				if (delta < 28 * 24 * 60 * 60) // 28 * 24 * 60 * 60
				{
					return "in " +  ts.Days + " days";
				}
				if (delta < 12 * 29 * 24 * 60 * 60) // 12 * 29 * 24 * 60 * 60
				{
					var months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
					return "in " + ((months <= 1) ? "one month" : months + " months");
				}
				else
				{
					var years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
					return years <= 1 ? "one year ago" : years + " years ago";
				}
			}
			if (delta < 0.1)
				return "now";
			if (delta < 60)
			{
				return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";
			}
			if (delta < 120)
			{
				return "one minute ago";
			}
			if (delta < 3000) // 50 * 60
			{
				return ts.Minutes + " minutes ago";
			}
			if (delta < 5400) // 90 * 60
			{
				return "one hour ago";
			}
			if (delta < 86400) // 24 * 60 * 60
			{
				return ts.Hours + " hours ago";
			}
			if (delta < 172800) // 48 * 60 * 60
			{
				return "one day ago";
			}
			if (delta < 29 * 24 * 60 * 60) // 29 * 24 * 60 * 60
			{
				return ts.Days + " days ago";
			}
			if (delta < 31104000) // 12 * 30 * 24 * 60 * 60
			{
				var months = Convert.ToInt32(Math.Floor((double) ts.Days/30));
				return months <= 1 ? "one month ago" : months + " months ago";
			}
			else
			{
				var years = Convert.ToInt32(Math.Floor((double) ts.Days/365));
				return years <= 1 ? "one year ago" : years + " years ago";
			}
		}

		// ==========================================================================
		//
		//   Comparison
		//
		// ==========================================================================
		public static bool IsBetween(this DateTime dateTime, DateTime dateTime1, DateTime dateTime2)
		{
			if (dateTime1 > dateTime2)
			{
				return dateTime >= dateTime2 && dateTime <= dateTime1;
			}
			return dateTime >= dateTime1 && dateTime <= dateTime2;
		}

		public static int CompareTo(this DateTime dateTime, DateTime dateToCompare, DateTimeSignificance significance)
		{
			int yearsDelta = dateTime.Year - dateToCompare.Year;
			if (yearsDelta != 0)
				return yearsDelta < 0 ? -1 : 1;

			if (significance == DateTimeSignificance.ToYears)
				return 0;

			int monthsDelta = dateTime.Month - dateToCompare.Month;
			if (monthsDelta != 0)
				return monthsDelta < 0 ? -1 : 1;

			if (significance == DateTimeSignificance.ToMonths)
				return 0;

			int daysDelta = dateTime.Day - dateToCompare.Day;
			if (daysDelta != 0)
				return daysDelta < 0 ? -1 : 1;

			if (significance == DateTimeSignificance.ToDays)
				return 0;

			int hoursDelta = dateTime.Hour - dateToCompare.Hour;
			if (hoursDelta != 0)
				return hoursDelta < 0 ? -1 : 1;

			if (significance == DateTimeSignificance.ToHours)
				return 0;

			int minutesDelta = dateTime.Minute - dateToCompare.Minute;
			if (minutesDelta != 0)
				return minutesDelta < 0 ? -1 : 1;

			if (significance == DateTimeSignificance.ToMinutes)
				return 0;

			int secondsDelta = dateTime.Second - dateToCompare.Second;
			if (secondsDelta != 0)
				return secondsDelta < 0 ? -1 : 1;

			return 0;
		}


		// ==========================================================================
		//
		//   Calendar
		//
		// ==========================================================================
		public static TimeSpan ElapsedToNow(this DateTime dateTime)
		{
			var utc = DateTime.UtcNow;
			if (utc < dateTime)
				return dateTime - utc;
			return utc - dateTime;
		}

		public static DateTime GetFirstDayOfMonth(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, 1, 0, 0, 0, date.Kind);
		}

		public static DateTime GetFirstDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
		{
			date = date.GetFirstDayOfMonth();

			var dow = date.DayOfWeek;
			if (dow == dayOfWeek)
				return date;

			var lwd = dayOfWeek - date.DayOfWeek;
			if (lwd < 0)
				lwd += 7;

			return date.AddDays(lwd);
		}

		public static DateTime GetFirstDayOfPreviousMonth(this DateTime date)
		{
			int month = date.Month;
			if (month == 1)
				return new DateTime(date.Year - 1, 12, 1, 0, 0, 0, date.Kind);

			return new DateTime(date.Year, month - 1, 1, 0, 0, 0, date.Kind);

		}

		public static DateTime GetFirstDayOfNextMonth(this DateTime date)
		{
			int month = date.Month;
			if (month == 12)
				return new DateTime(date.Year + 1, 1, 1, 0, 0, 0, date.Kind);

			return new DateTime(date.Year, month + 1, 1, 0, 0, 0, date.Kind);
		}


		public static DateTime GetFirstMondayOfMonth(this DateTime date)
		{
			return date.GetFirstDayOfMonth(DayOfWeek.Monday);
		}

		public static DateTime GetFirstSundayOfMonth(this DateTime date)
		{
			return date.GetFirstDayOfMonth(DayOfWeek.Sunday);
		}

		public static DateTime GetLastDayOfMonth(this DateTime date)
		{
			int year = date.Year;
			int month = date.Month;
			int daysInMonth = DateTime.DaysInMonth(year, month);
			var newDate = new DateTime(year, month, daysInMonth);
			return newDate;
		}

		public static DateTime GetLastDayOfMonth(this DateTime date, DayOfWeek dayOfWeek)
		{
			var newDate = date.GetLastDayOfMonth();

			var lwd = dayOfWeek - newDate.DayOfWeek;

			return newDate.AddDays(lwd);
		}

		public static DateTime GetFirstDayOfQuarter(this DateTime date)
		{
			var month = date.Month;
			var year = date.Year;
			if (month <= 3)
				return new DateTime(year, 1, 1, 0, 0, 0, date.Kind);
			if (month <= 6)
				return new DateTime(year, 4, 1, 0, 0, 0, date.Kind);
			if (month <= 9)
				return new DateTime(year, 7, 1, 0, 0, 0, date.Kind);

			return new DateTime(year, 10, 1, 0, 0, 0, date.Kind);
		}

		public static DateTime GetFirstDayOfQuarter(this DateTime date, DayOfWeek dayOfWeek)
		{
			date = date.GetFirstDayOfQuarter();
			var dow = date.DayOfWeek;
			if (dow == dayOfWeek)
				return date;

			var lwd = dayOfWeek - date.DayOfWeek;
			if (lwd < 0)
				lwd += 7;

			return date.AddDays(lwd);
		}


		public static DateTime GetLastDayOfQuarter(this DateTime date)
		{
			int month = date.Month;
			int year = date.Year;

			if (month >= 10)
				return new DateTime(year, 12, 31, 0, 0, 0, date.Kind);
			if (month >= 7)
				return new DateTime(year, 9, 30, 0, 0, 0, date.Kind);
			if (month >= 4)
				return new DateTime(year, 6, 30, 0, 0, 0, date.Kind);

			return new DateTime(year, 3, 31, 0, 0, 0, date.Kind);
		}

		public static DateTime GetLastDayOfQuarter(this DateTime date, DayOfWeek dayOfWeek)
		{
			date = date.GetLastDayOfQuarter();
			var dow = date.DayOfWeek;
			if (dow == dayOfWeek)
				return date;

			var lwd = dayOfWeek - date.DayOfWeek;
			if (lwd > 0)
				lwd -= 7;

			return date.AddDays(lwd);
		}


		public static DateTime GetPreviousSunday(this DateTime date)
		{
			return date.GetPrevious(DayOfWeek.Sunday);
		}

		public static DateTime GetPreviousMonday(this DateTime date)
		{
			return date.GetPrevious(DayOfWeek.Monday);
		}

		/// <summary>
		/// Return the most recent day of the week. If the dayOfWeek is the same as this date, than it returns one week prior.
		/// </summary>
		public static DateTime GetPrevious(this DateTime date, DayOfWeek dayOfWeek)
		{
			date = date.Date;
			int lwd = date.DayOfWeek - dayOfWeek;
			if (lwd <= 0)
			{
				lwd += 7;
			}

			date = date.AddDays(-1 * lwd);

			return date;
		}


		/// <summary>
		/// Return the next dayOfWeek from this date. If the DayOfWeek is the same as this date, than it returns one week later.
		/// </summary>
		public static DateTime GetNext(this DateTime date, DayOfWeek dayOfWeek)
		{
			date = date.Date;
			int lwd = dayOfWeek - date.DayOfWeek;
			if (lwd <= 0)
				lwd += 7;

			date = date.AddDays(lwd);
			return date;
		}

		public static DateTime GetNextSunday(this DateTime date)
		{
			return GetNext(date, DayOfWeek.Sunday);
		}

		public static DateTime GetNextMonday(this DateTime date)
		{
			return GetNext(date, DayOfWeek.Monday);
		}

		public static int GetAgeInYears(this DateTime date, DateTime? baseDate = null)
		{
			if (baseDate == null)
				baseDate = DateTime.UtcNow;

			var age = (baseDate.Value.Year - date.Year - 1) +
					   (((baseDate.Value.Month > date.Month) ||
						 ((baseDate.Value.Month == date.Month) && (baseDate.Value.Day >= date.Day)))
						   ? 1
						   : 0);

			return age;
		}

		public static int GetWeekOfYear(this DateTime date, 
			DayOfWeek firstDayOfWeek = DayOfWeek.Sunday, Calendar? calendar = null)
		{
			if (calendar == null)
				calendar = DateTimeBaseExtensions.DefaultCalendar;
			
			return calendar.GetWeekOfYear(date, CalendarWeekRule.FirstDay, firstDayOfWeek);
		}



		// ==========================================================================
		//
		//   Truncation
		//
		// ==========================================================================
		public static DateTime Round(this DateTime dateTime, DateTimeSignificance significance)
		{
			DateTime result;
			var year = dateTime.Year;
			var month = dateTime.Month;
			if (significance != DateTimeSignificance.ToYears)
			{
				var day = dateTime.Day;
				if (significance != DateTimeSignificance.ToMonths)
				{
					var hour = dateTime.Hour;
					if (significance != DateTimeSignificance.ToDays)
					{
						var minute = dateTime.Minute;
						if (significance != DateTimeSignificance.ToHours)
						{
							var second = dateTime.Second;
							if (significance != DateTimeSignificance.ToMinutes)
							{
								// Seconds
								result = new DateTime(year, month, day, hour, minute, second, dateTime.Kind);
								if (dateTime.Millisecond >= 500)
									result = result.AddSeconds(1);
							}
							else // minutes
							{
								result = new DateTime(year, month, day, hour, minute, 0, dateTime.Kind);
								if (second >= 30)
									result = result.AddMinutes(1);
							}

						}
						else // hours
						{
							result = new DateTime(year, month, day, hour, 0, 0, dateTime.Kind);
							if (minute >= 30)
								result = result.AddHours(1);
						}
					}
					else // day
					{
						result = new DateTime(year, month, day, 0, 0, 0, dateTime.Kind);
						if (hour >= 12)
							result = result.AddDays(1);
					}
				}
				else // month
				{
					result = new DateTime(year, month, 1);

					int dayThreshold = 16;
					int daysInMonth = DateTime.DaysInMonth(year, month);
					if (daysInMonth == 30 || daysInMonth == 29)
					{
						dayThreshold = 15;
					}
					else if (daysInMonth == 28)
					{
						dayThreshold = 14;
					}

					if (day > dayThreshold)
					{
						result = result.AddMonths(1);
					}
				}
			}
			else
			{
				if (month >= 7)
					year++;
				result = new DateTime(year, 1, 1, 0, 0, 0, dateTime.Kind);
			}
			return result;
		}

		public static DateTime Truncate(this DateTime dateTime, DateTimeSignificance significance)
		{
			var year = dateTime.Year;
			int month = 1, day = 1, hour = 0, minute = 0, second = 0;
			if (significance != DateTimeSignificance.ToYears)
			{
				month = dateTime.Month;
				if (significance != DateTimeSignificance.ToMonths)
				{
					day = dateTime.Day;
					if (significance != DateTimeSignificance.ToDays)
					{
						hour = dateTime.Hour;
						if (significance != DateTimeSignificance.ToHours)
						{
							minute = dateTime.Minute;
							if (significance != DateTimeSignificance.ToMinutes)
							{
								second = dateTime.Second;
							}
						}
					}
				}
			}
			return new DateTime(year, month, day, hour, minute, second, dateTime.Kind);
		}

	}
}
