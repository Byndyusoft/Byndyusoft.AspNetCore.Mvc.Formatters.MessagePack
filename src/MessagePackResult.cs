using System;
using System.Net.Http.MessagePack;
using System.Threading.Tasks;
using MessagePack;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Net.Http.Headers;

namespace Microsoft.AspNetCore.Mvc
{
    /// <summary>
    ///     An action result which formats the given object as MessagePack.
    /// </summary>
    public class MessagePackResult : ActionResult, IStatusCodeActionResult
    {
        /// <summary>
        ///     Creates a new <see cref="MessagePackResult" /> with the given <paramref name="value" />.
        /// </summary>
        /// <param name="value">The value to format as MessagePack.</param>
        public MessagePackResult(object value)
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
        public MessagePackResult(object value, MessagePackSerializerOptions serializerOptions)
        {
            Value = value;
            SerializerOptions = serializerOptions ?? throw new ArgumentNullException(nameof(serializerOptions));
        }

        /// <summary>
        ///     Gets or sets the <see cref="MediaTypeHeaderValue" /> representing the Content-Type header of the response.
        /// </summary>
        public string ContentType { get; set; } = MessagePackDefaults.MediaType;

        /// <summary>
        ///     Gets or sets the <see cref="MessagePackSerializerOptions" />.
        /// </summary>
        public MessagePackSerializerOptions SerializerOptions { get; set; }

        /// <summary>
        ///     Gets or sets the value to be formatted.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        ///     Gets or sets the HTTP status code.
        /// </summary>
        public int? StatusCode { get; set; }

        /// <inheritdoc />
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            context.HttpContext.Response.ContentType = ContentType;

            if (Value != null)
                await MessagePackSerializer
                    .SerializeAsync(Value.GetType(), context.HttpContext.Response.Body, Value, SerializerOptions)
                    .ConfigureAwait(false);
        }
    }
}