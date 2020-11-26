using System;
using System.Threading.Tasks;
using MessagePack;

namespace Microsoft.AspNetCore.Mvc.Formatters
{
    /// <summary>
    /// A <see cref="OutputFormatter"/> for MessagePack content that uses <see cref="MessagePackSerializer"/>.
    /// </summary>
    public class MessagePackOutputFormatter : OutputFormatter
    {
        /// <summary>
        /// Initializes a new <see cref="MessagePackOutputFormatter"/> instance.
        /// </summary>
        /// <param name="options">The <see cref="MvcMessagePackOptions"/>.</param>
        public MessagePackOutputFormatter(MvcMessagePackOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            SerializerOptions = options.SerializerOptions;

            foreach (var mediaType in options.SupportedMediaTypes) SupportedMediaTypes.Add(mediaType);
        }

        /// <summary>
        /// Gets the <see cref="MessagePackSerializerOptions"/> used to configure the <see cref="MessagePackSerializer"/>.
        /// </summary>
        /// <remarks>
        /// A single instance of <see cref="MessagePackOutputFormatter"/> is used for all MessagePack formatting. Any
        /// changes to the options will affect all output formatting.
        /// </remarks>
        public MessagePackSerializerOptions SerializerOptions { get; }

        /// <inheritdoc />
        protected override bool CanWriteType(Type type)
        {
            return base.CanWriteType(type) && !type.IsAbstract && !type.IsInterface;
        }

        /// <inheritdoc />
        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (context.Object == null) return;

            await MessagePackSerializer.SerializeAsync(context.HttpContext.Response.Body, context.Object, SerializerOptions)
                .ConfigureAwait(false);
        }
    }
}