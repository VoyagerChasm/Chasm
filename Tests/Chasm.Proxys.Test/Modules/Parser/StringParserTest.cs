using Chasm.Proxys.Data;
using Chasm.Proxys.Modules.Parsers;
using System;
using System.Collections.Generic;
using Xunit;

namespace Chasm.Proxys.Test.Modules.Parser
{
    public class StringParserTest
    {

        [Theory]
        [InlineData(null, null)]
        [InlineData(" ", " ")]
        [InlineData("", "")]
        [InlineData("", "./")]
        [InlineData("./", "")]
        [InlineData(" ", "./")]
        [InlineData("./", " ")]
        [InlineData(null, "./")]
        [InlineData("./", null)]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        public void TestParseThrowsArgumentNullException(string source, string regex = Defaults.PROXY_PARSER_REGEX)
        {
            StringParser _stringParser = new StringParser();
            Assert.Throws<ArgumentNullException>(() => _stringParser.Parse(source, regex));
        }

        [Theory]
        [InlineData("192.168.15.1:2000")]
        [InlineData("192.168.15.252:5222")]
        public void TestValidParse(string source, string regex = Defaults.PROXY_PARSER_REGEX)
        {
            StringParser _stringParser = new StringParser();
            HashSet<string> parser = _stringParser.Parse(source, regex);
            Assert.Single(parser);
        }

        [Theory]
        [InlineData("192.168.15.256:5222")]
        [InlineData("192.168.15.1:")]
        public void TestNotValidParse(string source, string regex = Defaults.PROXY_PARSER_REGEX)
        {
            StringParser _stringParser = new StringParser();
            HashSet<string> parser = _stringParser.Parse(source, regex);
            Assert.Empty(parser);
        }

    }
}
