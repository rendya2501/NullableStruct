using System.Collections.Generic;
using NullableDictionary.Struct;

namespace NullableDictionary.ItemsSource
{
    public static class ValidItemsSource
    {
        public static Dictionary<NullableStruct<bool?>, string> ValidItems =>
            new Dictionary<NullableStruct<bool?>, string>()
            {
                {null, "すべて" },
                {true, "使用する" },
                {false, "使用しない" }
            };
    }
}
