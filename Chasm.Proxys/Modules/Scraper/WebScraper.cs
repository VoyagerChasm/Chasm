using Chasm.Proxys.Data;
using Chasm.Proxys.Modules.Parser;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Chasm.Proxys.Modules.Scraper
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
                    throw new ArgumentOutOfRangeException(nameof(value), "Timeout was out of range. Must be non-negative and more than zero.");
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
                    throw new ArgumentException("Regex must nor be null or empty", nameof(Regex));
                _regex = value;
            }
        }

        protected override void InitGlobalParameters()
        {
            base.InitGlobalParameters();
            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(Timeout)
            };
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
