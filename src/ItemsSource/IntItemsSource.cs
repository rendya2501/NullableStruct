using System.Collections.Generic;
using NullableDictionary.Struct;

namespace NullableDictionary.ItemsSource
{
    public static class IntItemsSource
    {
        public static Dictionary<NullableStruct<int?>, string> IntItems =>
            new Dictionary<NullableStruct<int?>, string>()
            {
                {null, "null" },
                {0, "0" },
                {1, "1" }
            };
    }
}
