using System;
using System.Collections;

namespace NullableDictionary
{
    /// <summary>
    /// DictionaryのKeyをValueに変換するコンバーター
    /// </summary>
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
            if (!(parameter is IDictionary)) throw new Exception("型");
            // パラメータの型変換
            var dictionary = (IDictionary)parameter;
            // インデクサーで値を取得
            return dictionary[value];
        }
    }
}
