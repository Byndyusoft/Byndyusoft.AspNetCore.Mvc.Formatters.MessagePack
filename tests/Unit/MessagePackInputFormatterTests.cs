using System;
using System.IO;
using System.Net.Http.MessagePack;
using System.Text;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.Formatters.Models;
using Byndyusoft.AspNetCore.Mvc.Formatters.Unit.Types;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Unit
{
    public class MessagePackInputFormatterTests
    {
        private readonly MessagePackInputFormatter _formatter;
        private readonly NullLogger<MessagePackInputFormatter> _logger;
        private readonly MessagePackSerializerOptions _options;

        public MessagePackInputFormatterTests()
        {
            _logger = new NullLogger<MessagePackInputFormatter>();
            var options = new MvcMessagePackOptions();
            _options = options.SerializerOptions;
            _formatter = new MessagePackInputFormatter(options, _logger);
        }

        [Fact]
        public void SupportedMediaTypes_From_Options()
        {
            // Arrange
            var options = new MvcMessagePackOptions();
            options.SupportedMediaTypes.Clear();
            options.SupportedMediaTypes.Add("media-type");

            // Act
            var formatter = new MessagePackInputFormatter(options, _logger);

            // Assert
            var mediaType = Assert.Single(formatter.SupportedMediaTypes);
            Assert.Equal("media-type", mediaType);
        }

        [Theory]
        [InlineData(typeof(int), true)]
        [InlineData(typeof(Class), true)]
        [InlineData(typeof(Struct), true)]
        [InlineData(typeof(Abstract), false)]
        [InlineData(typeof(IInterface), false)]
        public void CanRead(Type type, bool expected)
        {
            // Arrange
            var context = CreateContext(type);

            // Act
            var result = _formatter.CanRead(context);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_NullContext_ThrowsException()
        {
            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentNullException>(() => _formatter.ReadRequestBodyAsync(null!));

            // Assert
            Assert.Equal("context", exception.ParamName);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task ReadRequestBodyAsync_EmptyInput(bool treatEmptyInputAsDefaultValue)
        {
            // Arrange
            var context = CreateContext(typeof(SimpleModel), null, treatEmptyInputAsDefaultValue);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(context);

            // Assert
            Assert.False(result.HasError);
            Assert.Null(result.Model);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_ReadsPrimitiveType()
        {
            // Arrange
            var model = 10;
            var context = CreateContext(model.GetType(), model);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(context);

            // Assert
            Assert.False(result.HasError);
            Assert.Equal(model, result.Model);
        }

        [Fact]
        public async Task ReadRequestBodyAsync_ReadsSimpleType()
        {
            // Arrange
            var input = SimpleModel.Create();
            var context = CreateContext(input.GetType(), input);

            // Act
            var result = await _formatter.ReadRequestBodyAsync(context);

            // Assert
            Assert.False(result.HasError);
            var model = Assert.IsType<SimpleModel>(result.Model);
            model.Verify();
        }

        private InputFormatterContext CreateContext(Type modelType, object model = null,
            bool treatEmptyInputAsDefaultValue = false)
        {
            var memoryStream = new MemoryStream();
            if (model != null) MessagePackSerializer.Serialize(modelType, memoryStream, model, _options);

            memoryStream.Position = 0;

            var httpContext = new DefaultHttpContext();
            httpContext.Request.ContentType = MessagePackDefaults.MediaType;
            httpContext.Request.Body = memoryStream;

            var modelStateDictionary = new ModelStateDictionary();
            var modelMetadata = new Mock<ModelMetadata>(ModelMetadataIdentity.ForType(modelType)).Object;

            static TextReader ReaderFactory(Stream stream, Encoding encoding)
            {
                return new StreamReader(stream, encoding);
            }

            return new InputFormatterContext(httpContext, modelType.Name, modelStateDictionary, modelMetadata,
                ReaderFactory, treatEmptyInputAsDefaultValue);
        }
    }
}