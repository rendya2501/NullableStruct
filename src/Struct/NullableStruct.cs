namespace NullableDictionary.Struct
{
    /// <summary>
    /// Dictionaryのキーにnullを許容させるための構造体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <remarks>
    /// 構造体はnullになりえない性質なので、nullを構造体でラップすることでDictionaryを騙す。
    /// </remarks>
    public struct NullableStruct<T>
    {
        /// <summary>
        /// 入力値
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="value">入力値</param>
        public NullableStruct(T value) => Value = value;

        /// <summary>
        /// 演算子のオーバーロード。
        /// == 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator ==(NullableStruct<T> x, NullableStruct<T> y) => x.Equals(y);
        /// <summary>
        /// 演算子のオーバーロード。
        /// != 演算子を使ってxとyの比較を行った場合に呼び出される。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool operator !=(NullableStruct<T> x, NullableStruct<T> y) => !x.Equals(y);
        /// <summary>
        /// キャストのオーバーロード
        /// Nullable<T>型をT型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator T(NullableStruct<T> source) => source.Value;
        /// <summary>
        /// キャストのオーバーロード。
        /// T型をNullable<T>型に変換する時に呼び出されます。
        /// </summary>
        /// <param name="source"></param>
        public static implicit operator NullableStruct<T>(T source) => new NullableStruct<T>(source);

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
            obj is NullableStruct<T> nullable
            && (ReferenceEquals(Value, nullable.Value) || Value.Equals(nullable.Value));
    }
}
