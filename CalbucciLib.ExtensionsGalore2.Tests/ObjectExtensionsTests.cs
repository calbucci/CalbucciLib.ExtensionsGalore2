using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CalbucciLib.ExtensionsGalore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static System.Net.Mime.MediaTypeNames;

namespace CalbucciLib.ExtensionsGalore.Tests
{
    [TestClass()]
    public class ObjectExtensionsTests
    {
        private void TestSet(object[] tests)
        {
            foreach (var test in tests)
            {
                var copy = test.DeepCopy();
                if (test == null && copy == null)
                    continue;

                Assert.AreEqual(test, copy, test != null ? test.ToString() : "null");

                if (test != null)
                {
                    Assert.AreEqual(test.GetType(), copy!.GetType());
                    if (test.GetType() != typeof(string))
                        Assert.AreNotSame(test, copy, test.ToString());
                }
            }
        }

        private void TestIDictionary(IDictionary dictionary)
        {
            var copy = ((object)dictionary).DeepCopy() as IDictionary;
            Assert.IsNotNull(copy);
            Assert.AreNotSame(dictionary, copy);
            Assert.AreEqual(dictionary.GetType(), copy.GetType());
            Assert.AreEqual(dictionary.Count, copy.Count);

            foreach (DictionaryEntry item in dictionary)
            {
                bool found = false;
                foreach (var keyCopy in copy.Keys)
                {
                    if (item.Key.Equals(keyCopy))
                    {
                        if (keyCopy.GetType() != typeof(string))
                            Assert.AreNotSame(keyCopy, item.Key);
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found);

                var valueCopy = copy[item.Key];

                if (item.Value != null)
                {
                    Assert.AreEqual(item.Value, valueCopy);
                    if (item.Value.GetType() != typeof(string))
                        Assert.AreNotSame(item.Value, valueCopy);
                    Assert.AreEqual(item.Value.GetType(), valueCopy!.GetType());
                }
                else
                {
                    Assert.IsNull(valueCopy);
                }
            }
        }

        private void TestIList(IList list)
        {
            // This casting is necessary so we don't call the ListExtension
            var copy = ((object)list).DeepCopy() as IList;
            Assert.IsNotNull(copy);

            Assert.AreEqual(list.GetType(), copy.GetType());
            Assert.AreNotSame(list, copy);
            Assert.AreEqual(list.Count, copy.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var val1 = list[i];
                var val2 = copy[i];

                if (val1 != null)
                {
                    Assert.AreEqual(val1, val2);
                    if (val1.GetType() != typeof(string))
                        Assert.AreNotSame(val1, val2);
                    Assert.AreEqual(val1.GetType(), val2!.GetType());
                }
                else
                {
                    Assert.IsNull(val2);
                }
            }
        }

        private void TestICollection(ICollection collection)
        {
            var copy = ((object)collection).DeepCopy() as ICollection;
            Assert.IsNotNull(copy);
            Assert.AreEqual(collection.GetType(), copy.GetType());
            Assert.AreNotSame(collection, copy);
            Assert.AreEqual(collection.Count, copy.Count);

            var enumeratorCopy = copy.GetEnumerator();
            foreach (var item in collection)
            {
                enumeratorCopy.MoveNext();
                var itemCopy = enumeratorCopy.Current;
                Assert.AreEqual(item, itemCopy);
                if (item != null)
                {
                    if (item.GetType() != typeof(string))
                        Assert.AreNotSame(item, itemCopy);
                    Assert.AreEqual(item.GetType(), itemCopy.GetType());
                }
            }

        }

        [TestMethod()]
        public void DeepCopy_BaseType_Tests()
        {
            var tests = new object[]
            {
                'a',
                (byte)11,
                17,
                true,
                3.14,
                (decimal)6.28,
                (float)12.3
            };

            TestSet(tests);

        }

        [TestMethod()]
        public void DeepCopy_String_Tests()
        {
            string[] tests = new[]
            {
                "",
                "abc"
            };

            TestSet(tests);
        }

        struct TestStruct
        {
            public int a;
            public string? s;

            //public override bool Equals([NotNullWhen(true)] object? obj)
            //{
            //	var rhs = (TestStruct)obj;
            //	return rhs.a == a && rhs.s == s;
            //}

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                if (obj == null)
                    return false;
                var rhs = (TestStruct)obj;
                return rhs.a == a && rhs.s == s;
            }

            public override int GetHashCode()
            {
                return a ^ s?.GetHashCode() ?? 0;
            }
        }

