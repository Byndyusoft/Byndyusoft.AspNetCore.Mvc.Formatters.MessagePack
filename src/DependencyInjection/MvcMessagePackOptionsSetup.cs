// ReSharper disable CheckNamespace

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     A <see cref="IConfigureOptions{TOptions}" /> implementation which will add the
    ///     MessagePack serializer formatters to <see cref="MvcOptions" />.
    /// </summary>
    internal class MvcMessagePackOptionsSetup : IConfigureOptions<MvcOptions>
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly MvcMessagePackOptions _options;

        /// <summary>
        ///     Initializes a new instance of <see cref="MessagePackInputFormatter" />.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory" />.</param>
        /// <param name="options">The <see cref="MvcMessagePackOptions" />.</param>
        public MvcMessagePackOptionsSetup(IOptions<MvcMessagePackOptions> options, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _options = options.Value;
        }

        /// <summary>
        ///     Adds the MessagePack serializer formatters to <see cref="MvcOptions" />.
        /// </summary>
        /// <param name="options">The <see cref="MvcOptions" />.</param>
        public void Configure(MvcOptions options)
        {
            ConfigureFormatters(options);
            ConfigureMediaTypeFormat(options);
        }

        private void ConfigureMediaTypeFormat(MvcOptions options)
        {
            var mapping = options.FormatterMappings.GetMediaTypeMappingForFormat(_options.MediaTypeFormat);
            if (!string.IsNullOrEmpty(mapping)) return;

            var mediaType = _options.SupportedMediaTypes.FirstOrDefault();
            if (mediaType == null) return;

            options.FormatterMappings.SetMediaTypeMappingForFormat(_options.MediaTypeFormat, mediaType);
        }

        private void ConfigureFormatters(MvcOptions options)
        {
            options.OutputFormatters.Add(new MessagePackOutputFormatter(_options));
            options.InputFormatters.Add(new MessagePackInputFormatter(_options,
                _loggerFactory.CreateLogger<MessagePackInputFormatter>()));
        }
    }
}