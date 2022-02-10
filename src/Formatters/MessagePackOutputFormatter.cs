using System;
using System.Net.Http.MessagePack;
using System.Threading.Tasks;
using MessagePack;

namespace Microsoft.AspNetCore.Mvc.Formatters
{
    /// <summary>
    ///     A <see cref="OutputFormatter" /> for MessagePack content that uses <see cref="MessagePackSerializer" />.
    /// </summary>
    public class MessagePackOutputFormatter : OutputFormatter
    {
        /// <summary>
        ///     Initializes a new <see cref="MessagePackOutputFormatter" /> instance.
        /// </summary>
        /// <param name="options">The <see cref="MvcMessagePackOptions" />.</param>
        public MessagePackOutputFormatter(MvcMessagePackOptions options)
        {
            Guard.NotNull(options, nameof(options));

            SerializerOptions = options.SerializerOptions;

            foreach (var mediaType in options.SupportedMediaTypes) SupportedMediaTypes.Add(mediaType);
        }

        internal MessagePackOutputFormatter(MessagePackSerializerOptions serializerOptions)
        {
            SupportedMediaTypes.Add(MessagePackDefaults.MediaTypes.ApplicationXMessagePack);
            SupportedMediaTypes.Add(MessagePackDefaults.MediaTypes.ApplicationMessagePack);

            SerializerOptions = Guard.NotNull(serializerOptions, nameof(serializerOptions));
        }


        /// <summary>
        ///     Gets the <see cref="MessagePackSerializerOptions" /> used to configure the <see cref="MessagePackSerializer" />.
        /// </summary>
        /// <remarks>
        ///     A single instance of <see cref="MessagePackOutputFormatter" /> is used for all MessagePack formatting. Any
        ///     changes to the options will affect all output formatting.
        /// </remarks>
        public MessagePackSerializerOptions SerializerOptions { get; }

        /// <inheritdoc />
        protected override bool CanWriteType(Type? type)
        {
            return type == null || base.CanWriteType(type) && MessagePackContent.CanSerialize(type);
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            Guard.NotNull(context, nameof(context));

            await using var content = MessagePackContent.Create(context.ObjectType ?? typeof(object), context.Object, SerializerOptions);
            await content.CopyToAsync(context.HttpContext.Response.Body).ConfigureAwait(false);
        }
    }
}