        [TestMethod()]
        public void DeepCopy_Struct_Tests()
        {
            object[] tests = new object[]
            {
                Color.White,
                new TestStruct { a = 1, s = "a"}
            };

            TestSet(tests);
        }


        [TestMethod()]
        public void DeepCopy_Array_Tests()
        {
            object[] tests = new object[]
            {
                new int[] { 1 },
                new int[] { 1, 2, 3},
                new double[] { 1.1, 2.2, 3.3},
                new byte[0],
                new string?[] { "abc", "def"}
            };

            foreach (Array test in tests)
            {
                var copy = test.DeepCopy() as Array;
                Assert.IsNotNull(copy);
                Assert.AreEqual(test.GetType(), copy.GetType());
                Assert.AreEqual(test.Length, copy.Length);
                for (int i = 0; i < test.Length; i++)
                {
                    Assert.AreEqual(test.GetValue(i), copy.GetValue(i));
                    if (test.GetValue(i)!.GetType() != typeof(string))
                        Assert.AreNotSame(test.GetValue(i), copy.GetValue(i));
                }
            }

        }

        [TestMethod()]
        public void DeepCopy_AnonymousType_Tests()
        {
            var anon = new { age = 17, name = "Peter", weight = 153.2 };
            var copy = anon.DeepCopy();
            Assert.IsNotNull(copy);
            Assert.AreEqual(anon.GetType(), copy.GetType());
            Assert.AreEqual(anon, copy);
            Assert.AreNotSame(anon, copy);

            Assert.AreEqual(anon.age, copy.GetType().GetProperty("age")!.GetValue(copy));
            Assert.AreEqual(anon.name, copy.GetType().GetProperty("name")!.GetValue(copy));
            Assert.AreEqual(anon.weight, copy.GetType().GetProperty("weight")!.GetValue(copy));
        }

        class TestClass
        {
            public string? s;
            public int i;

            public override bool Equals([NotNullWhen(true)] object? obj)
            {
                if (obj == null)
                    return false;

                var rhs = obj as TestClass;
                if (rhs == null)
                    return false;
                return rhs.s == s && rhs.i == i;
            }

            public override int GetHashCode()
            {
                return i ^ s?.GetHashCode() ?? 0;
            }
        }

        [TestMethod()]
        public void DeepCopy_Class_Tests()
        {
            object[] tests = {
                Tuple.Create("abc", 123, "def", 456),
                new TestClass {s = "ghi", i = 789}
            };

            TestSet(tests);
        }


        [TestMethod()]
        public void DeepCopy_ArrayList_Tests()
        {
            ArrayList al = new ArrayList();
            al.Add(123);
            al.Add("abc");
            al.Add(7.3);
            al.Add(true);

            TestIList(al);
        }

        [TestMethod()]
        public void DeepCopy_Queue_Tests()
        {
            Queue queue = new Queue();
            queue.Enqueue("abc");
            queue.Enqueue(123);
            queue.Enqueue(new { name = "Marcelo", city = "Seattle" });
            queue.Enqueue(Color.White);

            TestICollection(queue);
        }


        [TestMethod()]
        public void DeepCopy_Stack_Tests()
        {
            Stack stack = new Stack();
            stack.Push("abc");
            stack.Push(123);
            stack.Push(new { name = "Marcelo", city = "Seattle" });
            stack.Push(Color.White);

            TestICollection(stack);

        }


        [TestMethod()]
        public void DeepCopy_Hashtable_Tests()
        {
            Hashtable hashtable = new Hashtable();
            hashtable["abc"] = "def";
            hashtable[123] = 456;
            hashtable["Record"] = new { name = "Marcelo", city = "Seattle" };
            hashtable["Color"] = Color.White;

            TestIDictionary(hashtable);

        }

