﻿using Byndyusoft.AspNetCore.Mvc.Formatters.Models;
using Byndyusoft.AspNetCore.Mvc.Formatters.Unit.Types;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Unit
{
    public class MessagePackOutputFormatterTests
    {
        private readonly MessagePackOutputFormatter _formatter;
        private readonly MessagePackSerializerOptions _options;

        public MessagePackOutputFormatterTests()
        {
            var options = new MvcMessagePackOptions();
            _options = options.SerializerOptions;
            _formatter = new MessagePackOutputFormatter(options);
        }

        [Fact]
        public void SupportedMediaTypes_From_Options()
        {
            // Arrange
            var options = new MvcMessagePackOptions();
            options.SupportedMediaTypes.Clear();
            options.SupportedMediaTypes.Add("media-type");

            // Act
            var formatter = new MessagePackOutputFormatter(options);

            // Assert
            var mediaType = Assert.Single(formatter.SupportedMediaTypes);
            Assert.Equal("media-type", mediaType);
        }

        [Theory]
        [InlineData(null, true)]
        [InlineData(typeof(int), true)]
        [InlineData(typeof(Class), true)]
        [InlineData(typeof(Struct), true)]
        [InlineData(typeof(Abstract), false)]
        [InlineData(typeof(IInterface), false)]
        public void CanWriteResult(Type type, bool expected)
        {
            // Arrange
            var context = CreateContext(type, null);

            // Act
            var result = _formatter.CanWriteResult(context);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public async Task WriteResponseBodyAsync_NullContext_ThrowsException()
        {
            // Act
            var exception =
                await Assert.ThrowsAsync<ArgumentNullException>(() => _formatter.WriteResponseBodyAsync(null!));

            // Assert
            Assert.Equal("context", exception.ParamName);
        }

        [Fact]
        public async Task WriteResponseBodyAsync_WritesNullModel()
        {
            // Arrange
            var context = CreateContext(typeof(object), null);

            // Act
            await _formatter.WriteResponseBodyAsync(context);

            // Assert
            var model = ReadModel(typeof(object), context);
            Assert.Null(model);
        }

        [Fact]
        public async Task WriteResponseBodyAsync_WritesPrimitiveType()
        {
            // Arrange
            var context = CreateContext(typeof(int), 10);

            // Act
            await _formatter.WriteResponseBodyAsync(context);

            // Assert
            var model = ReadModel(typeof(int), context);
            Assert.Equal(10, model);
        }

        [Fact]
        public async Task WriteResponseBodyAsync_WritesSimpleType()
        {
            // Arrange
            var input = SimpleModel.Create();
            var context = CreateContext(typeof(SimpleModel), input);

            // Act
            await _formatter.WriteResponseBodyAsync(context);

            // Assert
            var model = Assert.IsType<SimpleModel>(ReadModel(typeof(SimpleModel), context));
            model.Verify();
        }

        private object? ReadModel(Type modelType, OutputFormatterWriteContext context)
        {
            if (context.HttpContext.Response.Body.Length == 0)
                return null;

            context.HttpContext.Response.Body.Position = 0;
            return MessagePackSerializer.Deserialize(modelType, context.HttpContext.Response.Body, _options);
        }

        private OutputFormatterWriteContext CreateContext(Type? modelType, object? model)
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();

            static StreamWriter WriterFactory(Stream stream, Encoding encoding)
            {
                return new StreamWriter(stream, encoding);
            }

            return new OutputFormatterWriteContext(httpContext, WriterFactory, modelType, model);
        }
    }
}