using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter
{
    public static class ResultCache
    {
        public static ConcurrentBag<string> Bag = new ConcurrentBag<string>();
    }
}
