using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrekBikes
{
    public static class Extensions
    {
        public static bool HasKeyLike<T>(this Dictionary<string, T> collection, string value)
        {
            var keysLikeCount = collection.Keys.Count(x => x.ToLower().StartsWith(value.ToLower()));
            return keysLikeCount > 0;
        }

        public static List<string> GetKeysLike<T>(this Dictionary<string, T> collection, string value)
        {
            return collection.Keys.Where(x => x.ToLower().StartsWith(value.ToLower())).ToList();
        }
    }

}