        [TestMethod()]
        public void DeepCopy_BitArray_Tests()
        {
            Random r = new Random(3);
            var bytes = new byte[7];
            r.NextBytes(bytes);
            BitArray bitArray = new BitArray(bytes);

            TestICollection(bitArray);

        }

        [TestMethod()]
        public void DeepCopy_SortedList_Tests()
        {
            SortedList sortedList = new SortedList();
            sortedList["not"] = null;
            sortedList["abc"] = "def";
            sortedList["number"] = 456;
            sortedList["Record"] = new { name = "Marcelo", city = "Seattle" };
            sortedList["Color"] = Color.White;

            TestIDictionary(sortedList);

        }

        [TestMethod()]
        public void DeepCopy_DictionaryT_Tests()
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>();
            dictionary["abc"] = "def";
            dictionary["123"] = 456;
            dictionary["Record"] = new { name = "Marcelo", city = "Seattle" };
            dictionary["Color"] = Color.White;

            TestIDictionary(dictionary);


        }

        [TestMethod()]
        public void DeepCopy_ListT_Tests()
        {
            var list = new List<string?>()
            {
                null,
                "",
                "abc"
            };

            TestIList(list);
        }

        [TestMethod()]
        public void DeepCopy_HashSetT_Tests()
        {
            var dictionary = new Dictionary<string, object>
            {
                ["abc"] = "def",
                ["123"] = 456,
                ["Record"] = new { name = "Marcelo", city = "Seattle" },
                ["Color"] = Color.White
            };

            TestIDictionary(dictionary);
        }


        [TestMethod()]
        public void DeepCopy_LinkedListT_Tests()
        {
            var linkedList = new LinkedList<TestClass?>();

            linkedList.AddLast(new LinkedListNode<TestClass?>(null));
            linkedList.AddLast(new TestClass { i = 1, s = "a" });
            linkedList.AddLast(new TestClass { i = 2, s = "b" });

            TestICollection(linkedList);
        }


        [TestMethod()]
        public void DeepCopy_QueueT_Tests()
        {
            var queue = new Queue<Color>();
            queue.Enqueue(Color.Blue);
            queue.Enqueue(Color.Black);
            queue.Enqueue(Color.DarkGoldenrod);

            TestICollection(queue);

        }

        [TestMethod()]
        public void DeepCopy_StackT_Tests()
        {
            var stack = new Stack<object?>();

            stack.Push("abc");
            stack.Push(null);
            stack.Push(123);
            stack.Push(new { name = "Marcelo" });
            stack.Push(new TestClass { i = 7, s = "z" });

            TestICollection(stack);
        }


        [TestMethod()]
        public void DeepCopy_SortedDictionaryT_Tests()
        {
            SortedDictionary<string, object> sortedDictionary = new SortedDictionary<string, object>();
            sortedDictionary["abc"] = "def";
            sortedDictionary["123"] = 456;
            sortedDictionary["Record"] = new { name = "Marcelo", city = "Seattle" };
            sortedDictionary["Color"] = Color.White;

            TestIDictionary(sortedDictionary);

        }


        [TestMethod()]
        public void DeepCopy_SortedSetT_Tests()
        {
            var sortedSet = new SortedSet<string?>()
            {
                null,
                "abc"
            };

            TestICollection(sortedSet);
        }


        [TestMethod()]
        public void DeepCopy_SortedListT_Tests()
        {
            SortedList<string, object> sortedList = new SortedList<string, object>();
            sortedList["abc"] = "def";
            sortedList["123"] = 456;
            sortedList["Record"] = new { name = "Marcelo", city = "Seattle" };
            sortedList["Color"] = Color.White;

            TestIDictionary(sortedList);
        }

