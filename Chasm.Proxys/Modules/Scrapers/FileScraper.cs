﻿using Chasm.Proxys.Data;
using Chasm.Proxys.Modules.Parsers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Chasm.Proxys.Modules.Scrapers
{
    public class FileScraper : ParallelScraper<string>
    {
        private readonly IParser<string> _parser;

        public FileScraper(string pattern = Defaults.PROXY_PARSER_REGEX)
        {
            this.Regex = pattern;
            _parser = new StringParser();
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

        protected override HashSet<string> Parse(string path, ParallelLoopState state)
        {
            var proxy = new HashSet<string>();

            if (!File.Exists(path))
            {
                OnErrorScraping(new FileNotFoundException(nameof(path)));
                return proxy;
            }

            try
            {
                var body = File.ReadAllText(path);

                if (string.IsNullOrWhiteSpace(body))
                    return proxy;

                proxy = _parser.Parse(body, Regex);
            }
            catch (Exception ex)
            {
                OnErrorScraping(ex);
            }

            OnSourceParsed(new HashSet<string>(proxy), path);

            return proxy;
        }
    }
}
