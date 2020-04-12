using System.Collections.Generic;

namespace Chasm.Proxys.Modules.Parsers
{
    public interface IParser<T>
    {
        public HashSet<string> Parse(T source, string regex);
    }
}
