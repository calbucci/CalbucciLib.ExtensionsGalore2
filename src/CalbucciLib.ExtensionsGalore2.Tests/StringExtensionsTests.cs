﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CalbucciLib.ExtensionsGalore2.Tests
{
    [TestClass()]
    public class StringExtensionsTests
    {
        private const string?[]? Item2 = (string?[])null;

        [TestMethod()]
        public void EscapeCStringTest()
        {
            string?[] testStrings = new[]
            {
                null, null,
                "", "",
                " ", " ",
                "\t", "\\t",
                " \t\r\n ", " \\t\\r\\n ",
                "abc", "abc",
                "\rabc", "\\rabc",
                "abc\r", "abc\\r",
                "\a\b\t\n\v\f\r'\"\\", "\\a\\b\\t\\n\\v\\f\\r'\\\"\\\\",
            };

            for (int i = 0; i < testStrings.Length; i += 2)
            {
                var original = testStrings[i];
                var expected = testStrings[i + 1];
                var actual = original.EscapeCString();

                Assert.AreEqual(expected, actual, "Failed for expected: " + expected);
            }

        }

        [TestMethod()]
        public void TrimInBetweenTest()
        {
            string?[] testStrings = new[]
            {
                null, null,
                "", "",
                " ", "",
                "a", "a",
                "a ", "a",
                " a", "a",
                "a b", "a b",
                " a  b", "a b",
                "a  b", "a b",
                "a\nb", "a b",
                "a\r\nb", "a b",
                "\t\r a \t\r b \t\n", "a b",
                "a\t\r", "a",
                "\t\ra", "a",
                "\r\n a \t\r\n b\r\n\t", "a b",
            };

            for (int i = 0; i < testStrings.Length; i += 2)
            {
                var original = testStrings[i];
                var expected = testStrings[i + 1];
                var actual = original.TrimInBetween();

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void ToListFromCsvLineTest()
        {
            string?[][] tests = new[]
            {
                new[] {"\\\",b", "\"", "b"},
                new string?[] {null, null},
                new[] {"", null},
                new[] {",", "", ""},
                new[] {",,", "", "", ""},
                new[] {" ,  ,\t ", "", "", ""},

                new[] {"a", "a"},
                new[] {"a,b", "a", "b"},
                new[] {" a , b ", "a", "b"},
                new[] {"\ta,\tb", "a", "b"},
                new[] {"a,", "a", ""},
                new[] {",a", "", "a"},
                new[] {"\"a,b\",c", "a,b", "c"},

                new[] {"\"ab\",c", "ab", "c"},
                new[] {"a\\\"b,c", "a\"b", "c"},
            };

            foreach (var test in tests)
            {
                var row = test[0].ToListFromCsvLine();

                if (row == null)
                {
                    Assert.IsNull(test[1], "test: " + test[0]);
                }
                else
                {
                    Assert.AreEqual(row.Count, test.Length - 1, "test: " + test[0]);
                    for (int i = 0; i < row.Count; i++)
                    {
                        Assert.AreEqual(row[i], test[i + 1], "test: " + test[0] + " - Item " + i);
                    }
                }
            }


        }

        [TestMethod()]
        public void UnescapeCSVTest()
        {
            string?[] tests = new string?[]
            {
                "a", "a",
                null, null,
                "", "",
                " a ", "a",
                "\"a\"", "a",
                "  \"a\" ", "a",
                "\"\\n\"", "\n",
                "\" a \"", " a ",

                "\"a,b\\\",c", "a,b\",c", // malformed
			};

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = tests[i].UnescapeCSVField();

                Assert.AreEqual(expected, actual, "Test: " + test);
            }
        }

        [TestMethod()]
        public void TransliterateTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "abc", "abc",
                "São Paulo é ótima", "Sao Paulo e otima",
                "שָׁלוֹם", "shalvom",
				//"Ελλάς", "Greek",
			};

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];

                var actual = test.Transliterate();

                Assert.AreEqual(expected, actual, "String: " + test);
            }
        }

        [TestMethod()]
        public void GenerateLoremIpsumTest()
        {
            Assert.AreEqual(StringExtensions.GenerateLoremIpsum(0), "");
            Assert.AreEqual(StringExtensions.GenerateLoremIpsum(1), "Lorem");
            Assert.AreEqual(StringExtensions.GenerateLoremIpsum(5), "Lorem ipsum dolor sit amet");
            Assert.AreEqual(StringExtensions.GenerateLoremIpsum(6), "Lorem ipsum dolor sit amet, consectetur");
        }

        [TestMethod()]
        public void CreateTRTDTest()
        {
            Assert.AreEqual(StringExtensions.CreateTRTD(null), "");
            var empty = new string?[0];
            Assert.AreEqual(StringExtensions.CreateTRTD(empty), "");
            Assert.AreEqual(StringExtensions.CreateTRTD(""), "<tr><td></td></tr>");
            Assert.AreEqual(StringExtensions.CreateTRTD("abc"), "<tr><td>abc</td></tr>");
            Assert.AreEqual(StringExtensions.CreateTRTD("abc", ""), "<tr><td>abc</td><td></td></tr>");
            Assert.AreEqual(StringExtensions.CreateTRTD("a", "b", "c"), "<tr><td>a</td><td>b</td><td>c</td></tr>");

        }

        [TestMethod()]
        public void UnescapeCStringTest()
        {
            string?[] tests = new[]
            {
                null,
                "",
                "abc",
                "\t\r\n",
                "\\t\\r\\n",
                "\\\t"
            };

            foreach (var test in tests)
            {
                var expected = test;
                var escaped = expected.EscapeCString();
                var actual = escaped.UnescapeCString();

                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void EscapeJsonTest()
        {
            string?[] tests = new[]
            {
                null, "null",
                "", "''",
                "abc", "'abc'",
                "a\"bc", "'a\"bc'",
                "a\'bc", "'a\\'bc'",
                "a\r\nb\tc", "'a\\r\\nb\\tc'"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].EscapeJson('\'');
                Assert.AreEqual(expected, actual);

            }
        }

        [TestMethod()]
        public void EscapeCSVTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                " ", " ",
                "abc", "abc",
                " abc ", " abc ",
                "abc,def", "\"abc,def\"",
                "abc\\def", "\"abc\\\\def\"",
                "Mark \"One\"", "\"Mark \\\"One\\\"\""
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = test.EscapeCSV();

                Assert.AreEqual(expected, actual, test);
            }

            Assert.AreEqual("\"\\r\\n\"", "\r\n".EscapeCSV(true));

            string?[] tests2 = new[]
            {
                null,
                "",
                "abc",
                "abc,def",
                "abc\"def",
                "abc\'def"
            };

            foreach (var test in tests2)
            {
                var expected = test;
                var escaped = test.EscapeCSV();
                var actual = escaped.UnescapeCSVField();
                Assert.AreEqual(expected, actual);
            }

            Assert.AreEqual(" abc ", " abc ".UnescapeCSVField(false));
            Assert.AreEqual(" abc ", " \" abc ".UnescapeCSVField(false));

        }

        [TestMethod()]
        public void ContainsAnyTest()
        {
            string?[][] tests = new[]
            {
                new[] {"Marcelo", "mar", "123", "rc1"},
                new[] {"Marcelo", "abc", "123", "elo"},
                new[] {"Marcelo", "abc", "rc", "7"},
                new[] {"Marcelo", "marcELO", "racelo", "7"},
                new[] {"Marcelo", "marcELO", "23", "7"},
                new[] {"Marcelo", "marcELO", "", null},
            };

            foreach (var testSet in tests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsTrue(str.ContainsAny(matches));
            }


            string?[][] negativeTests = new[]
            {
                new[] {"Marcelo", "mr", "123", "rc1"},
                new[] {"Marcelo", "abc", "123", "eo"},
                new[] {"Marcelo", "abc", "rEc", "7"},
                new[] {"Marcelo", "amarc", "", null},
            };

            foreach (var testSet in negativeTests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsFalse(str.ContainsAny(matches));
            }

            var charTests = new[]
            {
                "a", "a",
                "abcd", "abcd",
                "abcd", "defg",
                "abcd", "efgc"
            };

            for (int i = 0; i < charTests.Length; i += 2)
            {
                var chars = charTests[i + 1].ToCharArray();
                Assert.IsTrue(charTests[i].ContainsAny(chars));
            }


        }

        [TestMethod()]
        public void ContainsAny2Test()
        {
            Assert.IsTrue("Marcelo".ContainsAny("abc", "ELO"));
        }

        [TestMethod()]
        public void StartsWithAnyTest()
        {
            string?[][] tests = new[]
            {
                new[] {"Marcelo", "mar", "123", "rc1"},
                new[] {"Marcelo", "m", "123", "eo"},
                new[] {"Marcelo", "abc", "marcelo", "7"},
                new[] {"Marcelo", "cELO", "racelo", "MARCELO"},
            };

            foreach (var testSet in tests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsTrue(str.StartsWithAny(matches));
            }


            string?[][] negativeTests = new[]
            {
                new[] {"Marcelo", "mr", "123", "arcelo"},
                new[] {"Marcelo", "abc", "123", "lo"},
                new[] {"Marcelo", "abc", "rEc", "7"},
                new[] {"Marcelo", "amarc", "", null},
            };

            foreach (var testSet in negativeTests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsFalse(str.StartsWithAny(matches));
            }
        }

        [TestMethod]
        public void StartsWithAny2Test()
        {
            Assert.IsTrue("Marcelo".StartsWithAny("abc", "Mar"));
        }

        [TestMethod]
        public void EqualsAnyTest()
        {
            string?[][] tests = new[]
            {
                new[] {"Marcelo", "marcelo", "123", "rc1"},
                new[] {"Marcelo", "m", "MARCELO", "eo"},
                new[] {"Marcelo", "abc", "marcelo", "7"},
                new[] {"Marcelo", "cELO", "racelo", "MARCELO"},
            };

            foreach (var testSet in tests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsTrue(str.EqualsAny(matches));
            }


            string?[][] negativeTests = new[]
            {
                new[] {"Marcelo", "mr", "123", "arcelo"},
                new[] {"Marcelo", "abc", "123", "lo"},
                new[] {"Marcelo", "abc", "rEc", "7"},
                new[] {"Marcelo", "amarc", "", null},
            };

            foreach (var testSet in negativeTests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsFalse(str.EqualsAny(matches));
            }

        }

        [TestMethod()]
        public void EndsWithAnyTest()
        {
            string?[][] tests = new[]
            {
                new[] {"Marcelo", "mr", "123", "lo"},
                new[] {"Marcelo", "n", "elo", "eo"},
                new[] {"Marcelo", "rcelo", "marlo", "7"},
                new[] {"Marcelo", "cELO", "racelo", "MARCELO"},
            };

            foreach (var testSet in tests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsTrue(str.EndsWithAny(matches));
            }


            string?[][] negativeTests = new[]
            {
                new[] {"Marcelo", "mr", "123", "marcel"},
                new[] {"Marcelo", "abc", "123", "arce"},
                new[] {"Marcelo", "abc", "rEc", "7"},
                new[] {"Marcelo", "amarc", "", null},
            };

            foreach (var testSet in negativeTests)
            {
                var str = testSet[0];
                var matches = testSet.Skip(1).ToList();

                Assert.IsFalse(str.EndsWithAny(matches));
            }
        }

        [TestMethod()]
        public void CompareNonWhitespaceTest()
        {
            string?[] tests = new string?[]
            {
                null, null,
                "", "",
                "  ", "  ",
                "\t\r\n", "",
                "a", "a",
                " a", "a",
                "a ", " a ",
                "\ta\nb\rc", "abc",
                " A\t b \r C ", "\nab c",
                "abc", "abc",
                "a bc", "abc",
                "ab c", "abc",
                "abc", "a bc",
                "abc", "ab c"
            };

            string?[] negativeTests = new string?[]
            {
                "", "a",
                "A", "á",
                "a\ra", "a\rã",
                "abcd", "abc",
                "abc", "abcd"

            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var var1 = tests[i];
                var var2 = tests[i + 1];
                Assert.IsTrue(var1.CompareNonWhitespace(var2), $"[{var1}]:[{var2}]");
            }

            for (int i = 0; i < negativeTests.Length; i += 2)
            {
                var var1 = negativeTests[i];
                var var2 = negativeTests[i + 1];
                Assert.IsFalse(var1.CompareNonWhitespace(var2), $"[{var1}]:[{var2}]");

            }
        }

        [TestMethod()]
        public void LastIndexOfTest()
        {
            var tests = new List<Tuple<string?, Func<char, bool>, int>>
            {
                Tuple.Create<string?, Func<char, bool>, int>("efgb", c => "abc".Contains(c), 3),
                Tuple.Create<string?, Func<char, bool>, int>(null, c => "abc".Contains(c), -1),
                Tuple.Create<string?, Func<char, bool>, int>("", c => "abc".Contains(c), -1),
                Tuple.Create<string?, Func<char, bool>, int>("AAA", c => "abc".Contains(c), -1),
                Tuple.Create<string?, Func<char, bool>, int>("aaa", c => "abc".Contains(c), 2),
                Tuple.Create<string?, Func<char, bool>, int>("cde", c => "abc".Contains(c), 0),
                Tuple.Create<string?, Func<char, bool>, int>("cda", c => "abc".Contains(c), 2),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var func = test.Item2;
                var expected = test.Item3;
                var actual = str.LastIndexOf(func);

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void TruncatePhraseTest()
        {
            var tests = new List<Tuple<string?, int, string?>>
            {
                Tuple.Create<string?, int, string?>(null, 0, null),
                Tuple.Create<string?, int, string?>(null, 10, null),
                Tuple.Create<string?, int, string?>("", 10, ""),
                Tuple.Create<string?, int, string?>("a", 1, "a"),
                Tuple.Create<string?, int, string?>("a", 10, "a"),

                Tuple.Create<string?, int, string?>("This is a test", 1, "T..."),
                Tuple.Create<string?, int, string?>("This is a test", 7, "This..."),
                Tuple.Create<string?, int, string?>("This is a test", 8, "This..."),
                Tuple.Create<string?, int, string?>("This is a test", 10, "This is..."),
                Tuple.Create<string?, int, string?>("This is a test", 14, "This is a test"),

                Tuple.Create<string?, int, string?>("This is a test ThisLastWordIsTooBigToBeTruncatedNeatlyAndWeJustBreakIt", 40, "This is a test ThisLastWordIsTooBigTo..."),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var maxLength = test.Item2;
                var expected = test.Item3;
                var actual = test.Item1.TruncatePhrase(maxLength);

                Assert.AreEqual(expected, actual, "Test: " + str + "/" + maxLength);
            }

        }

        [TestMethod()]
        public void TruncateEllipsisTest()
        {
            var tests = new List<Tuple<string?, int, string?>>
            {
                Tuple.Create<string?, int, string?>(null, 0, null),
                Tuple.Create<string?, int, string?>(null, 10, null),
                Tuple.Create<string?, int, string?>("", 10, ""),
                Tuple.Create<string?, int, string?>("a", 1, "a"),
                Tuple.Create<string?, int, string?>("a", 10, "a"),

                Tuple.Create<string?, int, string?>("This is a test", 1, "T..."),
                Tuple.Create<string?, int, string?>("This is a test", 4, "T..."),
                Tuple.Create<string?, int, string?>("This is a test", 5, "Th..."),
                Tuple.Create<string?, int, string?>("This is a test", 8, "This..."),
                Tuple.Create<string?, int, string?>("This is a test", 15, "This is a test"),

            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var maxLength = test.Item2;
                var expected = test.Item3;
                var actual = test.Item1.TruncateEllipsis(maxLength);

                Assert.AreEqual(expected, actual, "Test: " + str + "/" + maxLength);
            }

        }

        [TestMethod()]
        public void TruncateTrimLinkTest()
        {
            var tests = new List<Tuple<string?, int, string?>>
            {

                Tuple.Create<string?, int, string?>(null, 0, null),
                Tuple.Create<string?, int, string?>(null, 10, null),
                Tuple.Create<string?, int, string?>("", 10, ""),
                Tuple.Create<string?, int, string?>("a", 1, "a"),
                Tuple.Create<string?, int, string?>("abcd", 1, "a..."),

                Tuple.Create<string?, int, string?>("google.com", 1, "g..."),

                Tuple.Create<string?, int, string?>("google.com/", 10, "google.com"),
                Tuple.Create<string?, int, string?>("www.google.com", 10, "google.com"),
                Tuple.Create<string?, int, string?>("www.google.com/search", 10, "google.com"),

                Tuple.Create<string?, int, string?>("www.google.com/search", 17, "google.com/search"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc", 17, "google.com/search"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc", 26, "google.com/search?hello..."),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc", 27, "google.com/search?hello=abc"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc", 40, "google.com/search?hello=abc"),

                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc&", 40, "google.com/search?hello=abc"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc&&", 40, "google.com/search?hello=abc"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc&&delta", 40, "google.com/search?hello=abc"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc&&delta=", 40, "google.com/search?hello=abc"),
                Tuple.Create<string?, int, string?>("www.google.com/search?hello=abc&&delta=def", 40, "google.com/search?hello=abc&delta=def"),

                Tuple.Create<string?, int, string?>("https://www.google.com/search?hello=abc#32", 27, "google.com/search?hello=abc"),

            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var maxLength = test.Item2;
                var expected = test.Item3;
                var actual = str.TruncateTrimLink(maxLength);

                Assert.AreEqual(expected, actual, "Test: " + str + " /" + maxLength);
            }
        }

        [TestMethod()]
        public void RemoveAccentsTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "  \t", "  \t",
                "Bob", "Bob",
                "Açucar", "Acucar",
                "Sãüñôr", "Saunor",
                "ööü", "oou"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = test.RemoveAccents();

                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void CapitalizeFirstWordTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "a", "A",
                "A", "A",
                "álô", "Álô",
                "this is a test", "This is a test",
                "this\ris\na\ttest", "This\ris\na\ttest",
                "this,is.a-test", "This,is.a-test",
                "ALL CAPS", "ALL CAPS",
                "123hello", "123hello"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = tests[i].CapitalizeFirstWord();

                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void CapitalizeAllWordsTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "a", "A",
                "A", "A",
                "álô", "Álô",
                "this is a test", "This Is A Test",
                "this\ris\na\ttest", "This\rIs\nA\tTest",
                "this,is.a-test", "This,Is.A-Test",
                "ALL CAPS", "ALL CAPS",
                "123hello", "123hello"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = tests[i].CapitalizeAllWords();

                Assert.AreEqual(expected, actual);
            }
        }


        [TestMethod()]
        public void GetFirstWordTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "abc", "abc",
                "abc def", "abc",
                "abc,def", "abc",
                "abc123-def", "abc123",
                "This-is", "This",
                "Don't", "Don't",
                "Don't run", "Don't",
                "Donʼt run", "Donʼt",
                "  Don't run", "Don't",
                "--  Don't run", "Don't",
                "'car", "car",
                "car'", "car",
                "'can we'", "can",
                " 'can we' ", "can"

            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = tests[i].GetFirstWord();

                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void GetLastWordTest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "abc", "abc",
                "abc def", "def",
                "abc,def", "def",
                "abc123-def123", "def123",
                "This-is", "is",
                "Don't", "Don't",
                "Don't run", "run",
                "run donʼt", "donʼt",
                " run  Don't ", "Don't",
                "  run don't --", "don't",
                "'car", "car",
                "car'", "car",
                "'can we'", "we",
                " 'can we' ", "we"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var test = tests[i];
                var expected = tests[i + 1];
                var actual = tests[i].GetLastWord();

                Assert.AreEqual(expected, actual, test);
            }
        }

        [TestMethod()]
        public void ToBoolTest()
        {
            var tests = new List<Tuple<string?, bool>>
            {
                Tuple.Create<string?, bool>(null, false),
                Tuple.Create<string?, bool>("", false),
                Tuple.Create<string?, bool>("banana", false),
                Tuple.Create<string?, bool>("false", false),

                Tuple.Create<string?, bool>("1", true),
                Tuple.Create<string?, bool>("yes", true),
                Tuple.Create<string?, bool>("true", true),
                Tuple.Create<string?, bool>("TRue", true),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = str.ToBool();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void ToLongTest()
        {
            var tests = new List<Tuple<string?, long>>
            {

                Tuple.Create<string?,long>(null, 0),
                Tuple.Create<string?,long>("", 0),
                Tuple.Create<string?,long>("a123", 0),
                Tuple.Create<string?,long>(",123", 0),
                Tuple.Create<string?,long>("x", 0),
                Tuple.Create<string?,long>("abc", 0),

                Tuple.Create<string?,long>("127", 127),
                Tuple.Create<string?,long>("  127", 127),
                Tuple.Create<string?,long>("  127  ", 127),
                Tuple.Create<string?,long>(" 127abc", 127),
                Tuple.Create<string?,long>(" 127 123", 127),
                Tuple.Create<string?,long>("127,", 127),

                Tuple.Create<string?,long>("0X7f", 127),
                Tuple.Create<string?,long>(" 0x7F ", 127),
                Tuple.Create<string?,long>(" 0x7Fz ", 127),
                Tuple.Create<string?,long>("0x7F", 127),


                Tuple.Create<string?,long>("127,123", 127123),
                Tuple.Create<string?,long>("987654321987654321", 987654321987654321),

                Tuple.Create<string?,long>("-127", -127),
                Tuple.Create<string?,long>(" -127", -127),
                Tuple.Create<string?,long>(" - 127", -127),
                Tuple.Create<string?,long>(" -127 ", -127),
                Tuple.Create<string?,long>("127-", -127),
                Tuple.Create<string?,long>("- 127.23", -127),

                Tuple.Create<string?,long>(long.MaxValue.ToString(), long.MaxValue),
                Tuple.Create<string?,long>(long.MinValue.ToString(), long.MinValue),
                Tuple.Create<string?,long>("0x" + long.MaxValue.ToString("x"), long.MaxValue),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = test.Item1.ToLong();

                Assert.AreEqual(expected, actual, str);
            }

        }

        [TestMethod()]
        public void ToDoubleTest()
        {
            var tests = new List<Tuple<string?, double>>
            {
                Tuple.Create<string?,double>(null, 0),
                Tuple.Create<string?,double>("", 0),
                Tuple.Create<string?,double>("a123", 0),
                Tuple.Create<string?,double>(",123", 0),
                Tuple.Create<string?,double>("x", 0),
                Tuple.Create<string?,double>("abc", 0),

                Tuple.Create<string?,double>("127", 127),
                Tuple.Create<string?,double>("  127", 127),
                Tuple.Create<string?,double>("  127  ", 127),
                Tuple.Create<string?,double>(" 127abc", 127),
                Tuple.Create<string?,double>(" 127 123", 127),
                Tuple.Create<string?,double>("127,", 127),
                Tuple.Create<string?,double>("127.00", 127),

                Tuple.Create<string?,double>("127%", 1.27),
                Tuple.Create<string?,double>("-127%", -1.27),

                Tuple.Create<string?,double>("127,123", 127123),
                Tuple.Create<string?,double>("987654321", 987654321),

                Tuple.Create<string?,double>("-127", -127),
                Tuple.Create<string?,double>(" -127", -127),
                Tuple.Create<string?,double>(" - 127", -127),
                Tuple.Create<string?,double>(" -127 ", -127),
                Tuple.Create<string?,double>("127-", -127),

                Tuple.Create<string?,double>("127.123", 127.123),
                Tuple.Create<string?,double>("- 127.123", -127.123),
                Tuple.Create<string?,double>("$-127.123", -127.123),
                Tuple.Create<string?,double>("€ - 127.123", -127.123),
                Tuple.Create<string?,double>("987654321.987654321", 987654321.987654321),

                Tuple.Create<string?,double>("1.23e+1", 12.3),
                Tuple.Create<string?,double>("-1.23e-1", -0.123),
                Tuple.Create<string?,double>(" - 1.23e-1", -0.123),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = test.Item1.ToDouble();

                Assert.AreEqual(expected, actual, str);
            }

        }

        [TestMethod()]
        public void ToFloatTest()
        {
            var tests = new List<Tuple<string?, float>>
            {
                Tuple.Create<string?,float>(null, 0),
                Tuple.Create<string?,float>("", 0),
                Tuple.Create<string?,float>("a123", 0),
                Tuple.Create<string?,float>(",123", 0),
                Tuple.Create<string?,float>("x", 0),
                Tuple.Create<string?,float>("abc", 0),

                Tuple.Create<string?,float>("127", 127),
                Tuple.Create<string?,float>("  127", 127),
                Tuple.Create<string?,float>("  127  ", 127),
                Tuple.Create<string?,float>(" 127abc", 127),
                Tuple.Create<string?,float>(" 127 123", 127),
                Tuple.Create<string?,float>("127,", 127),
                Tuple.Create<string?,float>("127.00", 127),

                Tuple.Create<string?,float>("127%", (float)1.27),
                Tuple.Create<string?,float>("-127%", (float)-1.27),

                Tuple.Create<string?,float>("127,123", 127123),
                Tuple.Create<string?,float>("1234567", 1234567),

                Tuple.Create<string?,float>("-127", -127),
                Tuple.Create<string?,float>(" -127", -127),
                Tuple.Create<string?,float>(" - 127", -127),
                Tuple.Create<string?,float>(" -127 ", -127),
                Tuple.Create<string?,float>("127-", -127),

                Tuple.Create<string?,float>("127.1", (float)127.1),
                Tuple.Create<string?,float>("- 127.123", (float)-127.123),
                Tuple.Create<string?,float>("$-127.123", (float)-127.123),
                Tuple.Create<string?,float>("€ - 127.123", (float)-127.123),
                Tuple.Create<string?,float>("0.1234567", (float)0.1234567),

                Tuple.Create<string?,float>("1.23e+1", (float)12.3),
                Tuple.Create<string?,float>("-1.23e-1", (float)-0.123),
                Tuple.Create<string?,float>(" - 1.23e-1", (float)-0.123),
            };

            var floatPrecision = 0.00001;
            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = test.Item1.ToDouble();

                var delta = Math.Abs(expected - actual);

                Assert.IsTrue(delta <= floatPrecision, str);
            }
        }

        [TestMethod()]
        public void ToDecimalTest()
        {
            var tests = new List<Tuple<string?, decimal>>
            {
                Tuple.Create<string?,decimal>(null, 0),
                Tuple.Create<string?,decimal>("", 0),
                Tuple.Create<string?,decimal>("a123", 0),
                Tuple.Create<string?,decimal>(",123", 0),
                Tuple.Create<string?,decimal>("x", 0),
                Tuple.Create<string?,decimal>("abc", 0),

                Tuple.Create<string?,decimal>("127", 127),
                Tuple.Create<string?,decimal>("  127", 127),
                Tuple.Create<string?,decimal>("  127  ", 127),
                Tuple.Create<string?,decimal>(" 127abc", 127),
                Tuple.Create<string?,decimal>(" 127 123", 127),
                Tuple.Create<string?,decimal>("127,", 127),
                Tuple.Create<string?,decimal>("127.00", 127),

                Tuple.Create<string?,decimal>("127%", (decimal)1.27),
                Tuple.Create<string?,decimal>("-127%", (decimal)-1.27),


                Tuple.Create<string?,decimal>("127,123", 127123),
                Tuple.Create<string?,decimal>("987654321987654321", 987654321987654321),

                Tuple.Create<string?,decimal>("-127", -127),
                Tuple.Create<string?,decimal>(" -127", -127),
                Tuple.Create<string?,decimal>(" - 127", -127),
                Tuple.Create<string?,decimal>(" -127 ", -127),
                Tuple.Create<string?,decimal>("127-", -127),

                Tuple.Create<string?,decimal>("127.123", (decimal)127.123),
                Tuple.Create<string?,decimal>("- 127.123", (decimal)-127.123),
                Tuple.Create<string?,decimal>("$-127.123", (decimal)-127.123),
                Tuple.Create<string?,decimal>("€ - 127.123", (decimal)-127.123),
                Tuple.Create<string?,decimal>("987654321.987654", (decimal)987654321.987654),

                Tuple.Create<string?,decimal>("1.23e+1", (decimal)12.3),
                Tuple.Create<string?,decimal>("-1.23e-1", (decimal)-0.123),
                Tuple.Create<string?,decimal>(" - 1.23e-1", (decimal)-0.123),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = test.Item1.ToDecimal();

                Assert.AreEqual(expected, actual, str);
            }
        }

        [TestMethod()]
        public void TrimStartTest()
        {
            var tests = new List<Tuple<string?, Func<char, bool>, string?>>
            {
                Tuple.Create<string?, Func<char, bool>, string?>((string?)"a b c", char.IsLetterOrDigit, (string?)" b c"),
                Tuple.Create<string?, Func<char, bool>, string?>((string?)null, char.IsLetterOrDigit, (string?)null),
                Tuple.Create<string?, Func<char, bool>, string?>((string?)"", char.IsLetterOrDigit, (string?)""),
                Tuple.Create<string?, Func<char, bool>, string?>((string?)" \t", char.IsLetterOrDigit, (string?)" \t"),
                Tuple.Create<string?, Func<char, bool>, string?>((string?)"abc", char.IsLetterOrDigit, (string?)""),
                Tuple.Create<string?, Func<char, bool>, string?>((string?)" a b c ", char.IsLetterOrDigit, (string?)" a b c "),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expression = test.Item2;
                var expected = test.Item3;
                var actual = str.TrimStart(expression);

                Assert.AreEqual(expected, actual, str);
            }
        }

        [TestMethod()]
        public void TrimEndTest()
        {
            var tests = new List<Tuple<string?, Func<char, bool>, string?>>
            {
                Tuple.Create<string?, Func<char, bool>, string?>(null, char.IsLetterOrDigit, null),
                Tuple.Create<string?, Func<char, bool>, string?>("", char.IsLetterOrDigit, ""),
                Tuple.Create<string?, Func<char, bool>, string?>(" \t", char.IsLetterOrDigit, " \t"),
                Tuple.Create<string?, Func<char, bool>, string?>("abc", char.IsLetterOrDigit, ""),
                Tuple.Create<string?, Func<char, bool>, string?>("a b c", char.IsLetterOrDigit, "a b "),
                Tuple.Create<string?, Func<char, bool>, string?>(" a b c ", char.IsLetterOrDigit, " a b c "),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expression = test.Item2;
                var expected = test.Item3;
                var actual = str.TrimEnd(expression);

                Assert.AreEqual(expected, actual, str);
            }
        }

        [TestMethod()]
        public void TrimTest()
        {
            var tests = new List<Tuple<string?, Func<char, bool>, string?>>
            {
                Tuple.Create<string?, Func<char, bool>, string?>(null, char.IsLetterOrDigit, null),
                Tuple.Create<string?, Func<char, bool>, string?>("", char.IsLetterOrDigit, ""),
                Tuple.Create<string?, Func<char, bool>, string?>(" \t", char.IsLetterOrDigit, " \t"),
                Tuple.Create<string?, Func<char, bool>, string?>("abc", char.IsLetterOrDigit, ""),
                Tuple.Create<string?, Func<char, bool>, string?>("a b c", char.IsLetterOrDigit, " b "),
                Tuple.Create<string?, Func<char, bool>, string?>(" a b c ", char.IsLetterOrDigit, " a b c "),
                Tuple.Create<string?, Func<char, bool>, string?>(".!a#&b;,", char.IsPunctuation, "a#&b"),

            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expression = test.Item2;
                var expected = test.Item3;
                var actual = str.Trim(expression);

                Assert.AreEqual(expected, actual, str);
            }

        }

        [TestMethod()]
        public void ToIntTest()
        {
            var tests = new List<Tuple<string?, int>>
            {

                Tuple.Create<string?,int>(null, 0),
                Tuple.Create<string?,int>("", 0),
                Tuple.Create<string?,int>("a123", 0),
                Tuple.Create<string?,int>(",123", 0),
                Tuple.Create<string?,int>("x", 0),
                Tuple.Create<string?,int>("abc", 0),

                Tuple.Create<string?,int>("127", 127),
                Tuple.Create<string?,int>("  127", 127),
                Tuple.Create<string?,int>("  127  ", 127),
                Tuple.Create<string?,int>(" 127abc", 127),
                Tuple.Create<string?,int>(" 127 123", 127),
                Tuple.Create<string?,int>("127,", 127),

                Tuple.Create<string?,int>("0X7f", 127),
                Tuple.Create<string?,int>(" 0x7F ", 127),
                Tuple.Create<string?,int>(" 0x7Fz ", 127),
                Tuple.Create<string?,int>("0x7F", 127),


                Tuple.Create<string?,int>("127,123", 127123),
                Tuple.Create<string?,int>("987654321", 987654321),

                Tuple.Create<string?,int>("-127", -127),
                Tuple.Create<string?,int>(" -127", -127),
                Tuple.Create<string?,int>(" - 127", -127),
                Tuple.Create<string?,int>(" -127 ", -127),
                Tuple.Create<string?,int>("127-", -127),
                Tuple.Create<string?,int>("- 127.23", -127),

                Tuple.Create<string?,int>(int.MaxValue.ToString(), int.MaxValue),
                Tuple.Create<string?,int>(int.MinValue.ToString(), int.MinValue),
                Tuple.Create<string?,int>("0x" + int.MaxValue.ToString("x"), int.MaxValue),
            };

            foreach (var test in tests)
            {
                var str = test.Item1;
                var expected = test.Item2;
                var actual = test.Item1.ToLong();

                Assert.AreEqual(expected, actual, str);
            }

        }

        [TestMethod()]
        public void HtmlEncodeTextareaTest()
        {
            string?[] tests = new[]
            {
                "\r\n", "\r\n",
                null, "",
                "", "",
                "abc", "abc",
                "<abc>","&lt;abc&gt;",
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].HtmlEncodeTextarea();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void HtmlDecodeTest()
        {
            string?[] tests = new string?[]
            {
                null, "",
                "", "",
                " abc\t", " abc\t",
                "&amp;", "&",
                "&lt", "&lt",
                "&lt;b&gt;", "<b>",
                "<b>", "<b>"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].HtmlDecode();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void UrlEncodeTest()
        {
            string?[] tests = new string?[]
            {
                null, "",
                "", "",
                " ", "+",
                "abc", "abc",
                "a=b", "a%3db",
                "a&b", "a%26b"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].UrlEncode();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void UrlDecodeTest()
        {
            string?[] tests = new string?[]
            {
                null, "",
                "", "",
                "+", " ",
                "abc", "abc",
                "a%3db", "a=b",
                "a%26b", "a&b"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].UrlDecode();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void EscapeStringFormatTest()
        {
            string?[] tests = new string?[]
            {
                null, null,
                "", "",
                " ", " ",
                "abc", "abc",
                "\r\nabc\t", "\r\nabc\t",
                "{0}", "{{0}",
                "hello{0:d}world{1:N}", "hello{{0:d}world{{1:N}"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].EscapeStringFormat();
                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void EscapeCDATATest()
        {
            string?[] tests = new[]
            {
                null, null,
                "", "",
                "abc", "abc",
                "<![that's cool","<![that's cool",
                "must escape]]>", "<![CDATA[must escape]]]]><![CDATA[>]]>",
                "must ]]> escape", "<![CDATA[must ]]]]><![CDATA[> escape]]>",
                "<![CDATA[double encoded]]>", "<![CDATA[<![CDATA[double encoded]]]]><![CDATA[>]]>"
            };
            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].EscapeCDATA();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod()]
        public void HasLowerCaseTest()
        {
            string?[] tests = new[]
            {
                "a",
                "ç",
                "ABC123d",
                "44ñ88"
            };

            foreach (var test in tests)
            {
                Assert.IsTrue(test.HasLowerCase(), test);
            }


            string?[] negativeTests = new[]
            {
                "A",
                "Ç",
                "ABC123D",
                "44Ñ88"
            };

            foreach (var test in negativeTests)
            {
                Assert.IsFalse(test.HasLowerCase(), test);
            }

        }

        [TestMethod()]
        public void HasUpperCaseTest()
        {
            string?[] tests = new[]
            {
                "A",
                "Ç",
                "abc123D",
                "44Ñ88"
            };

            foreach (var test in tests)
            {
                Assert.IsTrue(test.HasUpperCase(), test);
            }


            string?[] negativeTests = new[]
            {
                "a",
                "ç",
                "abc123d",
                "44ñ88"
            };

            foreach (var test in negativeTests)
            {
                Assert.IsFalse(test.HasUpperCase(), test);
            }
        }

        [TestMethod()]
        public void StartsWithAnyTest1()
        {
            Assert.IsTrue("Marcelo".StartsWithAnyIgnoreCase("MARC"));
            Assert.IsTrue("Marcelo".StartsWithAnyIgnoreCase("marc"));
            Assert.IsTrue("Marcelo".StartsWithAnyIgnoreCase("abc", "marc"));
            Assert.IsTrue("Marcelo".StartsWithAnyIgnoreCase("abc", "marc", "efg"));
        }

        [TestMethod()]
        public void IndexOfWhitespaceTest()
        {
            var tests = new List<Tuple<string?, int>>
            {
                Tuple.Create<string?, int>(null, -1),
                Tuple.Create<string?, int>("", -1),
                Tuple.Create<string?, int>(" ", 0),
                Tuple.Create<string?, int>("\t", 0),
                Tuple.Create<string?, int>("abc", -1),
                Tuple.Create<string?, int>("abc ", 3),
                Tuple.Create<string?, int>("abc \t", 3),
                Tuple.Create<string?, int>("abc \tabc ", 3),
            };

            foreach (var test in tests)
            {
                var expected = test.Item2;
                var actual = test.Item1.IndexOfWhitespace();
                Assert.AreEqual(expected, actual, test.Item1);
            }
        }

        [TestMethod()]
        public void LastIndexOfNonWhitespaceTest()
        {
            var tests = new List<Tuple<string?, int>>
            {
                Tuple.Create<string?, int>(null, -1),
                Tuple.Create<string?, int>("", -1),
                Tuple.Create<string?, int>(" ", -1),
                Tuple.Create<string?, int>("\t", -1),
                Tuple.Create<string?, int>("abc", 2),
                Tuple.Create<string?, int>("abc ", 2),
                Tuple.Create<string?, int>("abc \t", 2),
                Tuple.Create<string?, int>("abc \tabc ", 7),
            };

            foreach (var test in tests)
            {
                var expected = test.Item2;
                var actual = test.Item1.LastIndexOfNonWhitespace();
                Assert.AreEqual(expected, actual, test.Item1);
            }
        }

        [TestMethod()]
        public void TruncateTest()
        {
            var tests = new List<Tuple<string?, string?>>
            {
                Tuple.Create<string?, string?>(null, null),
                Tuple.Create<string?, string?>("", ""),
                Tuple.Create<string?, string?>("MyNameIsBlaBla", "MyNameIsBl"),
                Tuple.Create<string?, string?>("My Name Is Bla", "My Name Is"),
                Tuple.Create<string?, string?>("MyNameIs    Bla", "MyNameIs"),
            };

            foreach (var test in tests)
            {
                var expected = test.Item2;
                var actual = test.Item1.Truncate(10);
                Assert.AreEqual(expected, actual);
            }
        }



        [TestMethod()]
        public void ToListFromTabDelimitedLineTest()
        {
            string?[][] tests = new[]
            {
                new string?[] {null, null},
                new[] {"", null},
                new[] {"\t", "", ""},
                new[] {"\t\t", "", "", ""},
                new[] {" \t  \t\\t ", " ", "  ", "\t "},
                new[] {"a\\tb", "a\tb"},

                new[] {"a", "a"},
                new[] {"\ta", "", "a"},
                new[] {"a\t", "a", ""},
                new[] {"a\tb", "a", "b"},
                new[] {" a \t b ", " a ", " b "},
                new[] {"\\ta\t\\tb", "\ta", "\tb"},
                new[] {"\"a\tb\"\tc", "a\tb", "c"},

                new[] {"\\\"\tb", "\"", "b"},
                new[] {"\"ab\"\tc", "ab", "c"},
                new[] {"a\\\"b\tc", "a\"b", "c"},
            };

            foreach (var test in tests)
            {
                var row = test[0].ToListFromTabDelimitedLine();

                if (row == null)
                {
                    Assert.IsNull(test[1], "test: " + test[0]);
                }
                else
                {
                    Assert.AreEqual(row.Count, test.Length - 1, "test: " + test[0]);
                    for (int i = 0; i < row.Count; i++)
                    {
                        Assert.AreEqual(row[i], test[i + 1], "test: " + test[0] + " - Item " + i);
                    }
                }
            }

        }

        private enum EnumTest
        {
            One = 1,
            Two = 2,
            Three = 3
        };

        [Flags]
        private enum EnumFlagsTest : byte
        {
            First = 0x01,
            Second = 0x02,
            Third = 0x04,

            FirstAndSecond = First | Second,
            All = First | Second | Third,
        }

        [TestMethod()]
        public void ToEnumTest()
        {

            var tests = new List<Tuple<string?, EnumTest>>
            {
                Tuple.Create<string?, EnumTest>((string?)null, EnumTest.One),
                Tuple.Create<string?, EnumTest>("", EnumTest.One),
                Tuple.Create<string?, EnumTest>("\t", EnumTest.One),
                Tuple.Create<string?, EnumTest>("nada", EnumTest.One),
                Tuple.Create<string?, EnumTest>("one", EnumTest.One),
                Tuple.Create<string?, EnumTest>(" two\t", EnumTest.Two),
                Tuple.Create<string?, EnumTest>("Three", EnumTest.Three),
            };

            foreach (var test in tests)
            {
                var expected = test.Item2;
                var actual = test.Item1.ToEnum<EnumTest>(EnumTest.One);
                Assert.AreEqual(expected, actual, test.Item1);
            }

            var tests2 = new List<Tuple<string?, EnumFlagsTest>>
            {
                Tuple.Create<string?, EnumFlagsTest>((string?)null, EnumFlagsTest.First),
                Tuple.Create<string?, EnumFlagsTest>("", EnumFlagsTest.First),
                Tuple.Create<string?, EnumFlagsTest>(" ", EnumFlagsTest.First),
                Tuple.Create<string?, EnumFlagsTest>("first", EnumFlagsTest.First),
                Tuple.Create<string?, EnumFlagsTest>(" second ", EnumFlagsTest.Second),
                Tuple.Create<string?, EnumFlagsTest>("firstandsecond", EnumFlagsTest.FirstAndSecond),
                Tuple.Create<string?, EnumFlagsTest>("firstandsecond", EnumFlagsTest.First | EnumFlagsTest.Second),
                Tuple.Create<string?, EnumFlagsTest>("third", EnumFlagsTest.Third),
                Tuple.Create<string?, EnumFlagsTest>("7", EnumFlagsTest.All),
                Tuple.Create<string?, EnumFlagsTest>("all", EnumFlagsTest.All),
                Tuple.Create<string?, EnumFlagsTest>("6", EnumFlagsTest.Second | EnumFlagsTest.Third),

            };

            foreach (var test in tests2)
            {
                var expected = test.Item2;
                var actual = test.Item1.ToEnum<EnumFlagsTest>(EnumFlagsTest.First);

                Assert.AreEqual(expected, actual, test.Item1);
            }



        }

        [TestMethod()]
        public void HtmlEncodePreTest()
        {
            string?[] tests = new[]
            {
                "\r\n", "<br>",
                null, "",
                "", "",
                "abc", "abc",
                "<abc>","&lt;abc&gt;",
                "\tabc", "&nbsp;&nbsp;&nbsp;&nbsp;abc"
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].HtmlEncodePre();
                Assert.AreEqual(expected, actual);
            }

        }

        [TestMethod()]
        public void IndexOfAnyTest()
        {
            var tests = new List<Tuple<string?, string?[]?, int>>
            {
                Tuple.Create<string?, string?[]?, int>("",  null, -1),
                Tuple.Create<string?, string?[]?, int>(" ", null, -1),
                Tuple.Create<string?, string?[]?, int>("abc",  null, -1),

                Tuple.Create<string?, string?[]?, int>("", new[] {""}, -1),
                Tuple.Create<string?, string?[]?, int>("", new string?[] {null}, -1),
                Tuple.Create<string?, string?[]?, int>("", new[] {"abc"}, -1),
                Tuple.Create<string?, string?[]?, int>(null, new[] {"abc"}, -1),
                Tuple.Create<string?, string?[]?, int>("", new[] {"abc", ""}, -1),


                Tuple.Create<string?, string?[]?, int>("a", new[] {"a"}, 0),
                Tuple.Create<string?, string?[]?, int>("ab", new[] {"ab"}, 0),
                Tuple.Create<string?, string?[]?, int>("ab", new[] {"b", "a"}, 0),
                Tuple.Create<string?, string?[]?, int>("ab", new[] {"b", "c"}, 1),


                Tuple.Create<string?, string?[]?, int>("http://et.com phone https://home", new[] {"http://", "https://"}, 0),
                Tuple.Create<string?, string?[]?, int>("http://et.com phone https://home", new[] {"https://", "HTTP://"}, 0),
                Tuple.Create<string?, string?[]?, int>("http://et.com phone https://home", new[] {"https://", "ftp://"}, 20),


            };

            foreach (var test in tests)
            {
                var expected = test.Item3;
                var actual = test.Item1.IndexOfAny(test.Item2);
                Assert.AreEqual(expected, actual, test.Item1);
            }

        }

        [TestMethod]
        public void HtmlifyTest()
        {
            string?[] tests = new[]
            {
                null, "",
                "", "",
                "abc", "abc",
                "a&b", "a&amp;b",
                "<test>", "&lt;test&gt;",
                "abc.com", "<a href=\"https://abc.com\">abc.com</a>",
                "www.nyt.com", "<a href=\"https://www.nyt.com\">www.nyt.com</a>",
                "http://calbucci.com", "<a href=\"http://calbucci.com\">calbucci.com</a>",
                "Go to abc.com", "Go to <a href=\"https://abc.com\">abc.com</a>",
                "Join abc.com or def.com.", "Join <a href=\"https://abc.com\">abc.com</a> or <a href=\"https://def.com\">def.com</a>.",
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].Htmlify();
                Assert.AreEqual(expected, actual);
            }
        }

        [TestMethod]
        public void HtmlifyWithExtensionTest()
        {
            string?[] tests = new[]
            {
                null, "",
                "", "",
                "abc", "abc",
                "a&b", "a&amp;b",
                "<test>", "&lt;test&gt;",
                "abc.com", "<a href=\"https://abc.com\">abc.com</a>",
                "www.nyt.com", "<a href=\"https://www.nyt.com\">www.nyt.com</a>",
                "http://calbucci.com", "<a href=\"http://calbucci.com\">calbucci.com</a>",
                "Go to abc.com", "Go to <a href=\"https://abc.com\">abc.com</a>",
                "Join abc.com or def.com.", "Join <a href=\"https://abc.com\">abc.com</a> or <a href=\"https://def.com\">def.com</a>.",
                "This #test", "This <a href=\"https://twitter.com/hashtag/test\">#test</a>",
                "This #bad-hash", "This #bad-hash",
                "This #good!", "This <a href=\"https://twitter.com/hashtag/good\">#good</a>!",
            };

            Func<string, string?> hashfier = (token) =>
            {
                if (!token.StartsWith("#") || token.Length < 3)
                    return StringExtensions.DefaultLinkifier(token);

                char lastChar = token[token.Length - 1];
                string? suffix = null;
                if (".?!),([]{};:'\"<>".Contains(lastChar))
                {
                    token = token.Substring(0, token.Length - 1);
                    suffix = lastChar.ToString();
                }

                var hash = token.Substring(1);
                if (hash.IndexOfNonLetterOrDigit(1) >= 0)
                    return StringExtensions.DefaultLinkifier(token);

                return $"<a href=\"https://twitter.com/hashtag/{hash}\">{token}</a>" + suffix;
            };

            for (int i = 0; i < tests.Length; i += 2)
            {
                var expected = tests[i + 1];
                var actual = tests[i].Htmlify(hashfier);
                Assert.AreEqual(expected, actual);
            }
        }

    }

}