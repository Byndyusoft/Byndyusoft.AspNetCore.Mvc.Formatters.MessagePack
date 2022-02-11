using MessagePack;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Net.Http.MessagePack;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.Formatters
{
    /// <summary>
    ///     A <see cref="InputFormatter" /> for MessagePack content that uses <see cref="MessagePackSerializer" />.
    /// </summary>
    public class MessagePackInputFormatter : InputFormatter, IInputFormatterExceptionPolicy
    {
        //private readonly MessagePackMediaTypeFormatter
        private readonly ILogger<MessagePackInputFormatter> _logger;

        /// <summary>
        ///     Initializes a new instance of <see cref="MessagePackInputFormatter" />.
        /// </summary>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        /// <param name="options">The <see cref="MvcMessagePackOptions" />.</param>
        public MessagePackInputFormatter(MvcMessagePackOptions options, ILogger<MessagePackInputFormatter> logger)
        {
            Guard.NotNull(options, nameof(options));

            _logger = Guard.NotNull(logger, nameof(logger));

            SerializerOptions = options.SerializerOptions;

            foreach (var mediaType in options.SupportedMediaTypes) SupportedMediaTypes.Add(mediaType);
        }

        /// <summary>
        ///     Gets the <see cref="MessagePackSerializerOptions" /> used to configure the <see cref="MessagePackSerializer" />.
        /// </summary>
        /// <remarks>
        ///     A single instance of <see cref="MessagePackInputFormatter" /> is used for all MessagePack formatting. Any
        ///     changes to the options will affect all input formatting.
        /// </remarks>
        public MessagePackSerializerOptions SerializerOptions { get; }

        /// <inheritdoc />
        InputFormatterExceptionPolicy IInputFormatterExceptionPolicy.ExceptionPolicy =>
            InputFormatterExceptionPolicy.MalformedInputExceptions;

        /// <inheritdoc />
        protected override bool CanReadType(Type? type)
        {
            return type == null || base.CanReadType(type) && MessagePackContent.CanSerialize(type);
        }

        /// <inheritdoc />
        public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context)
        {
            Guard.NotNull(context, nameof(context));

            object? model;

            try
            {
                using var content = new StreamContent(context.HttpContext.Request.Body);
                model = await content.ReadFromMessagePackAsync(context.ModelType, SerializerOptions)
                    .ConfigureAwait(false);
            }
            catch (MessagePackSerializationException exception)
            {
                Log.MessagePackInputException(_logger, exception);
                context.ModelState.TryAddModelError(string.Empty, exception, context.Metadata);
                return await InputFormatterResult.FailureAsync();
            }

            if (model == null && context.TreatEmptyInputAsDefaultValue == false)
                return await InputFormatterResult.NoValueAsync();

            Log.MessagePackInputSuccess(_logger, context.ModelType);
            return await InputFormatterResult.SuccessAsync(model);
        }

        private static class Log
        {
            // ReSharper disable InconsistentNaming
            private static readonly Action<ILogger, string, Exception> _msgpackInputFormatterException;

            private static readonly Action<ILogger, string, Exception?> _msgpackInputSuccess;
            // ReSharper enable InconsistentNaming

            static Log()
            {
                _msgpackInputFormatterException = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(1, "MessagePackInputException"),
                    "MessagePack input formatter threw an exception: {Message}");
                _msgpackInputSuccess = LoggerMessage.Define<string>(
                    LogLevel.Debug,
                    new EventId(2, "MessagePackInputSuccess"),
                    "MessagePack input formatter succeeded, deserializing to type '{TypeName}'");
            }

            public static void MessagePackInputException(ILogger logger, Exception exception)
            {
                _msgpackInputFormatterException(logger, exception.Message, exception);
            }

            public static void MessagePackInputSuccess(ILogger logger, Type modelType)
            {
                _msgpackInputSuccess(logger, modelType.FullName!, null);
            }
        }
    }
}