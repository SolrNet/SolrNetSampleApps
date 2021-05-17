using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SolrNet;

namespace SampleSolrApp.Helpers
{
    public class LoggingConnection: ISolrConnection
    {
        readonly ISolrConnection _connection;
        readonly ILogger<LoggingConnection> _logger;

        public LoggingConnection(ISolrConnection connection, ILogger<LoggingConnection> logger)
        {
            _connection = connection;
            _logger = logger;
        }

        void Log(string message, [CallerMemberName] string caller = null)
        {
            _logger.LogInformation($"{caller}: {message}");
        }

        public string Post(string relativeUrl, string s)
        {
            Log($"{nameof(relativeUrl)}={relativeUrl}, {nameof(s)}={s}");
            return _connection.Post(relativeUrl, s);
        }

        public async Task<string> PostAsync(string relativeUrl, string s)
        {
            Log($"{nameof(relativeUrl)}={relativeUrl}, {nameof(s)}={s}");
            return await _connection.PostAsync(relativeUrl, s);
        }

        public string PostStream(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            // TODO add other args
            Log($"{nameof(relativeUrl)}={relativeUrl}, {nameof(contentType)}={contentType}");
            return _connection.PostStream(relativeUrl, contentType, content, getParameters);
        }

        public async Task<string> PostStreamAsync(string relativeUrl, string contentType, Stream content, IEnumerable<KeyValuePair<string, string>> getParameters)
        {
            // TODO add other args
            Log($"{nameof(relativeUrl)}={relativeUrl}, {nameof(contentType)}={contentType}");
            return await _connection.PostStreamAsync(relativeUrl, contentType, content, getParameters);
        }

        public string Get(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters)
        {
            // TODO add other args
            Log($"{nameof(relativeUrl)}={relativeUrl}");
            return _connection.Get(relativeUrl, parameters);
        }

        public async Task<string> GetAsync(string relativeUrl, IEnumerable<KeyValuePair<string, string>> parameters,
            CancellationToken cancellationToken = new CancellationToken())
        {
            Log(
                $"{nameof(relativeUrl)}={relativeUrl}, " +
                $"{nameof(parameters)}={string.Join(", ", parameters.Select(p => $"{p.Key}={p.Value}"))}"
            );
            return await _connection.GetAsync(relativeUrl, parameters, cancellationToken);
        }
    }
}