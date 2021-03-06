﻿using System;
using System.IO;
using System.Net.Http.MessagePack;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.Formatters.Models;
using MessagePack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Xunit;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Unit
{
    public class MessagePackResultTests
    {
        private readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard;

        [Fact]
        public void Constructor_WithValue()
        {
            // Arrange
            var model = 10;

            // Act
            var result = new MessagePackResult(model);

            // Assert
            Assert.Equal(model, result.Value);
            Assert.Equal(MessagePackDefaults.MediaType, result.ContentType);
        }

        [Fact]
        public void Constructor_WithValueAndOptions()
        {
            // Arrange
            var model = 10;

            // Act
            var result = new MessagePackResult(model, _options);

            // Assert
            Assert.Equal(model, result.Value);
            Assert.Equal(_options, result.SerializerOptions);
            Assert.Equal(MessagePackDefaults.MediaType, result.ContentType);
        }

        [Fact]
        public void Constructor_NullOptions_ThrowsException()
        {
            // Act
            var exception = Assert.ThrowsAny<ArgumentNullException>(() => new MessagePackResult(10, null));

            // Assert
            Assert.Equal("serializerOptions", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteResultAsync_NullContext_ThrowsException()
        {
            // Arrange
            var result = new MessagePackResult(null);

            // Act
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => result.ExecuteResultAsync(null));

            // Assert
            Assert.Equal("context", exception.ParamName);
        }

        [Fact]
        public async Task ExecuteResultAsync_NullValue()
        {
            // Arrange
            var context = CreateContext();
            var result = new MessagePackResult(null);

            // Act
            await result.ExecuteResultAsync(context);

            // Assert
            var model = ReadModel<object>(context);
            Assert.Null(model);
        }

        [Fact]
        public async Task ExecuteResultAsync_ContentType()
        {
            // Arrange
            var context = CreateContext();
            var result = new MessagePackResult(null);

            // Act
            await result.ExecuteResultAsync(context);

            // Assert
            Assert.Equal(MessagePackDefaults.MediaType, context.HttpContext.Response.ContentType);
        }

        [Fact]
        public async Task ExecuteResultAsync_PrimitiveValue()
        {
            // Arrange
            var context = CreateContext();
            var result = new MessagePackResult(10);

            // Act
            await result.ExecuteResultAsync(context);

            // Assert
            var model = ReadModel<int>(context);
            Assert.Equal(10, model);
        }

        [Fact]
        public async Task ExecuteResultAsync_SimpleTypeValue()
        {
            // Arrange
            var simpleType = SimpleModel.Create();
            var context = CreateContext();
            var result = new MessagePackResult(simpleType);

            // Act
            await result.ExecuteResultAsync(context);

            // Assert
            var model = ReadModel<SimpleModel>(context);
            Assert.NotNull(model);
            model.Verify();
        }

        [Fact]
        public void StatusCode_SerializerOptions()
        {
            // Arrange
            var result = new MessagePackResult(null);

            // Act
            result.SerializerOptions = _options;

            // Assert
            Assert.Same(_options, result.SerializerOptions);
        }

        [Fact]
        public void StatusCode_Property()
        {
            // Arrange
            var result = new MessagePackResult(null);

            // Act
            result.StatusCode = 200;

            // Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void Value_Property()
        {
            // Arrange
            var result = new MessagePackResult(null);

            // Act
            result.Value = 10;

            // Assert
            Assert.Equal(10, result.Value);
        }

        [Fact]
        public void ContentType_Property()
        {
            // Arrange
            var result = new MessagePackResult(null);

            // Act
            result.ContentType = "content/type";

            // Assert
            Assert.Equal("content/type", result.ContentType);
        }

        private T ReadModel<T>(ActionContext context)
        {
            if (context.HttpContext.Response.Body.Length == 0)
                return default;

            context.HttpContext.Response.Body.Position = 0;
            return MessagePackSerializer.Deserialize<T>(context.HttpContext.Response.Body, _options);
        }

        private ActionContext CreateContext()
        {
            var httpContext = new DefaultHttpContext();
            httpContext.Response.Body = new MemoryStream();
            return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        }
    }
}