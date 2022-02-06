﻿using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;

namespace CalbucciLib.ExtensionsGalore2
{
    public static class StringExtensions
    {
        private static readonly string[] _loremIpsum = new[]
        {
            "Lorem", "ipsum", "dolor", "sit", "amet,", "consectetur", "adipiscing", "elit,", "sed", "do", "eiusmod", "tempor",
            "incididunt", "ut", "labore", "et", "dolore", "magna", "aliqua.", "Ut", "enim", "ad", "minim", "veniam,", "quis",
            "nostrud", "exercitation", "ullamco", "laboris", "nisi", "ut", "aliquip", "ex", "ea", "commodo", "consequat.", "Duis",
            "aute", "irure", "dolor", "in", "reprehenderit", "in", "voluptate", "velit", "esse", "cillum", "dolore", "eu",
            "fugiat", "nulla", "pariatur.", "Excepteur", "sint", "occaecat", "cupidatat", "non", "proident,", "sunt", "in",
            "culpa", "qui", "officia", "deserunt", "mollit", "anim", "id", "est", "laborum."
        };

        private static string _currencySymbols = "$¢£¥₠€€";


        private static readonly List<string> _typicalHomepages;

        public static string? DefaultHtmlifyLinkAttributes { get; set; }

        static StringExtensions()
        {
            _typicalHomepages = new List<string>();
            var typicalExtensions = new[]
            {"asp", "aspx", "cfm", "htm", "html", "xhtml", "jsp", "action", "pl", "php", "py", "rb", "xml"};
            var typicalHomePaths = new[] { "home", "index", "default" };
            foreach (var thp in typicalHomePaths)
            {
                foreach (var text in typicalExtensions)
                {
                    _typicalHomepages.Add("/" + typicalHomePaths + "." + text);
                }
            }
        }

        // ==========================================================================
        //
        //    Static
        //
        // ==========================================================================
        public static int GetHashCode(params string[] strings)
        {
            var hash = 0;
            foreach (var str in strings)
            {
                hash = ((hash << 5) + hash);
                hash ^= str.GetHashCode();
            }
            return hash;
        }


        // ==========================================================================
        //
        //    Create string
        //
        // ==========================================================================
        public static string GenerateLoremIpsum(int wordCount)
        {
            if (wordCount <= 0)
                return "";

            var sb = new StringBuilder(7 * wordCount);
            while (wordCount > _loremIpsum.Length)
            {
                sb.Append(string.Join(" ", _loremIpsum));
                sb.Append(' ');
                wordCount -= _loremIpsum.Length;
            }
            sb.Append(string.Join(" ", _loremIpsum.Take(wordCount)));

            if (sb[^1] == ',')
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public static string CreateTRTD(params string?[]? tdValues)
        {
            if (tdValues == null || tdValues.Length == 0)
                return "";

            var length = 10 + 10 * tdValues.Length + tdValues.Sum(v => v == null ? 0 : v.Length);
            var sb = new StringBuilder(length);
            sb.Append("<tr>");
            foreach (var tdValue in tdValues)
            {
                sb.Append("<td>");
                sb.Append(tdValue.HtmlEncode());
                sb.Append("</td>");
            }
            sb.Append("</tr>");

            return sb.ToString();
        }




        // ==========================================================================
        //
        //    HTML & URL Encoding
        //
        // ==========================================================================
        public static string HtmlEncode(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return HttpUtility.HtmlEncode(str);
        }

        /// <summary>
        /// Encodes a string to be HTML safe to be used inside a Textarea. Same thing as HtmlEncode, except that NL and CR are not encoded.
        /// </summary>
        public static string HtmlEncodeTextarea(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return EscapeCharacters(str, "<>&",
                new[] { "&lt;", "&gt;", "&amp;" }, false)
                ?? string.Empty;
        }

        /// <summary>
        /// Encodes a string to be HTML safe to be used inside a PRE element.
        /// </summary>
	    public static string HtmlEncodePre(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return EscapeCharacters(str,
                "<>&\"\n\r\t",
                new[] { "&lt;", "&gt;", "&amp;", "&quot;", "<br>", "", "&nbsp;&nbsp;&nbsp;&nbsp;" }, false)
                ?? string.Empty;
        }

        public static string HtmlDecode(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            return HttpUtility.HtmlDecode(str);
        }

        public static string UrlEncode(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return HttpUtility.UrlEncode(str);
        }

        public static string UrlDecode(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return "";

            return HttpUtility.UrlDecode(str);
        }

        public static string? DefaultLinkifier(string? token)
        {
            if (token == null || token.Length < 5)
                return null;

            var buildLink = (string link, string text) =>
            {
                if (!string.IsNullOrWhiteSpace(DefaultHtmlifyLinkAttributes))
                    return $"<a href=\"{link}\" {DefaultHtmlifyLinkAttributes}>{text}</a>";
                return $"<a href=\"{link}\">{text}</a>";
            };

            var lastChar = token[^1];
            string? suffix = null;
            if (".?!),([]{};:'\"<>".Contains(lastChar))
            {
                token = token[..^1];
                suffix = lastChar.ToString();
            }

            if (token.StartsWith("http://", StringComparison.InvariantCultureIgnoreCase)
                || token.StartsWith("https://", StringComparison.InvariantCultureIgnoreCase))
            {
                if (token.Length < 12)
                    return null;
                return buildLink(token, token.TruncateTrimLink(150)!) + suffix;

            }
            if (token.StartsWith("www.", StringComparison.InvariantCultureIgnoreCase))
            {
                if (token.Length < 10)
                    return null;
                var link = "https://" + token;
                if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
                    return null;

                return buildLink(link, token) + suffix;
            }

            if (token.Contains('.'))
            {
                var link = "https://" + token;
                if (!Uri.IsWellFormedUriString(link, UriKind.Absolute))
                    return null;
                try
                {
                    var uri = new Uri(link);
                    if (!Validate.IsValidDomain(uri.Host, true))
                        return null;
                }
                catch { }

                return buildLink(link, token) + suffix;
            }

            return null;
        }

        public static string Htmlify(this string? str, Func<string, string?>? tokenHtmlifier = null)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            if (tokenHtmlifier == null)
                tokenHtmlifier = DefaultLinkifier;

            bool appendSpace = false;

            var sb = new StringBuilder(str.Length + 50);
            for (int i = 0; i < str.Length; i++)
            {
                var nextWhitespace = str.IndexOfWhitespace(i);
                string token;
                if (nextWhitespace == -1)
                {
                    token = str[i..].Trim();
                    i = str.Length;
                }
                else
                {
                    token = str[i..nextWhitespace].Trim();
                    i = nextWhitespace;
                    appendSpace = true;
                }

                if (token.Length == 0)
                    continue;

                var html = tokenHtmlifier(token);
                sb.Append(html ?? token.HtmlEncode());
                if (appendSpace)
                    sb.Append(' ');
            }

            return sb.ToString().Trim();

        }


