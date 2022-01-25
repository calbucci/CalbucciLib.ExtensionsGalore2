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
        public static Dictionary<string, string> _olsonToWindows;
        private static Dictionary<string, string[]> _windowsToOlson;
        static TimeZoneInfoExtensions()
        {
            // The first mapping is the most popular one
            _windowsToOlson = new(StringComparer.InvariantCultureIgnoreCase)
            {
                ["Afghanistan Standard Time"] = new[] { "Asia/Kabul", },
                ["Alaskan Standard Time"] = new[]
            {
                "US/Alaska",
                "America/Anchorage",
                "America/Juneau",
                "America/Nome",
                "America/Sitka",
                "America/Yakutat",
            },
                ["Arab Standard Time"] = new[]
            {
                "Asia/Riyadh",
                "Asia/Aden",
                "Asia/Bahrain",
                "Asia/Kuwait",
                "Asia/Qatar",
            },
                ["Arabian Standard Time"] = new[]
            {
                "Asia/Dubai",
                "Asia/Muscat",
                "Etc/GMT-4",
            },
                ["Arabic Standard Time"] = new[]
            {
                "Asia/Baghdad",
            },
                ["Argentina Standard Time"] = new[]
            {
                "America/Buenos_Aires",
                "America/Argentina/Buenos_Aires",
                "America/Argentina/Cordoba",
                "America/Argentina/Rio_Gallegos",
                "America/Argentina/Salta",
                "America/Argentina/San_Juan",
                "America/Argentina/San_Luis",
                "America/Argentina/Tucuman",
                "America/Argentina/Ushuaia",
                "America/Catamarca",
                "America/Cordoba",
                "America/Jujuy",
                "America/Mendoza",
            },
                ["Atlantic Standard Time"] = new[]
            {
                "America/Halifax",
                "America/Glace_Bay",
                "America/Goose_Bay",
                "America/Moncton",
                "America/Thule",
                "Atlantic/Bermuda",
            },
                ["AUS Central Standard Time"] = new[]
            {
                "Australia/Darwin",
            },
                ["AUS Eastern Standard Time"] = new[]
            {
                "Australia/Sydney",
                "Australia/Melbourne",
            },
                ["Azerbaijan Standard Time"] = new[]
            {
                "Asia/Baku",
            },
                ["Azores Standard Time"] = new[]
            {
                "Atlantic/Azores",
                "America/Scoresbysund",
            },
                ["Bahia Standard Time"] = new[]
            {
                "America/Bahia",
            },
                ["Bangladesh Standard Time"] = new[]
            {
                "Asia/Dhaka",
                "Asia/Thimphu",
            },
                ["Canada Central Standard Time"] = new[]
            {
                "America/Regina",
                "America/Swift_Current",
            },
                ["Cape Verde Standard Time"] = new[]
            {
                "Atlantic/Cape_Verde",
                "Etc/GMT+1",
            },
                ["Caucasus Standard Time"] = new[]
            {
                "Asia/Yerevan",
            },
                ["Cen. Australia Standard Time"] = new[]
            {
                "Australia/Adelaide",
                "Australia/Broken_Hill",
            },
                ["Central America Standard Time"] = new[]
            {
                "America/Guatemala",
                "America/Belize",
                "America/Costa_Rica",
                "America/El_Salvador",
                "America/Managua",
                "America/Tegucigalpa",
                "Etc/GMT+6",
                "Pacific/Galapagos",
            },
                ["Easter Island Standard Time"] = new[]
                {
                    "Chile/EasterIsland"
                },
                ["Central Asia Standard Time"] = new[]
            {
                "Asia/Almaty",
                "Antarctica/Vostok",
                "Asia/Bishkek",
                "Asia/Qyzylorda",
                "Indian/Chagos",
                "Etc/GMT-6",
            },
                ["Central Brazilian Standard Time"] = new[]
            {
                "America/Campo_Grande",
                "America/Cuiaba",
            },
                ["Central Europe Standard Time"] = new[]
            {
                "Europe/Budapest",
                "Europe/Belgrade",
                "Europe/Bratislava",
                "Europe/Ljubljana",
                "Europe/Podgorica",
                "Europe/Prague",
                "Europe/Tirane",
            },
                ["Central European Standard Time"] = new[]
            {
                "Europe/Warsaw",
                "Europe/Sarajevo",
                "Europe/Skopje",
                "Europe/Zagreb",
            },
                ["Central Pacific Standard Time"] = new[]
            {
                "Pacific/Guadalcanal",
                "Antarctica/Macquarie",
                "Pacific/Efate",
                "Pacific/Kosrae",
                "Pacific/Noumea",
                "Pacific/Ponape",
                "Etc/GMT-11",
            },
                ["Central Standard Time"] = new[]
            {
                "US/Central",
                "America/Chicago",
                "America/Indiana/Knox",
                "America/Indiana/Tell_City",
                "America/Matamoros",
                "America/Menominee",
                "America/North_Dakota/Beulah",
                "America/North_Dakota/Center",
                "America/North_Dakota/New_Salem",
                "America/Rainy_River",
                "America/Rankin_Inlet",
                "America/Resolute",
                "America/Winnipeg",
                "CST6CDT",
            },
                ["Central Standard Time (Mexico)"] = new[]
            {
                "America/Mexico_City",
                "America/Bahia_Banderas",
                "America/Merida",
                "America/Monterrey",
            },
                ["China Standard Time"] = new[]
            {
                "Asia/Shanghai",
                "Asia/Chongqing",
                "Asia/Harbin",
                "Asia/Hong_Kong",
                "Asia/Kashgar",
                "Asia/Macau",
                "Asia/Urumqi",
            },
                ["Dateline Standard Time"] = new[]
            {
                "Etc/GMT+12",
            },
                ["E. Africa Standard Time"] = new[]
            {
                "Africa/Khartoum",
                "Africa/Addis_Ababa",
                "Africa/Asmera",
                "Africa/Dar_es_Salaam",
                "Africa/Djibouti",
                "Africa/Juba",
                "Africa/Kampala",
                "Africa/Mogadishu",
                "Africa/Nairobi",
                "Antarctica/Syowa",
                "Indian/Antananarivo",
                "Indian/Comoro",
                "Indian/Mayotte",
                "Etc/GMT-3",
            },
                ["E. Australia Standard Time"] = new[]
            {
                "Australia/Brisbane",
                "Australia/Lindeman",
            },
                ["E. Europe Standard Time"] = new[]
            {
                "Asia/Nicosia",
            },
                ["E. South America Standard Time"] = new[]
            {
                "America/Sao_Paulo",
            },
                ["Tocantins Standard Time"] = new[]
                {
                    "America/Araguaina"
                },
                ["Eastern Standard Time"] = new[]
            {
                "US/Eastern",
                "America/Atikokan",
                "America/Detroit",
                "America/Grand_Turk",
                "America/Indiana/Petersburg",
                "America/Indiana/Vincennes",
                "America/Indiana/Winamac",
                "America/Iqaluit",
                "America/Kentucky/Louisville",
                "America/Kentucky/Monticello",
                "America/Louisville",
                "America/Montreal",
                "America/Nassau",
                "America/New_York",
                "America/Nipigon",
                "America/Pangnirtung",
                "America/Thunder_Bay",
                "America/Toronto",
                "EST5EDT",
            },
                ["Eastern Standard Time (Mexico)"] = new[]
            {
                "America/Cancun",
            },
                ["Egypt Standard Time"] = new[]
            {
                "Africa/Cairo",
                "Asia/Gaza",
                "Asia/Hebron",
            },
                ["Ekaterinburg Standard Time"] = new[]
            {
                "Asia/Yekaterinburg",
            },
                ["Fiji Standard Time"] = new[]
            {
                "Pacific/Fiji",
            },
                ["FLE Standard Time"] = new[]
            {
                "Europe/Kiev",
                "Europe/Helsinki",
                "Europe/Mariehamn",
                "Europe/Riga",
                "Europe/Simferopol",
                "Europe/Sofia",
                "Europe/Tallinn",
                "Europe/Uzhgorod",
                "Europe/Vilnius",
                "Europe/Zaporozhye",
            },
                ["Georgian Standard Time"] = new[]
            {
                "Asia/Tbilisi",
            },
                ["GMT Standard Time"] = new[]
            {
                "Europe/London",
                "Atlantic/Canary",
                "Atlantic/Faeroe",
                "Atlantic/Madeira",
                "Europe/Dublin",
                "Europe/Guernsey",
                "Europe/Isle_of_Man",
                "Europe/Jersey",
                "Europe/Lisbon",
            },
                ["Greenland Standard Time"] = new[]
            {
                "America/Godthab",
            },
                ["Greenwich Standard Time"] = new[]
            {
                "GMT",
                "Africa/Abidjan",
                "Africa/Accra",
                "Africa/Bamako",
                "Africa/Banjul",
                "Africa/Bissau",
                "Africa/Conakry",
                "Africa/Dakar",
                "Africa/El_Aaiun",
                "Africa/Freetown",
                "Africa/Lome",
                "Africa/Monrovia",
                "Africa/Nouakchott",
                "Africa/Ouagadougou",
                "Africa/Sao_Tome",
                "Atlantic/Reykjavik",
                "Atlantic/St_Helena",
            },
                ["GTB Standard Time"] = new[]
            {
                "Europe/Bucharest",
                "Europe/Athens",
                "Europe/Chisinau",
            },
                ["Marquesas Standard Time"] = new[]
                {
                    "Pacific/Marquesas"
                },
                ["Hawaiian Standard Time"] = new[]
            {
                "US/Hawaii",
                "Pacific/Honolulu",
                "Pacific/Johnston",
                "Pacific/Rarotonga",
                "Pacific/Tahiti",
                "Etc/GMT+10",
            },
                ["Aleutian Standard Time"] = new[]
                {
                    "US/Hawaii",
                },
                ["India Standard Time"] = new[]
            {
                "Asia/Calcutta",
                "Asia/Kolkata",
            },
                ["Iran Standard Time"] = new[]
            {
                "Asia/Tehran",
            },
                ["Israel Standard Time"] = new[]
            {
                "Asia/Jerusalem",
            },
                ["Jordan Standard Time"] = new[]
            {
                "Asia/Amman",
            },
                ["Kaliningrad Standard Time"] = new[]
            {
                "Europe/Minsk",
                "Europe/Kaliningrad",
            },
                ["Korea Standard Time"] = new[]
            {
                "Asia/Seoul",
            },
                ["North Korea Standard Time"] = new[]
            {
                "Asia/Pyongyang",
            },
                ["Line Islands Standard Time"] = new[]
            {
                "Pacific/Kiritimati",
            },
                ["Magadan Standard Time"] = new[]
            {
                "Asia/Kamchatka",
                "Asia/Anadyr",
                "Asia/Magadan",
            },
                ["Mauritius Standard Time"] = new[]
            {
                "Indian/Mauritius",
                "Indian/Mahe",
                "Indian/Reunion",
            },
                ["Middle East Standard Time"] = new[]
            {
                "Asia/Beirut",
            },
                ["Montevideo Standard Time"] = new[]
            {
                "America/Montevideo",
            },
                ["Morocco Standard Time"] = new[]
            {
                "Africa/Casablanca",
            },
                ["Mountain Standard Time"] = new[]
            {
                "America/Denver",
                "America/Boise",
                "America/Cambridge_Bay",
                "America/Edmonton",
                "America/Inuvik",
                "America/Ojinaga",
                "America/Shiprock",
                "America/Yellowknife",
                "MST7MDT",
            },
                ["Mountain Standard Time (Mexico)"] = new[]
            {
                "America/Chihuahua",
                "America/Mazatlan",
            },
                ["Myanmar Standard Time"] = new[]
            {
                "Asia/Rangoon",
                "Indian/Cocos",
            },
                ["N. Central Asia Standard Time"] = new[]
            {
                "Asia/Novosibirsk",
                "Asia/Novokuznetsk",
                "Asia/Omsk",
            },
                ["Namibia Standard Time"] = new[]
            {
                "Africa/Windhoek",
            },
                ["Nepal Standard Time"] = new[]
            {
                "Asia/Katmandu",
            },
                ["New Zealand Standard Time"] = new[]
            {
                "Pacific/Auckland",
                "Antarctica/McMurdo",
                "Antarctica/South_Pole",
            },
                ["Newfoundland Standard Time"] = new[]
            {
                "America/St_Johns",
            },
                ["North Asia East Standard Time"] = new[]
            {
                "Asia/Irkutsk",
            },
                ["North Asia Standard Time"] = new[]
            {
                "Asia/Krasnoyarsk",
            },
                ["Pacific SA Standard Time"] = new[]
            {
                "America/Santiago",
                "Antarctica/Palmer",
            },
                ["Pacific Standard Time"] = new[]
            {
                "US/Pacific",
                "America/Dawson",
                "America/Los_Angeles",
                "America/Tijuana",
                "America/Vancouver",
                "America/Whitehorse",
                "Canada/Pacific",
                "Pacific/Pitcairn",
                "PST8PDT",
            },
                ["Yukon Standard Time"] = new[]
                {
                    "Canada/Yukon"
                },
                ["Pacific Standard Time (Mexico)"] = new[]
            {
                "America/Santa_Isabel",
            },
                ["Pakistan Standard Time"] = new[]
            {
                "Asia/Karachi",
            },
                ["Paraguay Standard Time"] = new[]
            {
                "America/Asuncion",
            },
                ["Romance Standard Time"] = new[]
            {
                "Europe/Paris",
                "Africa/Ceuta",
                "Europe/Brussels",
                "Europe/Copenhagen",
                "Europe/Madrid",
            },
                ["Russian Standard Time"] = new[]
            {
                "Europe/Moscow",
                "Europe/Samara",
                "Europe/Volgograd",
            },
                ["SA Eastern Standard Time"] = new[]
            {
                "America/Recife",
                "America/Araguaina",
                "America/Belem",
                "America/Cayenne",
                "America/Fortaleza",
                "America/Maceio",
                "America/Paramaribo",
                "America/Santarem",
                "Antarctica/Rothera",
                "Atlantic/Stanley",
                "Etc/GMT+3",
            },
                ["SA Pacific Standard Time"] = new[]
            {
                "America/Lima",
                "America/Bogota",
                "America/Cayman",
                "America/Coral_Harbour",
                "America/Guayaquil",
                "America/Jamaica",
                "America/Panama",
                "America/Port-au-Prince",
                "Etc/GMT+5",
            },
                ["Haiti Standard Time"] = new[]
                {
                    "America/Port-au-Prince" 
                },
                ["Cuba Standard Time"] = new[]
                {
                    "America/Havana"
                },
                ["Turks And Caicos Standard Time"] = new[]
                {
                    "Etc/GMT-6"
                },
                ["SA Western Standard Time"] = new[]
            {
                "America/Puerto_Rico",
                "America/Anguilla",
                "America/Antigua",
                "America/Aruba",
                "America/Barbados",
                "America/Blanc-Sablon",
                "America/Boa_Vista",
                "America/Curacao",
                "America/Dominica",
                "America/Eirunepe",
                "America/Grenada",
                "America/Guadeloupe",
                "America/Guyana",
                "America/Kralendijk",
                "America/La_Paz",
                "America/Lower_Princes",
                "America/Manaus",
                "America/Marigot",
                "America/Martinique",
                "America/Montserrat",
                "America/Port_of_Spain",
                "America/Porto_Velho",
                "America/Rio_Branco",
                "America/Santo_Domingo",
                "America/St_Barthelemy",
                "America/St_Kitts",
                "America/St_Lucia",
                "America/St_Thomas",
                "America/St_Vincent",
                "America/Tortola",
                "Etc/GMT+4",
            },
                ["Samoa Standard Time"] = new[]
            {
                "Pacific/Apia",
            },
                ["SE Asia Standard Time"] = new[]
            {
                "Asia/Saigon",
                "Antarctica/Davis",
                "Asia/Bangkok",
                "Asia/Hovd",
                "Asia/Jakarta",
                "Asia/Phnom_Penh",
                "Asia/Pontianak",
                "Asia/Vientiane",
                "Indian/Christmas",
                "Etc/GMT-7",
            },
                ["Singapore Standard Time"] = new[]
            {
                "Asia/Singapore",
                "Asia/Brunei",
                "Asia/Kuala_Lumpur",
                "Asia/Kuching",
                "Asia/Makassar",
                "Asia/Manila",
                "Etc/GMT-8",
            },
                ["South Africa Standard Time"] = new[]
            {
                "Africa/Lusaka",
                "Africa/Blantyre",
                "Africa/Bujumbura",
                "Africa/Gaborone",
                "Africa/Harare",
                "Africa/Johannesburg",
                "Africa/Kigali",
                "Africa/Lubumbashi",
                "Africa/Maputo",
                "Africa/Maseru",
                "Africa/Mbabane",
                "Etc/GMT-2",
            },
                ["Sri Lanka Standard Time"] = new[]
            {
                "Asia/Colombo",
            },
                ["Syria Standard Time"] = new[]
            {
                "Asia/Damascus",
            },
                ["Taipei Standard Time"] = new[]
            {
                "Asia/Taipei",
            },
                ["Tasmania Standard Time"] = new[]
            {
                "Australia/Hobart",
                "Australia/Currie",
            },
                ["Tokyo Standard Time"] = new[]
            {
                "Asia/Tokyo",
                "Asia/Dili",
                "Asia/Jayapura",
                "Pacific/Palau",
                "Etc/GMT-9",
            },
                ["Tonga Standard Time"] = new[]
            {
                "Pacific/Tongatapu",
                "Pacific/Enderbury",
                "Pacific/Fakaofo",
                "Etc/GMT-13",
            },
                ["Turkey Standard Time"] = new[]
            {
                "Europe/Istanbul",
            },
                ["Ulaanbaatar Standard Time"] = new[]
            {
                "Asia/Ulaanbaatar",
                "Asia/Choibalsan",
            },
                ["US Eastern Standard Time"] = new[]
            {
                "America/Indianapolis",
                "America/Indiana/Indianapolis",
                "America/Indiana/Marengo",
                "America/Indiana/Vevay",
            },
                ["US Mountain Standard Time"] = new[]
            {
                "US/Mountain",
                "America/Creston",
                "America/Dawson_Creek",
                "America/Hermosillo",
                "America/Phoenix",
                "US/Arizona",
                "Etc/GMT+7",
            },
                ["Gambier Time"] = new[]
                {
                    "Pacific/Gambier"
                },
                ["Venezuela Standard Time"] = new[]
            {
                "America/Caracas",
            },
                ["Vladivostok Standard Time"] = new[]
            {
                "Asia/Vladivostok",
                "Asia/Sakhalin",
            },
                ["W. Australia Standard Time"] = new[]
            {
                "Australia/Perth",
                "Antarctica/Casey",
            },
                ["W. Central Africa Standard Time"] = new[]
            {
                "Africa/Kinshasa",
                "Africa/Algiers",
                "Africa/Bangui",
                "Africa/Brazzaville",
                "Africa/Douala",
                "Africa/Lagos",
                "Africa/Libreville",
                "Africa/Luanda",
                "Africa/Malabo",
                "Africa/Ndjamena",
                "Africa/Niamey",
                "Africa/Porto-Novo",
                "Africa/Tunis",
                "Etc/GMT-1",
            },
                ["W. Europe Standard Time"] = new[]
            {
                "Europe/Berlin",
                "Africa/Tripoli",
                "Arctic/Longyearbyen",
                "Europe/Amsterdam",
                "Europe/Andorra",
                "Europe/Gibraltar",
                "Europe/Luxembourg",
                "Europe/Malta",
                "Europe/Monaco",
                "Europe/Oslo",
                "Europe/Rome",
                "Europe/San_Marino",
                "Europe/Stockholm",
                "Europe/Vaduz",
                "Europe/Vatican",
                "Europe/Vienna",
                "Europe/Zurich",
            },
                ["West Asia Standard Time"] = new[]
            {
                "Asia/Tashkent",
                "Antarctica/Mawson",
                "Asia/Aqtau",
                "Asia/Aqtobe",
                "Asia/Ashgabat",
                "Asia/Dushanbe",
                "Asia/Oral",
                "Asia/Samarkand",
                "Indian/Kerguelen",
                "Indian/Maldives",
                "Etc/GMT-5",
            },
                ["West Pacific Standard Time"] = new[]
            {
                "Pacific/Port_Moresby",
                "Antarctica/DumontDUrville",
                "Pacific/Guam",
                "Pacific/Saipan",
                "Pacific/Truk",
                "Etc/GMT-10",
            },
                ["Yakutsk Standard Time"] = new[]
            {
                "Asia/Yakutsk",
            },
                ["UTC"] = new[]
            {
                "America/Danmarkshavn",
                "Etc/GMT",
            },
                ["UTC+12"] = new[]
            {
                "Pacific/Tarawa",
                "Pacific/Funafuti",
                "Pacific/Kwajalein",
                "Pacific/Majuro",
                "Pacific/Nauru",
                "Pacific/Wake",
                "Pacific/Wallis",
                "Etc/GMT-12",
            },
                ["UTC-02"] = new[]
            {
                "America/Noronha",
                "Atlantic/South_Georgia",
                "Etc/GMT+2",
            },
                ["UTC-08"] = new[]
                {
                    "Etc/GMT-8"
                },
                ["UTC-09"] = new[]
                {
                    "Etc/GMT-9"
                },
                ["UTC-11"] = new[]
            {
                "Pacific/Pago_Pago",
                "Pacific/Midway",
                "Pacific/Niue",
                "Etc/GMT+11",
            },

            };

            _olsonToWindows = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            foreach (var kvp in _windowsToOlson)
            {
                foreach (var olson in kvp.Value)
                {
                    _olsonToWindows[olson] = kvp.Key;
                }
            }

            // These are deprecated or have two windows-id to one Olson
            _windowsToOlson["Mid-Atlantic Standard Time"] = new[]
            {
                "America/Noronha",
                "Atlantic/South_Georgia",
                "Etc/GMT+2",
            };
            _windowsToOlson["Belarus Standard Time"] = new[]
            {
                "Europe/Minsk",
            };
            _windowsToOlson["Libya Standard Time"] = new[]
            {
                "Africa/Tripoli"
            };

            _windowsToOlson["Russia Time Zone 3"] = new[]
            {
                "Europe/Moscow",
                "Europe/Samara",
                "Europe/Volgograd",
            };
            _windowsToOlson["Russia Time Zone 10"] = new[]
            {
                "Etc/GMT-11",
            };
            _windowsToOlson["Russia Time Zone 11"] = new[]
            {
                "Asia/Kamchatka",
                "Asia/Anadyr",
                "Asia/Magadan",
            };
            _windowsToOlson["Kamchatka Standard Time"] = new[]
            {
                "Asia/Kamchatka",
            };
        }
        public static string? ToOlsonTimeZone(this TimeZoneInfo tzi)
        {
            var olsonIds = ToOlsonTimeZones(tzi);
            if (olsonIds != null && olsonIds.Length > 0)
                return olsonIds[0];

            return null;
        }

        public static string[]? ToOlsonTimeZones(this TimeZoneInfo tzi)
        {
            if (tzi == null)
                return null;

            if (_windowsToOlson.TryGetValue(tzi.Id, out string[]? olsonIds) && olsonIds.Length > 0)
                return olsonIds;

            return null;
        }

        public static string? FromOlsonToTimeZoneId(string? olsonTimezone)
        {
            if (string.IsNullOrWhiteSpace(olsonTimezone))
                return null;

            if (_olsonToWindows.TryGetValue(olsonTimezone, out string? id))
                return id;

            // Try by prefix now
            foreach (var kvp in _olsonToWindows)
            {
                if (olsonTimezone.StartsWith(kvp.Key))
                    return kvp.Value;
            }

            return null;
        }

        public static TimeZoneInfo? FromOlsonToTimeZoneInfo(string? olsonTimezone)
        {
            string? id = FromOlsonToTimeZoneId(olsonTimezone);
            if (id == null)
                return null;

            return TimeZoneInfo.FindSystemTimeZoneById(id);
        }
    }
}

