using Chasm.Proxys.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Chasm.Proxys.Modules.Parsers
{
    public class StringParser : IParser<string>
    {

        public HashSet<string> Parse(string source, string regex = Defaults.PROXY_PARSER_REGEX)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ArgumentNullException(nameof(source));

            if (string.IsNullOrWhiteSpace(regex))
                throw new ArgumentNullException(nameof(regex));

            return Regex.Matches(source, regex)
                .Cast<Match>()
                .Select(s => s.Value)
                .Where(w => Uri.TryCreate(string.Format("http://{0}", w), UriKind.Absolute, out var uri) && IPAddress.TryParse(uri.Host, out _))
                .ToHashSet();
        }

    }
}