        // ==========================================================================
        //
        //   Encoding/Decoding/Escaping/Unescaping
        //
        // ==========================================================================
        [return: NotNullIfNotNull("str")]
        public static string? EscapeCString(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            return EscapeCharacters(str,
                "\\\"\a\b\t\n\v\f\r",
                new[] { "\\\\", "\\\"", "\\a", "\\b", "\\t", "\\n", "\\v", "\\f", "\\r" },
                true);
        }

        [return: NotNullIfNotNull("str")]
        public static string? UnescapeCString(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var len = str.Length;
            var lenMinus = len - 1;

            StringBuilder? sb = null;
            var start = 0;
            var i = 0;
            while (i < len)
            {
                char c = str[i];
                if (c != '\\')
                {
                    i++;
                    continue;
                }
                if (i > start)
                {
                    if (sb == null)
                        sb = new StringBuilder(len);
                    sb.Append(str, start, i - start);
                    start = i;
                }
                if (i >= lenMinus)
                    break;
                i++;

                string? newChar = null;
                switch (str[i])
                {
                    case 'a': newChar = "\a"; break;
                    case 'b': newChar = "\b"; break;
                    case 'f': newChar = "\f"; break;
                    case 'v': newChar = "\v"; break;
                    case 'n': newChar = "\n"; break;
                    case 'r': newChar = "\r"; break;
                    case 't': newChar = "\t"; break;
                    case '\\': newChar = "\\"; break;
                    case 'x':
                    case 'X':
                        // Two hex digits
                        if (i + 3 < len)
                        {
                            byte b = ByteExtensions.FromHex(str.Substring(i + 1, 2));
                            if (b != 0)
                            {
                                newChar = new string((char)b, 1);
                            }
                        }
                        break;
                    case 'u': // UTF-16 encoded
                        if (i + 5 < len)
                        {
                            c = (char)Convert.ToInt16(str.Substring(i + 1, 4));
                            newChar = new string(c, 1);
                        }
                        break;
                    case 'U': // UTF-32 encoded
                        if (i + 9 < len)
                        {
                            var utf32 = Convert.ToInt32(str.Substring(i + 1, 8));
                            newChar = char.ConvertFromUtf32(utf32);
                        }
                        break;
                    default:
                        if (str[i] >= '0' && str[0] <= '8')
                        {
                            // Octal (3 digits)
                            if (i + 4 < len)
                            {
                                // Only if the following character is also a digit
                                if (!char.IsDigit(str[i + 1]))
                                {
                                    c = (char)Convert.ToInt16(str.Substring(i + 1, 3), 8);
                                    newChar = new string(c, 1);
                                    break;
                                }
                            }
                        }
                        if (sb == null)
                            sb = new StringBuilder(len);
                        sb.Append(str[i]);
                        break;
                }
                if (newChar != null)
                {
                    if (sb == null)
                        sb = new StringBuilder(len);
                    sb.Append(newChar);
                }
                i++;
                start = i;
            }
            if (sb == null)
                return str;

            if (i > start)
                sb.Append(str, start, i - start);
            return sb.ToString();

        }

        /// <summary>
        /// Escape a string to be used as a literal string in JSON or JavaScript
        /// </summary>
        public static string EscapeJson(this string? str, char quoteCharacter = '\"')
        {
            if (str == null)
                return "null";
            if (str.Length == 0)
                return new string(quoteCharacter, 2);

            if (quoteCharacter == '\"')
            {
                return
                    quoteCharacter
                    + EscapeCharacters(str,
                        "\\\"\a\b\t\n\v\f\r",
                        new[] { "\\\\", "\\\"", "\\a", "\\b", "\\t", "\\n", "\\v", "\\f", "\\r" }, true)
                    + quoteCharacter;

            }
            else if (quoteCharacter == '\'')
            {
                return
                    quoteCharacter
                    + EscapeCharacters(str,
                        "\\'\a\b\t\n\v\f\r",
                        new[] { "\\\\", "\\'", "\\a", "\\b", "\\t", "\\n", "\\v", "\\f", "\\r" }, true)
                    + quoteCharacter;
            }

            return quoteCharacter
                   + EscapeCharacters(str,
                       "\\" + quoteCharacter + "\a\b\t\n\v\f\r",
                       new[] { "\\\\", "\\" + quoteCharacter, "\\a", "\\b", "\\t", "\\n", "\\v", "\\f", "\\r" }, true)
                   + quoteCharacter;
        }


        [return: NotNullIfNotNull("str")]
        public static string? EscapeStringFormat(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;
            return EscapeCharacters(str,
                "{", new[] { "{{" }, false);
        }

        [return: NotNullIfNotNull("str")]
        public static string? EscapeCSV(this string? str, bool escapeControlCharacters = false)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var len = str.Length;
            StringBuilder? sb = null;

            int i = 0, start = 0;
            while (i < len)
            {
                string? cstr = null;
                char c = str[i];
                if (c == ',')
                {
                    cstr = ","; // don't escape the comma, but indicate we will double-quote the string
                }
                else if (c == '\"')
                {
                    cstr = "\\\""; // escape double-quote
                }
                else if (c == '\\')
                {
                    cstr = "\\\\"; // 
                }
                else if (c < 32 && escapeControlCharacters)
                {
                    // C# / C++ / JavaScript
                    switch (c)
                    {
                        case '\r': cstr = "\\r"; break;
                        case '\t': cstr = "\\t"; break;
                        case '\n': cstr = "\\n"; break;
                        case '\a': cstr = "\\a"; break;
                        case '\b': cstr = "\\b"; break;
                        case '\v': cstr = "\\v"; break;
                        case '\f': cstr = "\\f"; break;
                        default:
                            cstr = "\\x" + ((byte)c).ToHex();
                            break;
                    }

                }
                else
                {
                    i++;
                    continue;
                }

                if (sb == null)
                {
                    sb = new StringBuilder((21 * len) / 20); // 5%
                    sb.Append('\"');
                }
                if (i > start)
                {
                    sb.Append(str, start, i - start);
                }

                sb.Append(cstr);
                i++;
                start = i;
            }
            if (sb == null)
                return str;

