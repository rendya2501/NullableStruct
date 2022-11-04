using System;

namespace NullableDictionary.Experiment
{
    /// <summary>
    /// Dictionaryのキーにnullを許容させるための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 構造体はnullになりえない性質なので、nullを構造体でラップすることでDictionaryを騙す。
    /// </remarks>
    public struct Nullable_Test<T> : IEquatable<Nullable_Test<T>>
    {
        /// <summary>
        /// 入力値
        /// </summary>
        public T Value { get; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value"></param>
        /// <remarks>
        /// 不要なNewを避けるためPrivateで宣言する。
        /// Dictionaryからコンストラクタが呼び出されるのではなく、implicit operator経由でたどり着く。
        /// </remarks>
        public Nullable_Test(T value) => Value = value;

        /// <summary>
        /// 演算子のオーバーロード。
        /// == 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(Nullable_Test<T> x, Nullable_Test<T> y) => x.Equals(y);
        /// <summary>
        /// 演算子のオーバーロード。
        /// =! 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(Nullable_Test<T> x, Nullable_Test<T> y) => !x.Equals(y);
        /// <summary>
        /// Nullable<T>型をT型に変換します。
        /// 「var b1 = new Nullable<double>(1); var b2 = (int)b1;」等のキャストを行ったときに呼び出される。
        /// explicit:明示的にキャストしないとエラー。new Dictionary<Nullable<int?>, string>(){(Nullable<int?>)null, "null" }→こうしないといけない。
        /// implicit:明示的にキャストしなくてもエラーにならない。 new Dictionary<Nullable<int?>, string>(){null, "null" }→これでもOK。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(Nullable_Test<T> source) => source.Value;
        /// <summary>
        /// T型をNullable<T>型に変換します。
        /// 変換するT型の値をコンストラクタの引数としてNullable<T>型を生成することでT型をNullable<T>型に変換します。
        /// 「(Nullable<T>)null」 等のキャストを行ったときに呼び出される。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator Nullable_Test<T>(T source) => new Nullable_Test<T>(source);
        /// <summary>
        /// ToStringのオーバーロード。
        /// Dictionaryへの入力以外に使う予定はないので、これは実装しない。
        /// </summary>
        /// <returns></returns>
        public override string ToString() => Value?.ToString();

        /// <summary>
        /// ハッシュコードを取得します。
        /// nullの場合は0を返却します。
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// int 0の場合とハッシュ値が同じになるのはなぁ。
        /// でもハッシュが同じでもちゃんと動作するしな。なんなんだろう。
        /// </remarks>
        public override int GetHashCode() => Value == null ? 0 : Value.GetHashCode();

        public override bool Equals(object obj)
        {
            if (obj is Nullable_Test<T> nullable)
            {
                //return Equals(nullable);
                return ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value);
            }
            else
            {
                return false;
            }

            // 1行にできる。
            //public override bool Equals(object obj) => obj is Nullable<T> nullable && Equals(nullable);
        }
        /// <summary>
        /// IEquatable実装メソッドです。
        /// 速度向上のために実装しています。
        /// </summary>
        /// <param name="nullable"></param>
        /// <returns></returns>
        public bool Equals(Nullable_Test<T> nullable) => ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value);
    }

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
        public override bool Equals(object obj) => obj is Nullable_Test<T> nullable && Equals(nullable);
    }

}
