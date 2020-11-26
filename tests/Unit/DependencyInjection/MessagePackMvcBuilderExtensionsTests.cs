using System;
using System.Net.Http.MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Unit.DependencyInjection
{
    public class MessagePackMvcBuilderExtensionsTests
    {
        private readonly IServiceCollection _serviceCollection;
        private readonly IMvcBuilder _mvcBuilder;
        private readonly IMvcCoreBuilder _mvcCoreBuilder;

        public MessagePackMvcBuilderExtensionsTests()
        {
            _serviceCollection = new ServiceCollection().AddLogging();
            _mvcBuilder = _serviceCollection.AddMvc();
            _mvcCoreBuilder = _serviceCollection.AddMvcCore();
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcBuilder_NullBuilder_ThrowsException()
        {
            // Arrange
            var builder = null as IMvcBuilder;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var exception = Assert.Throws<ArgumentNullException>(() => builder.AddMessagePackFormatters());

            // Assert
            Assert.Equal("builder", exception.ParamName);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcBuilder()
        {
            // Act
            _mvcBuilder.AddMessagePackFormatters();

            // Assert
            var mvcOptions = _serviceCollection.BuildServiceProvider().GetService<IOptions<MvcOptions>>().Value;
            Assert.Single(mvcOptions.InputFormatters, x => x.GetType() == typeof(MessagePackInputFormatter));
            Assert.Single(mvcOptions.OutputFormatters, x => x.GetType() == typeof(MessagePackOutputFormatter));

            var mapping = mvcOptions.FormatterMappings.GetMediaTypeMappingForFormat(MessagePackDefaults.MediaTypeFormat);
            Assert.NotNull(mapping);
            Assert.Equal(MessagePackDefaults.MediaType, mapping);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcBuilder_Options()
        {
            // Act
            _mvcBuilder.AddMessagePackFormatters(msgpack =>
            {
                msgpack.SupportedMediaTypes.Clear();
                msgpack.SupportedMediaTypes.Add("application/mediatype");
                msgpack.MediaTypeFormat = "format";
            });
            var mvcOptions = _serviceCollection.BuildServiceProvider().GetService<IOptions<MvcOptions>>().Value;

            // Assert
            var inputFormatter = (MessagePackInputFormatter)Assert.Single(mvcOptions.InputFormatters, x => x.GetType() == typeof(MessagePackInputFormatter));
            Assert.Equal(inputFormatter!.SupportedMediaTypes, new[] { "application/mediatype" });

            var outputFormatter = (MessagePackOutputFormatter)Assert.Single(mvcOptions.OutputFormatters, x => x.GetType() == typeof(MessagePackOutputFormatter));
            Assert.Equal(outputFormatter!.SupportedMediaTypes, new[] { "application/mediatype" });

            var mapping = mvcOptions.FormatterMappings.GetMediaTypeMappingForFormat("format");
            Assert.NotNull(mapping);
            Assert.Equal("application/mediatype", mapping);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcBuilder_Options_NullBuilder_ThrowsException()
        {
            // Arrange
            var builder = null as IMvcBuilder;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var exception = Assert.Throws<ArgumentNullException>(() => builder.AddMessagePackFormatters(options => { }));

            // Assert
            Assert.Equal("builder", exception.ParamName);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcBuilder_Options_NullOptions_ThrowsException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => _mvcBuilder.AddMessagePackFormatters(null));

            // Assert
            Assert.Equal("setupAction", exception.ParamName);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcCoreBuilder_NullBuilder_ThrowsException()
        {
            // Arrange
            var builder = null as IMvcCoreBuilder;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var exception = Assert.Throws<ArgumentNullException>(() => builder.AddMessagePackFormatters());

            // Assert
            Assert.Equal("builder", exception.ParamName);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcCoreBuilder()
        {
            // Act
            _mvcCoreBuilder.AddMessagePackFormatters();

            // Assert
            var mvcOptions = _serviceCollection.BuildServiceProvider().GetService<IOptions<MvcOptions>>().Value;
            Assert.Single(mvcOptions.InputFormatters, x => x.GetType() == typeof(MessagePackInputFormatter));
            Assert.Single(mvcOptions.OutputFormatters, x => x.GetType() == typeof(MessagePackOutputFormatter));

            var mapping = mvcOptions.FormatterMappings.GetMediaTypeMappingForFormat(MessagePackDefaults.MediaTypeFormat);
            Assert.NotNull(mapping);
            Assert.Equal(MessagePackDefaults.MediaType, mapping);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcCoreBuilder_Options()
        {
            // Act
            _mvcCoreBuilder.AddMessagePackFormatters(msgpack =>
            {
                msgpack.SupportedMediaTypes.Clear();
                msgpack.SupportedMediaTypes.Add("application/mediatype");
                msgpack.MediaTypeFormat = "format";
            });
            var mvcOptions = _serviceCollection.BuildServiceProvider().GetService<IOptions<MvcOptions>>().Value;

            // Assert
            var inputFormatter = (MessagePackInputFormatter)Assert.Single(mvcOptions.InputFormatters, x => x.GetType() == typeof(MessagePackInputFormatter));
            Assert.Equal(inputFormatter!.SupportedMediaTypes, new[] { "application/mediatype" });

            var outputFormatter = (MessagePackOutputFormatter)Assert.Single(mvcOptions.OutputFormatters, x => x.GetType() == typeof(MessagePackOutputFormatter));
            Assert.Equal(outputFormatter!.SupportedMediaTypes, new[] { "application/mediatype" });

            var mapping = mvcOptions.FormatterMappings.GetMediaTypeMappingForFormat("format");
            Assert.NotNull(mapping);
            Assert.Equal("application/mediatype", mapping);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcCoreBuilder_Options_NullBuilder_ThrowsException()
        {
            // Arrange
            var builder = null as IMvcCoreBuilder;

            // Act
            // ReSharper disable once ExpressionIsAlwaysNull
            var exception = Assert.Throws<ArgumentNullException>(() => builder.AddMessagePackFormatters(options => { }));

            // Assert
            Assert.Equal("builder", exception.ParamName);
        }

        [Fact]
        public void AddMessagePackFormatters_IMvcCoreBuilder_Options_NullOptions_ThrowsException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => _mvcCoreBuilder.AddMessagePackFormatters(null));

            // Assert
            Assert.Equal("setupAction", exception.ParamName);
        }
    }
}