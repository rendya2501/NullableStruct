using NullableDictionary.Experiment;
using NullableDictionary.ItemsSource;
using NullableDictionary.Struct;
using System;
using static NullableDictionary.DictionaryConverter;

namespace NullableStruct
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // test1
            foreach (var item in IntItemsSource.IntItems)
            {
                Console.WriteLine(item.Key + " " + item.Value);
                Console.WriteLine(Convert(item.Key, IntItemsSource.IntItems));
            }

            // test2
            foreach (var item in ValidItemsSource.ValidItems)
            {
                Console.WriteLine(item.Key + " " + item.Value);
                Console.WriteLine(Convert(item.Key, ValidItemsSource.ValidItems));
            }

            //nullとint0のハッシュが同じでも、Valueは違うからキーとして認識させるには十分なのでは？
            _ = new NullableStruct<int?>(null) == new NullableStruct<int?>(0);

            // IEquatable速度比較
            Benchmark(10_000_000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        static void Benchmark(int n)
        {
            var sw = new System.Diagnostics.Stopwatch();

            // 通常 : 2635ms
            sw.Start();
            var xs = new[] { new NullableNotEquatable<int?>(null), };
            for (int i = 0; i < n; i++) Array.IndexOf(xs, xs[0]);
            sw.Stop();
            Console.WriteLine($"Point             = {sw.ElapsedMilliseconds}ms");

            // OverLoadのみ : 286ms
            sw.Restart();
            var ys = new[] { new NullableOverLoad<int?>(null), };
            for (int i = 0; i < n; i++) Array.IndexOf(ys, ys[0]);
            sw.Stop();
            Console.WriteLine($"Point(OverLoad)   = {sw.ElapsedMilliseconds}ms");

            // IEquatable実装 : 717ms
            sw.Restart();
            var zs = new[] { new Nullable_Test<int?>(null), };
            for (int i = 0; i < n; i++) Array.IndexOf(zs, zs[0]);
            sw.Stop();
            Console.WriteLine($"Point(IEquatable) = {sw.ElapsedMilliseconds}ms");
        }
    }
}
