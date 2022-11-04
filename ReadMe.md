# NullableStruct

---

## 概要

KeyValuePairではなくDictionaryで定数を定義しようとした。  
しかし、Dictionaryはキーをnullにすることができない。  
なんとかならないかと調べたのがきっかけ。  

nullを構造体でラップすることでDictionaryを騙す方法でうまくいくことを発見した。  
因みになぜ構造体だったかというと、実装が楽だったから。  
IDictionary\<K,V>を実装しようとした場合、たくさん実装しないといけなくて、それでは実装できなかったので構造体を使ってラップすることにした。  
サンプルもあったからね。  

しかし、結局、KeyValuePairのnullをコンバートできる方法を見つけたので、これはお蔵入りになったが備忘録として残す。  

[Why doesn't Dictionary<TKey, TValue> support null key? [duplicate]](https://stackoverflow.com/questions/4632945/dictionary-with-null-key)  
[IReadOnlyDictionary<TKey, TValue> の TValue は共変の型パラメーターではない](https://qiita.com/chocolamint/items/9f13fe7e3c6343f898c2)  
[Dictionary<TKey, TValue>はキーとしてnullを許容しない](https://qiita.com/RyotaMurohoshi/items/03937297810e7c9aaf8b)  

---

## NullableStructの実装

``` C#
    /// <summary>
    /// Dictionaryのキーにnullを許容させるための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 構造体はnullになりえない性質なので、nullを構造体でラップすることでDictionaryを騙す。
    /// </remarks>
    public struct Nullable<T>
    {
        /// <summary>
        /// 入力値
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">入力値</param>
        private Nullable(T value) => Value = value;

        /// <summary>
        /// 演算子のオーバーロード。
        /// == 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Nullable<T> x, Nullable<T> y) => x.Equals(y);
        /// <summary>
        /// 演算子のオーバーロード。
        /// != 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Nullable<T> x, Nullable<T> y) => !x.Equals(y);
        /// <summary>
        /// キャストのオーバーロード
        /// Nullable<T>型をT型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Nullable<T> source) => source.Value;
        /// <summary>
        /// キャストのオーバーロード。
        /// T型をNullable<T>型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Nullable<T>(T source) => new Nullable<T>(source);

        /// <summary>
        /// ハッシュコードを取得します。
        /// nullの場合は0を返却します。
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
        /// <summary>
        /// 2つのオブジェクト インスタンスが等しいかどうかを判断します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => 
            obj is Nullable<T> nullable
            && (ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value));
    }
```

---

## NullableStructの使い方

`Nullable<bool?>`のようにnull許容型をNullableでラップしてDictionaryに渡す。  

``` C# : ItemsSource
    public static class ValidItemsSource
    {
        public static Dictionary<Nullable<bool?>, string> ValidItems =>
            new Dictionary<Nullable<bool?>, string>()
            {
                {null, "すべて" },
                {true, "使用する" },
                {false, "使用しない" }
            };
    }
```

コンバーターをかませる。  

``` C# : Converter
    public static class DictionaryConverter
    {
        /// <summary>
        /// DictionaryのKeyをValueに変換します。
        /// </summary>
        /// <param name="value">バインディング ソースによって生成された値</param>
        /// <param name="targetType">バインディング ターゲット プロパティの型</param>
        /// <param name="parameter">使用するコンバーター パラメーター</param>
        /// <param name="culture">コンバーターで使用するカルチャ</param>
        /// <returns></returns>
        public static object Convert(object value, object parameter)
        {
            if (!(parameter is IDictionary))
                throw new Exception("型");
            // パラメータの型変換
            var dictionary = (IDictionary)parameter;
            // インデクサーで値を取得
            return dictionary[value];
        }
    }
```

MVVMのバインドでコンバートした場合を想定しての動作だが、これでnullでもエラーにならずに目的の値を出力することができる。  

``` C#
    foreach (var item in ValidItemsSource.ValidItems)
    {
        Console.WriteLine(Convert(item.Key, ValidItemsSource.ValidItems));
    }
```

---

## 速度エビデンス

[構造体を定義すると Equals が自動的に実装されるが、IEquatable\<T> を実装した方がよい](http://noriok.hatenadiary.jp/entry/2017/07/17/233146)  

こんな記事を見つけたので、ついでに実装して速度を図ってみたら、OverLoadのほうが早かったというやつ。  

``` txt
Point             = 1928ms
Point(OverLoad)   = 252ms
Point(IEquatable) = 493ms
```

OverLoadしたソースはこれ。  

``` C#
    /// <summary>
    /// 速度比較用
    /// オーバーロードのみ
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct NullableOverLoad<T>
    {
        public T Value { get; }
        public NullableOverLoad(T value) => Value = value;
        public static bool operator ==(NullableOverLoad<T> x, NullableOverLoad<T> y) => x.Equals(y);
        public static bool operator !=(NullableOverLoad<T> x, NullableOverLoad<T> y) => !x.Equals(y);
        public static implicit operator T(NullableOverLoad<T> source) => source.Value;
        public static implicit operator NullableOverLoad<T>(T source) => new NullableOverLoad<T>(source);
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
        public override bool Equals(object obj) => obj is Nullable<T> nullable && Equals(nullable);
    }
```

一番速度出なかった奴はこれ。  

``` C#
    /// <summary>
    /// 速度比較用 IEquatableなしVer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct NullableNotEquatable<T>
    {
        public T Value { get; }
        public NullableNotEquatable(T value) => Value = value;
        public static implicit operator T(NullableNotEquatable<T> source) => source.Value;
        public static implicit operator NullableNotEquatable<T>(T source) => new NullableNotEquatable<T>(source);
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();
    }
```