        [TestMethod()]
        public void DeepCopy_ConcurrentDictionaryT_Tests()
        {
            ConcurrentDictionary<string, object> concurrentDictionary = new ConcurrentDictionary<string, object>();
            concurrentDictionary["abc"] = "def";
            concurrentDictionary["123"] = 456;
            concurrentDictionary["Record"] = new { name = "Marcelo", city = "Seattle" };
            concurrentDictionary["Color"] = Color.White;

            TestIDictionary(concurrentDictionary);

        }

        [TestMethod()]
        public void DeepCopy_ConcurrentQueueT_Tests()
        {
            ConcurrentQueue<Color> queue = new ConcurrentQueue<Color>();
            queue.Enqueue(Color.Blue);
            queue.Enqueue(Color.Black);
            queue.Enqueue(Color.DarkGoldenrod);

            TestICollection(queue);

        }

        [TestMethod()]
        public void DeepCopy_ConcurrentStackT_Tests()
        {
            var stack = new ConcurrentStack<object?>();

            stack.Push("abc");
            stack.Push(null);
            stack.Push(123);
            stack.Push(new { name = "Marcelo" });
            stack.Push(new TestClass { i = 7, s = "z" });

            TestICollection(stack);

        }

        public void DeepCopy_DeepObject_Tests()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();

            Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
            dictionary1["abc"] = "def";
            dictionary1["123"] = 456;
            dictionary1["Record"] = new { name = "Marcelo", city = "Seattle" };
            dictionary1["Color"] = Color.White;
            dictionary1["Class"] = new TestClass { i = 9, s = "k" };
            list.Add(dictionary1);

            Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
            dictionary2["ghi"] = Tuple.Create(1, "abc");
            dictionary2["789"] = new Stack<string>();
            ((Stack<string>)dictionary2["789"]).Push("Push it");
            dictionary2["NewRecord"] = new { name = "Marcelo", city = "Seattle" };
            dictionary2["Struct"] = new TestStruct { a = 27, s = "t" };
            list.Add(dictionary2);

            var listCopy = ((object)list).DeepCopy() as IList;
            Assert.IsNotNull(listCopy);

            Assert.AreEqual(list.GetType(), listCopy.GetType());
            Assert.AreNotSame(list, listCopy);
            Assert.AreEqual(list.Count, listCopy.Count);

            for (int i = 0; i < list.Count; i++)
            {
                var dict = (IDictionary)list[i];
                var dictCopy = listCopy[i] as IDictionary;
                Assert.IsNotNull(dictCopy);

                Assert.AreNotSame(dict, dictCopy);
                Assert.AreEqual(dict.GetType(), dictCopy.GetType());

                foreach (DictionaryEntry item in dict)
                {
                    bool found = false;
                    foreach (var keyCopy in dictCopy.Keys)
                    {
                        if (item.Key.Equals(keyCopy))
                        {
                            found = true;
                            break;
                        }
                    }
                    Assert.IsTrue(found);

                    var valueCopy = dictCopy[item.Key];
                    Assert.IsNotNull(valueCopy);
                    Assert.AreEqual(item.Value, valueCopy);
                    if (item.Value != null)
                    {
                        if (item.Value!.GetType() != typeof(string))
                            Assert.AreNotSame(item.Value, valueCopy);
                        Assert.AreEqual(item.Value.GetType(), valueCopy.GetType());
                    }
                }
            }


        }

        //[TestMethod()]
        //public void Object_ToJson()
        //{
        //    List<Tuple<object, string>> tests = new List<Tuple<object, string>>()
        //    {
        //        Tuple.Create<object, string>(null, "{}"),
        //        Tuple.Create<object, string>(17, "17"),
        //        Tuple.Create<object, string>("hello", "\"hello\""),
        //        Tuple.Create<object, string>(new int[] {1, 3}, "[1,3]"),
        //        Tuple.Create<object, string>(new
        //        {
        //            Hello = "World",
        //            Number = 17
        //        }, "{\"Hello\":\"World\",\"Number\":17}")
        //    };

        //    foreach (var t in tests)
        //    {
        //        var expected = t.Item2;
        //        var actual = t.Item1.ToJson();

        //        Assert.AreEqual(expected, actual);
        //    }
        //}


    }
}
