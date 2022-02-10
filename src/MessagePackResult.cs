using System.Net.Http.MessagePack;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    ///     An action result which formats the given object as MessagePack.
    /// </summary>
    public class MessagePackResult : ObjectResult
    {
        /// <summary>
        ///     Creates a new <see cref="MessagePackResult" /> with the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value to format as MessagePack.</param>
        public MessagePackResult(object? value)
            : this(value, MessagePackDefaults.SerializerOptions)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="MessagePackResult" /> with the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value to format as MessagePack.</param>
        /// <param name="serializerOptions">
        ///     The <see cref="MessagePackSerializerOptions" /> to be used by the formatter.
        /// </param>
        public MessagePackResult(object? value, MessagePackSerializerOptions serializerOptions)
            : base(value)
        {
            var formatter = new MessagePackOutputFormatter(serializerOptions);

            Formatters.Add(formatter);
            ContentTypes = formatter.SupportedMediaTypes;

            SerializerOptions = Guard.NotNull(serializerOptions, nameof(serializerOptions));
        }
        
        /// <summary>
        ///     Gets or sets the <see cref="MessagePackSerializerOptions" />.
        /// </summary>
        public MessagePackSerializerOptions SerializerOptions { get; set; }

        /// <inheritdoc />
        public override Task ExecuteResultAsync(ActionContext context)
        {
            Guard.NotNull(context, nameof(context));

            return base.ExecuteResultAsync(context);
        }
    }
}