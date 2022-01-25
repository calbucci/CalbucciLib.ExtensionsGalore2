using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalbucciLib.ExtensionsGalore
{
    public static class TimeZoneInfoExtensions
    {
        public static string? ToIanaId(this TimeZoneInfo tzi)
        {
            if (tzi.HasIanaId)
                return tzi.Id;
            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(tzi.Id, out var ianaId))
                return ianaId;

            
            var offset = -tzi.BaseUtcOffset; // GMT is the inverse of UTC in time presentation
            return "Etc/GMT" + offset.Hours.ToString("+#;-#;+0")
                + (offset.Minutes != 0 ? (":" + offset.Minutes.ToString("n2")) : "");
        }

        public static string? ToWindowsId(this TimeZoneInfo tzi)
        {
            if(!tzi.HasIanaId)
                return tzi.Id;

            if(TimeZoneInfo.TryConvertIanaIdToWindowsId(tzi.Id, out var windowsId))
                return windowsId;

            return null;
        }

        public static string? ToIanaId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            if (TimeZoneInfo.TryConvertWindowsIdToIanaId(id, out var ianaId))
                return ianaId;

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(id);
            if(tzi != null)
                return tzi.ToIanaId();

            return null;
        }

        public static string? ToWindowsId(string? id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            if (TimeZoneInfo.TryConvertIanaIdToWindowsId(id, out var windowsId))
                return windowsId;

            var tzi = TimeZoneInfo.FindSystemTimeZoneById(id);
            if (tzi != null)
                return tzi.ToWindowsId();

            return null;

        }

    }
}