            if (i > start)
                sb.Append(str, start, i - start);

            sb.Append('\"'); // close quote

            return sb.ToString();

        }

        [return: NotNullIfNotNull("str")]
        public static string? UnescapeCSVField(this string? str, bool trimWhitespace = true)
        {
            return UnescapeQuoted(str, trimWhitespace);
        }

        [return: NotNullIfNotNull("str")]
        public static string? UnescapeTabField(this string? str, bool trimWhitespace = true)
        {
            return UnescapeQuoted(str, trimWhitespace);
        }

        [return: NotNullIfNotNull("str")]
        private static string? UnescapeQuoted(string? str, bool trimWhitespace)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            bool startWithQuote = false;
            int firstNonWhitespace = str.IndexOfNonWhitespace();
            if (firstNonWhitespace == -1)
                return str;

            int pos = 0;
            if (str[firstNonWhitespace] == '\"')
            {
                startWithQuote = true;
                pos = firstNonWhitespace + 1;
            }
            else
            {
                if (trimWhitespace)
                {
                    pos = firstNonWhitespace;
                }
            }

            StringBuilder sb = new StringBuilder(str.Length);
            for (int i = pos; i < str.Length; i++)
            {
                var c = str[i];
                if (c == '\\')
                {
                    i++;
                    if (i == str.Length)
                        break;

                    c = str[i];
                    switch (c)
                    {
                        case 'n': c = '\n'; break;
                        case 'r': c = '\r'; break;
                        case 't': c = '\t'; break;
                        case 'a': c = '\t'; break;
                        case 'b': c = '\t'; break;
                        case 'v': c = '\t'; break;
                        case 'f': c = '\t'; break;
                        case 'x':
                        case 'X':
                            if (i + 2 < str.Length)
                            {
                                c = (char)ByteExtensions.FromHex(str.Substring(i, 2));
                            }
                            break;
                    }
                    if (c != '\0')
                        sb.Append(c);
                }
                else if (startWithQuote && c == '\"')
                {
                    return sb.ToString();
                }
                else
                {
                    sb.Append(c);
                }
            }

            if (trimWhitespace)
                return sb.ToString().Trim();

