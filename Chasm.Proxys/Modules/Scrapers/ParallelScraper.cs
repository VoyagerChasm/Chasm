using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Chasm.Proxys.Modules.Scrapers
{
    public abstract class ParallelScraper<T> : IScraper<ICollection<T>>
    {
        public delegate void OnSourceParsedCallback(ICollection<T> proxys, T source);
        public delegate void OnErrorScrapingCallback(Exception exception);

        public event OnSourceParsedCallback OnSourceParsedEventHandler;
        public event OnErrorScrapingCallback OnErrorScrapingEventHandler;
        public event EventHandler OnStartScrapingEventHandler;
        public event EventHandler OnStopScrapingEventHandler;

        protected CancellationTokenSource _cancellationToken;

        protected ParallelScraper()
        {
            _cancellationToken = null;
        }

        protected virtual void OnStopScraping() => OnStopScrapingEventHandler?.Invoke(this, EventArgs.Empty);
        protected virtual void OnStartScraping() => OnStartScrapingEventHandler?.Invoke(this, EventArgs.Empty);

        protected virtual void OnErrorScraping(Exception exception) => OnErrorScrapingEventHandler?.Invoke(exception);
        protected virtual void OnSourceParsed(ICollection<T> proxys, T source) => OnSourceParsedEventHandler?.Invoke(proxys, source);


        public virtual HashSet<string> StartScrape(ICollection<T> source)
        {
            InitGlobalParameters();
            ThrowIfParametersIsNotValid(source);

            var proxy = new HashSet<string>();

            OnStartScraping();

            var parallelOptions = new ParallelOptions
            {
                CancellationToken = _cancellationToken.Token,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            Parallel.ForEach(source.Distinct(), parallelOptions, (path, state) =>
            {
                //Stop If Requested
                try
                {
                    _cancellationToken?.Token.ThrowIfCancellationRequested();
                }
                catch
                {
                    state.Stop();
                }

                //scrape
                proxy.UnionWith(Parse(path, state));

            });
            OnStopScraping();

            return proxy;
        }

        protected virtual void InitGlobalParameters()
        {
            _cancellationToken = new CancellationTokenSource();
        }

        protected virtual void ThrowIfParametersIsNotValid(ICollection<T> source)
        {
            if (source is null)
                throw new ArgumentNullException(nameof(source));

            if (source.Count < 1)
                throw new ArgumentOutOfRangeException(nameof(source));

            if (_cancellationToken is null)
                throw new ArgumentNullException(nameof(_cancellationToken));
        }

        protected abstract HashSet<string> Parse(T source, ParallelLoopState state);

        public virtual void StopScrape()
        {
            _cancellationToken?.Cancel();
        }

    }
}
