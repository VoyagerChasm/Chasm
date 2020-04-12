using Chasm.Proxys.Data;
using Chasm.Proxys.Modules.Parsers;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chasm.Proxys.Modules.Scrapers
{
    public class WebScraper : ParallelScraper<string>
    {

        private readonly IParser<string> _parser;
        private HttpClient _client;

        public WebScraper(uint timeout = 60, string pattern = Defaults.PROXY_PARSER_REGEX)
        {
            Regex = pattern;
            Timeout = timeout;
            _parser = new StringParser();
        }


        private uint _timeout;
        public uint Timeout
        {
            get => _timeout;
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _timeout = value;
            }
        }

        private string _regex;
        public string Regex
        {
            get => _regex;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException(nameof(value));
                _regex = value;
            }
        }

        protected override void InitGlobalParameters()
        {
            base.InitGlobalParameters();
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(Timeout);
        }

        protected override HashSet<string> Parse(string url, ParallelLoopState state)
        {
            var proxy = new HashSet<string>();

            try
            {
                var taskResponse = Task.Run(() => _client.GetAsync(new Uri(url)));
                taskResponse.Wait();
                var response = taskResponse.Result;

                response.EnsureSuccessStatusCode();

                var taskBody = Task.Run(() => response.Content.ReadAsStringAsync());
                taskBody.Wait();
                var body = taskBody.Result;

                if (string.IsNullOrWhiteSpace(body))
                    return proxy;

                proxy = _parser.Parse(body, Regex);
            }
            catch (Exception ex)
            {
                OnErrorScraping(ex);
            }

            OnSourceParsed(new HashSet<string>(proxy), url);
            return proxy;
        }
    }
}