            return sb.ToString();
        }

        [return: NotNullIfNotNull("str")]
        public static string? EscapeTabDelimited(this string? str, bool escapeControlCharacters = false)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int len = str.Length;
            StringBuilder? sb = null;

            int i = 0, start = 0;
            while (i < len)
            {
                string? cstr = null;
                char c = str[i];
                if (c == '\t')
                {
                    cstr = "\\t";
                }
                else if (c == '\\')
                {
                    cstr = "\\\\"; // 
                }
                else if (c < 32 && escapeControlCharacters)
                {
                    // C# / C++ / JavaScript
                    switch (c)
                    {
                        case '\r': cstr = "\\r"; break;
                        case '\n': cstr = "\\n"; break;
                        case '\a': cstr = "\\a"; break;
                        case '\b': cstr = "\\b"; break;
                        case '\v': cstr = "\\v"; break;
                        case '\f': cstr = "\\f"; break;
                        default:
                            cstr = "\\x" + ((byte)c).ToHex();
                            break;
                    }

                }
                else
                {
                    i++;
                    continue;
                }

                if (sb == null)
                {
                    sb = new StringBuilder((21 * len) / 20); // 5%
                }
                if (i > start)
                {
                    sb.Append(str, start, i - start);
                }

                sb.Append(cstr);
                i++;
                start = i;
            }
            if (sb == null)
                return str;

            if (i > start)
                sb.Append(str, start, i - start);

            return sb.ToString();
        }

        [return: NotNullIfNotNull("str")]
        public static string? EscapeCDATA(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var parts = str.Split(new string[] { "]]>" }, StringSplitOptions.None);
            if (parts.Length == 1)
                return str; // nothing to escape

            return "<![CDATA["
                   + string.Join("]]]]><![CDATA[>", parts)
                   + "]]>";
        }






        // ==========================================================================
        //
        //   Has*** / Contains*** / IndexOf*** / Ends*** / Starts***
        //
        // ==========================================================================
        public static bool HasLowerCase(this string? str)
        {
            return str != null && str.Any(char.IsLower);
        }

        public static bool HasUpperCase(this string? str)
        {
            return str != null && str.Any(char.IsUpper);
        }

        public static bool EqualsCaseInsentive(this string? str, string? other)
        {
            if (str == null)
            {
                if (other == null)
                    return true;
                return false;
            }
            else if (other == null)
                return false;
            return str.Equals(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EqualsAny(this string? str, IList<string?>? matchList,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Count == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.Equals(m, stringComparison));
        }

        public static bool EqualsAny(this string? str, params string?[]? matchList)
        {
            return EqualsAny(str, matchList, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsAny(this string? str, IList<string?>? matchList,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Count == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.IndexOf(m, stringComparison) >= 0);
        }

        public static bool ContainsAny(this string? str, params string?[]? matchList)
        {
            return ContainsAny(str, matchList, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool ContainsAny(this string? str, char[]? chars)
        {
            if (chars == null || chars.Length == 0 || string.IsNullOrEmpty(str))
                return false;

            return str.IndexOfAny(chars) >= 0;
        }

        public static bool StartsWithCaseInsensitive(this string? str, string? other)
        {
            if (str == null || other == null)
                return false;
            return str.StartsWith(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool StartsWithAny(this string? str, IList<string?>? matchList,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Count == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.StartsWith(m, stringComparison));
        }

        public static bool StartsWithAny(this string? str, params string?[]? matchList)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Length == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.StartsWith(m, StringComparison.CurrentCulture));
        }

        public static bool StartsWithAnyIgnoreCase(this string? str, params string?[]? matchList)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Length == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.StartsWith(m, StringComparison.InvariantCultureIgnoreCase));
        }

        public static bool StartsWithAny(this string? str, char[]? chars)
        {
            if (string.IsNullOrEmpty(str) || chars == null || chars.Length == 0)
                return false;

            return chars.Any(c => str[0] == c);
        }

        public static int IndexOfAny(this string? str, IList<string?>? matchList, int start = 0, int count = int.MaxValue,
            StringComparison stringComparsion = StringComparison.InvariantCultureIgnoreCase)
        {
            if (matchList == null || matchList.Count == 0)
                return -1;

            matchList = matchList.Where(i => !string.IsNullOrEmpty(i)).ToList();
            if (matchList.Count == 0)
                return -1;

            var longest = matchList.Max(i => i?.Length ?? 0);
            var shortest = matchList.Min(i => i?.Length ?? int.MaxValue);

            if (string.IsNullOrEmpty(str) || (start + shortest) > str.Length)
                return -1;

            int end;
            if (count == int.MaxValue)
                end = str.Length;
            else
            {
                end = start + count - shortest;
                if (end > str.Length)
                    end = str.Length;
            }

            for (int i = start; i < end; i++)
            {
                foreach (var ml in matchList)
                {
                    if (ml == null && str == null)
                        return i;

                    if (i + ml!.Length > str.Length)
                        continue;

                    if (string.Compare(str, i, ml, 0, ml.Length, stringComparsion) == 0)
                        return i;
                }
            }

            return -1;
        }

        public static bool EndsWithCaseInsensitive(this string? str, string? other)
        {
            if (str == null || other == null)
                return false;
            return str.EndsWith(other, StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool EndsWithAny(this string? str, IList<string?>? matchList,
            StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Count == 0)
                return false;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.EndsWith(m, stringComparison));

        }

        public static bool EndsWithAny(this string? str, bool ignoreCase = true, params string?[]? matchList)
        {
            if (string.IsNullOrEmpty(str) || matchList == null || matchList.Length == 0)
                return false;

            var stringComparison = ignoreCase
                ? StringComparison.InvariantCultureIgnoreCase
                : StringComparison.InvariantCulture;

            return matchList.Any(m => !string.IsNullOrEmpty(m) && str.EndsWith(m, stringComparison));
        }

        public static bool EndsWithAny(this string? str, char[]? chars)
        {
            if (string.IsNullOrEmpty(str) || chars == null || chars.Length == 0)
                return false;

            return chars.Any(c => str[0] == c);
        }

        /// <summary>
        /// Compare two strings ignoring all whitespace ("A dog" = "Adog" = " A  dog ").
        /// </summary>
        public static bool CompareNonWhitespace(this string? str, string? str2, bool ignoreCase = true)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                if (string.IsNullOrWhiteSpace(str2))
                    return true;
                return false;
            }
            if (string.IsNullOrWhiteSpace(str2))
                return false;

            int i2 = 0;
            for (int i1 = 0; i1 < str.Length; i1++)
            {
                char c1 = str[i1];
                if (char.IsWhiteSpace(c1) || char.IsControl(c1))
                    continue;

                if (i2 == str2.Length)
                    return false;

                char c2 = str2[i2];
                while (char.IsWhiteSpace(c2) || char.IsControl(c2))
                {
                    i2++;
                    if (i2 == str2.Length)
                        return false;
                    c2 = str2[i2];
                }

                if (c1 == c2
                    || (ignoreCase && char.ToLowerInvariant(c1) == char.ToLowerInvariant(c2)))
                {
                    i2++;
                    continue;
                }

                return false;
            }

            while (i2 < str2.Length)
            {
                var c2 = str2[i2];
                if (!char.IsWhiteSpace(c2) && !char.IsWhiteSpace(c2))
                    return false;
                i2++;
            }

            return true;
        }


        // ==========================================================================
        //
        //   IndexOf
        //
        // ==========================================================================
        public static int IndexOf(this string? str, Func<char, bool> expresssion, int start = 0, int count = int.MaxValue)
        {
            if (string.IsNullOrEmpty(str) || start >= str.Length)
                return -1;

            int end;
            if (count == int.MaxValue)
                end = str.Length;
            else
            {
                end = start + count;
                if (end > str.Length)
                    end = str.Length;
            }

            for (int i = start; i < end; i++)
            {
                char c = str[i];
                if (expresssion(c))
                    return i;
            }
            return -1;
        }

        public static int LastIndexOf(this string? str, Func<char, bool> expression, int start = int.MaxValue, int count = int.MaxValue)
        {
            if (string.IsNullOrEmpty(str))
                return -1;

            if (start >= str.Length)
                start = str.Length - 1;

            int end = start - count;
            if (end < 0)
                end = -1;
            for (int i = start; i > end; i--)
            {
                char c = str[i];
                if (expression(c))
                    return i;
            }
            return -1;
        }

        public static int IndexOfWhitespace(this string? str, int start = 0, int count = int.MaxValue)
        {
            return str.IndexOf(c => char.IsWhiteSpace(c) || char.IsControl(c), start, count);
        }

        public static int IndexOfNonWhitespace(this string? str, int start = 0, int count = int.MaxValue)
        {
            return str.IndexOf(c => !char.IsWhiteSpace(c) && !char.IsControl(c), start, count);
        }

        public static int IndexOfLetterOrDigit(this string? str, int start = 0, int count = int.MaxValue)
        {
            return str.IndexOf(char.IsLetterOrDigit, start, count);
        }

        public static int IndexOfNonLetterOrDigit(this string? str, int start = 0, int count = int.MaxValue)
        {
            return str.IndexOf(c => !char.IsLetterOrDigit(c), start, count);
        }

        public static int LastIndexOfWhitespace(this string? str, int start = int.MaxValue, int count = int.MaxValue)
        {
            return str.LastIndexOf(c => char.IsWhiteSpace(c) || char.IsControl(c), start, count);
        }


        public static int LastIndexOfNonWhitespace(this string? str, int start = int.MaxValue, int count = int.MaxValue)
        {
            return str.LastIndexOf(c => !char.IsWhiteSpace(c) && !char.IsControl(c), start, count);
        }

        public static int LastIndexOfLetterOrDigit(this string? str, int start = int.MaxValue, int count = int.MaxValue)
        {
            return str.LastIndexOf(char.IsLetterOrDigit, start, count);
        }

        public static int LastIndexOfNonLetterOrDigit(this string? str, int start = int.MaxValue, int count = int.MaxValue)
        {
            return str.LastIndexOf(c => !char.IsLetterOrDigit(c), start, count);
        }


        // ==========================================================================
        //
        //   Truncate
        //
        // ==========================================================================
        [return: NotNullIfNotNull("str")]
        public static string? TrimStart(this string? str, Func<char, bool> expression)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int trimPos = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (!expression(str[i]))
                    break;
                trimPos = i;
            }

            if (trimPos == -1)
                return str;

            trimPos++;
            if (trimPos == str.Length)
                return "";

            return str[trimPos..];
        }

        [return: NotNullIfNotNull("str")]
        public static string? TrimEnd(this string? str, Func<char, bool> expression)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int trimPos = -1;
            for (int i = str.Length - 1; i >= 0; i--)
            {
                if (!expression(str[i]))
                    break;
                trimPos = i;
            }

            if (trimPos == -1)
                return str;

            if (trimPos == 0)
                return "";

            return str[..trimPos];
        }

        [return: NotNullIfNotNull("str")]
        public static string? Trim(this string? str, Func<char, bool> expression)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int trimStart = -1;
            for (int i = 0; i < str.Length; i++)
            {
                if (!expression(str[i]))
                    break;
                trimStart = i;
            }

            trimStart++;
            if (trimStart == str.Length)
                return "";

            int trimEnd = -1;
            for (int i = str.Length - 1; i > trimStart; i--)
            {
                if (!expression(str[i]))
                    break;
                trimEnd = i;
            }

            if (trimEnd == -1)
                return str[trimStart..];

            return str[trimStart..trimEnd];
        }

        [return: NotNullIfNotNull("str")]
        public static string? Truncate(this string? str, int maxLength)
        {
            if (str == null || str.Length <= maxLength)
                return str;

            return str[..maxLength].Trim();
        }

        [return: NotNullIfNotNull("str")]
        public static string? TruncateEllipsis(this string? str, int maxLength)
        {
            if (str == null || str.Length <= maxLength)
                return str;

            if (maxLength < 4)
                maxLength = 4;


            return str[..(maxLength - 3)]
                .TrimEnd(c => char.IsWhiteSpace(c) || char.IsControl(c) || c == '.')
                + "...";
        }

        [return: NotNullIfNotNull("str")]
        public static string? TruncatePhrase(this string? str, int maxLength, int maxLengthLastWord = 20)
        {
            if (str == null || str.Length <= maxLength)
                return str;

            maxLength -= 3; // ellipsis

            if (maxLength < 1)
                maxLength = 1;

            var pos = str.LastIndexOfWhitespace(maxLength, maxLengthLastWord);
            if (pos == -1)
            {
                str = str[..maxLength];
            }
            else
            {
                str = str[..pos];
            }

            return str.TrimEnd(c => char.IsWhiteSpace(c) || char.IsControl(c) || c == '.') + "...";
        }

        [return: NotNullIfNotNull("str")]
        public static string? TruncateTrimLink(this string? link, int maxLength)
        {
            if (link == null)
                return null;

            if (string.IsNullOrWhiteSpace(link))
                return "";

            Uri uri;
            try
            {
                if (!link.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                    uri = new Uri("http://" + link);
                else
                    uri = new Uri(link);
            }
            catch (Exception)
            {
                return link.TruncateEllipsis(maxLength);
            }

            var trimHostName = new[] { "www.", "www2.", "mobile.", "m." };
            var hostName = uri.Host;
            foreach (var th in trimHostName)
            {
                if (hostName.StartsWith(th, StringComparison.InvariantCultureIgnoreCase))
                {
                    hostName = hostName[th.Length..];
                    break;
                }
            }

            if (hostName.Length >= maxLength)
                return hostName.TruncateEllipsis(maxLength);

            var sb = new StringBuilder(maxLength);
            sb.Append(hostName);
            if (uri.Port != 80 && uri.Port != 443)
            {
                sb.Append(':');
                sb.Append(uri.Port);
            }

            if (sb.Length > maxLength)
                return sb.ToString().TruncateEllipsis(maxLength);

            string path = uri.PathAndQuery;
            if (path == "/")
                return sb.ToString();

            string? qs = null;
            int pos = path.IndexOf('?');
            if (pos > 0)
            {
                qs = path[(pos + 1)..];
                path = path[..pos];
            }

            if (!_typicalHomepages.Any(th => string.Compare(th, path, true) == 0))
            {
                sb.Append(path);
                if (sb.Length >= maxLength)
                    return sb.ToString().TruncateEllipsis(maxLength);
            }

            int spaceLeft = maxLength - sb.Length;
            if (spaceLeft < 3 || string.IsNullOrWhiteSpace(qs))
                return sb.ToString().TruncateEllipsis(maxLength);

            bool hasQS = false;
            string[] qsParts = qs.Split('&');

            foreach (var qsp in qsParts)
            {
                if (string.IsNullOrWhiteSpace(qsp))
                    continue;

                if (qsp.EndsWith("=") || !qsp.Contains('='))
                    continue;

                if (!hasQS)
                {
                    sb.Append('?');
                    hasQS = true;
                }
                else
                {
                    sb.Append('&');
                }
                sb.Append(qsp);
                if (sb.Length > maxLength)
                    break;
            }


            return sb.ToString().TruncateEllipsis(maxLength);
        }

        [return: NotNullIfNotNull("str")]
        public static string? TrimLower(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return "";

            return str.Trim().ToLowerInvariant();
        }


        // ==========================================================================
        //
        //   Transform
        //
        // ==========================================================================
        /// <summary>
        /// Remove the accents (diacritics) from the text
        /// </summary>
        [return: NotNullIfNotNull("str")]
        public static string? RemoveAccents(this string? str)
        {
            // Originally from http://stackoverflow.com/a/249126/603637
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var normalizedString = str.Normalize(NormalizationForm.FormD);
            StringBuilder? sb = null;

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    if (sb == null)
                        sb = new StringBuilder(str.Length);
                    sb.Append(c);
                }
            }
            if (sb == null || sb.Length == 0)
                return str;

            return sb.ToString().Normalize(NormalizationForm.FormC);

        }

        [return: NotNullIfNotNull("str")]
        private static string? RemapCharacters(string? str, Func<char, string?> map)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            StringBuilder? sb = null;
            for (int i = 0; i < str.Length; i++)
            {
                var ch = str[i];
                var mappedChar = map(ch);
                if (mappedChar == null)
                    continue;

                if (sb == null)
                {
                    if (mappedChar.Length == 1 && mappedChar[0] == ch)
                        continue;

                    // Break the string
                    sb = new StringBuilder(str.Length * 11 / 10); // 10% room
                                                                  // Copy everything up to this point
                    if (i > 0)
                    {
                        sb.Append(str, 0, i);
                    }
                }
                sb.Append(mappedChar);
            }

            if (sb == null)
                return str;

            return sb.ToString();

        }

        [return: NotNullIfNotNull("str")]
        public static string? Transliterate(this string? str)
        {
            return RemapCharacters(str, ch => ch.Transliterate());
        }

        [return: NotNullIfNotNull("str")]
        public static string? GlyphMapAndTransliterate(this string? str)
        {
            return RemapCharacters(str, ch => ch.GlyphMapAndTransliterate());
        }

        [return: NotNullIfNotNull("str")]
        public static string? GlyphMap(this string? str)
        {
            return RemapCharacters(str, ch => ch.GlyphMap());
        }

        [return: NotNullIfNotNull("str")]
        public static string? CapitalizeFirstWord(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            StringBuilder? sb = null;
            int i = str.IndexOfNonWhitespace();
            var c = str[i];
            if (!char.IsLetterOrDigit(c))
                return str;

            if (char.IsUpper(c) || !char.IsLetter(c))
                return str; // first is upper or a digit

            sb = new StringBuilder(str.Length);
            if (i > 0)
                sb.Append(str, 0, i);
            sb.Append(char.ToUpper(c));
            if (str.Length - i > 1)
                sb.Append(str, i + 1, str.Length - i - 1);

            return sb.ToString();
        }

        [return: NotNullIfNotNull("str")]
        public static string? CapitalizeAllWords(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var sb = new StringBuilder(str.Length);
            bool prevSeparator = true;
            foreach (char c in str)
            {
                if (prevSeparator)
                {
                    if (char.IsLower(c))
                    {
                        sb.Append(char.ToUpper(c));
                        prevSeparator = false;
                        continue;
                    }
                }
                sb.Append(c);
                prevSeparator = !char.IsLetterOrDigit(c);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Remove leading and trailing whitespaces, and convert all whitespaces (sequences) into a single space character
        /// </summary>
        [return: NotNullIfNotNull("str")]
        public static string? TrimInBetween(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int len = str.Length;
            StringBuilder? sb = null;

            int start = str.IndexOfNonWhitespace(), i;
            if (start < 0)
                return "";

            if (start > 0)
            {
                sb = new StringBuilder(str.Length - start);
            }
            for (i = start; i < len; i++)
            {
                char c = str[i];
                if (char.IsWhiteSpace(c) || char.IsControl(c))
                {
                    var segmentLength = i - start;
                    if (segmentLength > 0)
                    {
                        if (sb == null)
                            sb = new StringBuilder(str.Length);
                        sb.Append(str, start, segmentLength);
                        sb.Append(' ');
                    }
                    start = i + 1;
                }
            }

            if (sb == null)
            {
                if (start == 0)
                    return str;
                return str[start..];
            }

            if (i > start)
                sb.Append(str, start, i - start);

            if (sb[^1] == ' ')
                sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }


        /// <summary>
        /// Returns the first word of the sentence (by English standards)
        /// </summary>
        public static string? GetFirstWord(this string? str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int wordStart = str.IndexOfLetterOrDigit();
            if (wordStart == -1)
                return null;

            int pos = wordStart;
            do
            {
                int wordEnd = str.IndexOfNonLetterOrDigit(pos);

                if (wordEnd == -1)
                {
                    if (wordStart == 0)
                        return str;
                    return str[wordStart..];
                }

                if (wordEnd <= str.Length - 2 &&
                    (str[wordEnd] == '\'' || str[wordEnd] == 'ʼ') && char.IsLetter(str[wordEnd + 1]))
                {
                    pos = wordEnd + 1;
                    continue;
                }

                return str[wordStart..wordEnd];

            } while (true);

        }

        /// <summary>
        /// Returns the last word in the string
        /// </summary>
        [return: NotNullIfNotNull("str")]
        public static string? GetLastWord(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return str;

            var wordEnd = str.LastIndexOfLetterOrDigit();
            if (wordEnd == -1)
                return string.Empty;

            if (wordEnd == 0)
                return str;

            var pos = wordEnd;
            do
            {
                int wordStart = str.LastIndexOfNonLetterOrDigit(pos);
                if (wordStart == -1)
                    return str[..(wordEnd + 1)];

                if (wordStart >= 1
                    && (str[wordStart] == '\'' || str[wordStart] == 'ʼ') && char.IsLetter(str[wordStart - 1]))
                {
                    pos = wordStart - 1;
                    continue;
                }
                wordStart++;

                return str.Substring(wordStart, wordEnd - wordStart + 1);
            } while (true);
        }

        //public static List<string> TokenizeEnglish(this string? str)
        //{
        //	throw new NotImplementedException();
        //}
        [return: NotNullIfNotNull("text")]
        public static string? MakeUrlSegmentSafe(this string? text, int maxLength = 50)
        {
            if (string.IsNullOrWhiteSpace(text))
                return null;

            text = text.GlyphMapAndTransliterate().RemoveAccents();
            if (text == null)
                return null;

            var sb = new StringBuilder(maxLength);
            bool lastSeparator = true;
            foreach (var c in text)
            {
                if (c <= 32 || char.IsWhiteSpace(c) || char.IsControl(c) || (char.IsSymbol(c) && c != '-'))
                {
                    if (lastSeparator)
                        continue;
                    if (sb.Length == maxLength - 1)
                        break;
                    lastSeparator = true;
                    sb.Append('-');
                    continue;
                }
                if (c.IsASCIILetterOrDigit() || c == '-')
                {
                    sb.Append(c);
                    lastSeparator = c == '-';
                }
                else
                {
                    sb.Append(((int)c).ToHex());
                    lastSeparator = false;
                }
                if (sb.Length >= maxLength)
                    break;
            }
            if (sb.Length > maxLength)
            {
                sb.Remove(maxLength - 1, maxLength - sb.Length);
            }

            while (sb.Length > 0 && sb[^1] == '-')
            {
                sb.Remove(sb.Length - 1, 1);
            }

            if (sb.Length == 0)
                return string.Empty;

            return sb.ToString();
        }


        // ==========================================================================
        //
        //   To Other Types
        //
        // ==========================================================================
        public static bool ToBool(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return false;

            var str2 = str.TrimLower();
            switch (str2)
            {
                case "1":
                case "y":
                case "s":
                case "true":
                case "si":
                case "ja":
                case "oui":
                case "ken":
                case "sea":
                case "jes":
                case "hai":
                case "ndiyo":
                case "gee":
                case "haa'n":
                case "haan":
                case "hanji":
                case "ano":
                case "áno":
                case "igen":
                case "da":
                case "evet":
                case "nai":
                case "ya":
                case "oo":
                case "na'am":
                case "já":
                case "sim":
                case "yes":
                case "yay":
                case "yey":
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Converts a named color or a hex color ('#fcfcde') to a Color structure
        /// </summary>
        public static Color? ToColor(this string? str)
        {
            return ColorExtensions.ToColor(str);
        }

        [return: NotNullIfNotNull("str")]
        public static byte[]? ToBytesFromBase64(this string? str)
        {
            if (str == null)
                return null;

            return Convert.FromBase64String(str);
        }

        public static byte[]? ToBytesFromBase62(this string? str)
        {
            if (str == null)
                return null;
            return ByteArrayExtensions.FromBase62(str);
        }

        public static byte[]? ToBytesFromHex(this string? str)
        {
            if (str == null)
                return null;

            return ByteArrayExtensions.FromHexEncoding(str);
        }

        public static int ToInt(this string? str)
        {
            var l = str.ToLong();
            if (l > int.MaxValue || l < int.MinValue)
                return 0;
            return (int)l;
        }

        public static long ToLong(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var groupSeparator = currentCulture.NumberFormat.NumberGroupSeparator;

            int firstNonWhitespace = str.IndexOfNonWhitespace();
            if (string.Compare(str, firstNonWhitespace, "0x", 0, 2,
                StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                int hexStart = firstNonWhitespace + 2;
                // Parse from hex
                int end = str.IndexOf(c => !char.IsDigit(c)
                                           && !(c >= 'a' && c <= 'f') && !(c >= 'A' && c <= 'F'), hexStart);
                if (end != -1)
                    str = str[hexStart..end];
                else
                    str = str[hexStart..];

                long l2;
                if (!long.TryParse(str, NumberStyles.HexNumber, currentCulture, out l2))
                    return 0;
                return l2;
            }

            var sb = new StringBuilder(str.Length);
            bool hasDigit = false;
            for (int p = firstNonWhitespace; p < str.Length; p++)
            {
                // Skip group separators
                if (string.Compare(str, p, groupSeparator, 0, groupSeparator.Length, true) == 0)
                {
                    if (!hasDigit)
                        return 0; // cannot start with a group separator
                    if (groupSeparator.Length > 1)
                        p += groupSeparator.Length - 1;
                    continue;
                }

                char c = str[p];

                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    sb.Append(c);
                }
                else if (c == '+' || c == '-')
                {
                    sb.Append(c);
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (hasDigit)
                        break;
                }
                else
                {
                    break;
                }
            }

            long l;
            if (!long.TryParse(sb.ToString(), NumberStyles.Number, currentCulture, out l))
                return 0;

            return l;
        }

        public static double ToDouble(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return 0.0;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var numberFormatInfo = currentCulture.NumberFormat;
            var groupSeparator = numberFormatInfo.NumberGroupSeparator;
            var decimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            bool hasPercent = false;

            var sb = new StringBuilder(str.Length);
            bool hasDigit = false;
            for (int p = 0; p < str.Length; p++)
            {
                // Skip group separators
                if (string.Compare(str, p, groupSeparator, 0, groupSeparator.Length, true) == 0)
                {
                    if (!hasDigit)
                        return 0; // cannot start with a group separator
                    p += groupSeparator.Length - 1;
                    continue;
                }
                else if (string.Compare(str, p, decimalSeparator, 0, decimalSeparator.Length, true) == 0)
                {
                    p += decimalSeparator.Length - 1;
                    sb.Append(decimalSeparator);
                    continue;
                }

                char c = str[p];

                if (_currencySymbols.Contains(c))
                    continue;

                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    sb.Append(c);
                }
                else if (c == '+' || c == '-' || c == 'e' || c == 'E')
                {
                    sb.Append(c);
                }
                else if (c == '%')
                {
                    hasPercent = true;
                    break;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (hasDigit)
                        break;
                }
                else
                {
                    break;
                }
            }
            if (sb.Length == 0)
                return 0;

            if (double.TryParse(sb.ToString(),
                NumberStyles.Float | NumberStyles.AllowTrailingSign | NumberStyles.AllowExponent,
                currentCulture, out double ret))
            {
                if (hasPercent)
                    ret /= (double)100;
                return ret;
            }

            return 0;
        }

        public static float ToFloat(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (float)0.0;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var numberFormatInfo = currentCulture.NumberFormat;
            var groupSeparator = numberFormatInfo.NumberGroupSeparator;
            var decimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            bool hasPercent = false;
            StringBuilder sb = new StringBuilder(str.Length);
            bool hasDigit = false;
            for (int p = 0; p < str.Length; p++)
            {
                // Skip group separators
                if (string.Compare(str, p, groupSeparator, 0, groupSeparator.Length, true) == 0)
                {
                    if (!hasDigit)
                        return 0; // cannot start with a group separator
                    p += groupSeparator.Length - 1;
                    continue;
                }
                else if (string.Compare(str, p, decimalSeparator, 0, decimalSeparator.Length, true) == 0)
                {
                    p += decimalSeparator.Length - 1;
                    sb.Append(decimalSeparator);
                    continue;
                }

                char c = str[p];

                if (_currencySymbols.Contains(c))
                    continue;

                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    sb.Append(c);
                }
                else if (c == '+' || c == '-' || c == 'e' || c == 'E')
                {
                    sb.Append(c);
                }
                else if (c == '%')
                {
                    hasPercent = true;
                    break;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (hasDigit)
                        break;
                }
                else
                {
                    break;
                }
            }
            if (sb.Length == 0)
                return 0;

            if (float.TryParse(sb.ToString(),
                NumberStyles.Float | NumberStyles.AllowTrailingSign | NumberStyles.AllowExponent,
                currentCulture, out float ret))
            {
                if (hasPercent)
                    ret /= (float)100;
                return ret;
            }

            return 0;
        }

        public static decimal ToDecimal(this string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return (decimal)0;

            var currentCulture = Thread.CurrentThread.CurrentCulture;
            var numberFormatInfo = currentCulture.NumberFormat;
            var groupSeparator = numberFormatInfo.NumberGroupSeparator;
            var decimalSeparator = numberFormatInfo.NumberDecimalSeparator;

            bool hasPercent = false;

            var sb = new StringBuilder(str.Length);
            bool hasDigit = false;
            for (int p = 0; p < str.Length; p++)
            {
                // Skip group separators
                if (string.Compare(str, p, groupSeparator, 0, groupSeparator.Length, true) == 0)
                {
                    if (!hasDigit)
                        return 0; // cannot start with a group separator
                    p += groupSeparator.Length - 1;
                    continue;
                }
                else if (string.Compare(str, p, decimalSeparator, 0, decimalSeparator.Length, true) == 0)
                {
                    p += decimalSeparator.Length - 1;
                    sb.Append(decimalSeparator);
                    continue;
                }

                char c = str[p];

                if (_currencySymbols.IndexOf(c) >= 0)
                    continue;

                if (char.IsDigit(c))
                {
                    hasDigit = true;
                    sb.Append(c);
                }
                else if (c == '+' || c == '-' || c == 'e' || c == 'E')
                {
                    sb.Append(c);
                }
                else if (c == '%')
                {
                    hasPercent = true;
                    break;
                }
                else if (char.IsWhiteSpace(c))
                {
                    if (hasDigit)
                        break;
                }
                else
                {
                    break;
                }
            }
            if (sb.Length == 0)
                return 0;

            decimal ret;

            if (decimal.TryParse(sb.ToString(),
                NumberStyles.Float | NumberStyles.AllowTrailingSign | NumberStyles.AllowExponent,
                currentCulture, out ret))
            {
                if (hasPercent)
                    ret /= (decimal)100;
                return ret;
            }

            return 0;
        }

        public static T? ToEnum<T>(this string? value, T? defaultValue = default) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value))
                return defaultValue;

            if (Enum.TryParse<T>(value, true, out var result))
                return result;

            return defaultValue;
        }

        [return: NotNullIfNotNull("str")]
        public static List<string?>? ToListFromCsvLine(this string? str)
        {
            return ToListFromDelimiterLine(str, ',', '\0',
                s => UnescapeCSVField(s != null ? s.Trim() : ""));

        }


        [return: NotNullIfNotNull("str")]
        public static List<string?>? ToListFromTabDelimitedLine(this string? str, bool trimWhitespaces = false)
        {
            //if (string.IsNullOrWhiteSpace(str))
            //	return null;

            //return str.Split(new []{'\t'}).ToList();

            return ToListFromDelimiterLine(str, '\t', 't', s => UnescapeTabField(s, trimWhitespaces));
        }


        [return: NotNullIfNotNull("str")]
        private static List<string?>? ToListFromDelimiterLine(string? str, char delimiter,
            char unescapedSlashDelim, Func<string?, string?> unescapeFunc)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            var row = new List<string?>();
            var sb = new StringBuilder();
            bool hasField = false;
            bool inQuote = false;
            for (int i = 0; i < str.Length; i++)
            {
                char c = str[i];
                char c1 = i < (str.Length - 1) ? str[i + 1] : '\0'; // look ahead

                if (inQuote)
                {
                    if (c == '\\' && c1 == '\"')
                    {
                        sb.Append("\\\"");
                        i++; // skip an extra character to preserve the escaped quote
                    }
                    else if (c == '\"')
                    {
                        // end quote
                        inQuote = false;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
                else
                {
                    if (c == delimiter)
                    {
                        var field = unescapeFunc(sb.ToString());
                        row.Add(field);
                        sb.Clear();
                        hasField = true;
                    }
                    else if (c == '\\')
                    {
                        if (c1 == '\"')
                        {
                            sb.Append("\\\"");
                            i++;
                        }
                        else if (c1 == unescapedSlashDelim)
                        {
                            sb.Append(delimiter);
                            i++;
                        }
                    }
                    else if (c == '\"')
                    {
                        inQuote = true;
                    }
                    //else if (char.IsWhiteSpace(c) || char.IsControl(c))
                    //{
                    //	if (sb.Length != 0)
                    //		sb.Append(c);
                    //}
                    else
                    {
                        sb.Append(c);
                    }
                }
            }

            if (hasField || sb.Length > 0)
            {
                var unescaped = unescapeFunc(sb.ToString());
                row.Add(unescaped);
            }

            return row;
        }



        [return: NotNullIfNotNull("str")]
        private static string? EscapeCharacters(string? str, string charsToEscape, string[] escapedChars, bool xEncodeSub32)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            int len = str.Length;
            StringBuilder? sb = null;

            int i = 0, start = 0;
            while (i < len)
            {
                string? cstr = null;
                char c = str[i];
                int pos = charsToEscape.IndexOf(c);
                if (pos >= 0)
                {
                    cstr = escapedChars[pos];
                }
                else if (c < 32 && xEncodeSub32)
                {
                    // C# / C++ / JavaScript
                    cstr = "\\x" + ((byte)c).ToHex();
                }
                else
                {
                    i++;
                    continue;
                }

                if (sb == null)
                    sb = new StringBuilder((21 * len) / 20); // 5%
                if (i > start)
                {
                    sb.Append(str, start, i - start);
                }

                sb.Append(cstr);
                i++;
                start = i;
            }
            if (sb == null)
                return str;

            if (i > start)
                sb.Append(str, start, i - start);

            return sb.ToString();
        }


    }
}
