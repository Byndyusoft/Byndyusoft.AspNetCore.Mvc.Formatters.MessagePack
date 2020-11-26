using System.Net.Http;
using System.Net.Http.MessagePack;
using System.Threading.Tasks;
using Byndyusoft.AspNetCore.Mvc.Formatters.Models;
using MessagePack;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Byndyusoft.AspNetCore.Mvc.Formatters.Functional
{
    public class MvcMessagePackTests : MvcTestFixture
    {
        private readonly MessagePackSerializerOptions _serializerOptions;

        public MvcMessagePackTests()
        {
            _serializerOptions = MessagePackDefaults.SerializerOptions;
        }

        protected override void ConfigureHttpClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Add(MessagePackDefaults.MediaTypeHeader);
        }

        protected override void ConfigureMvc(IMvcCoreBuilder builder)
        {
            builder.AddMessagePackFormatters(options => { options.SerializerOptions = _serializerOptions; });
        }

        [Fact]
        public async Task NullObject()
        {
            // Act
            var response =
                await Client.PostAsMessagePackAsync<SimpleModel>("/msgpack-formatter/echo", null, _serializerOptions);

            // Asert
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromMessagePackAsync<SimpleModel>(_serializerOptions);
            Assert.Null(model);
        }

        [Fact]
        public async Task PrimitiveType()
        {
            // Act
            var response = await Client.PostAsMessagePackAsync("/msgpack-formatter/echo", 10, _serializerOptions);

            // Asert
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromMessagePackAsync<int>(_serializerOptions);
            Assert.Equal(10, model);
        }

        [Fact]
        public async Task SimpleType()
        {
            // Arrange
            var simpleType = SimpleModel.Create();

            // Act
            var response =
                await Client.PostAsMessagePackAsync("/msgpack-formatter/echo", simpleType, _serializerOptions);

            // Asert
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromMessagePackAsync<SimpleModel>(_serializerOptions);
            Assert.NotNull(model);
            model.Verify();
        }

        [Fact]
        public async Task MediaTypeFormat()
        {
            // Arrange
            var simpleType = SimpleModel.Create();
            var content = MessagePackContent.Create(simpleType, _serializerOptions);

            // Act
            Client.DefaultRequestHeaders.Accept.Clear();
            var response = await Client.PostAsync("/msgpack-formatter/echo?format=msgpack", content);

            // Asert
            response.EnsureSuccessStatusCode();
            var model = await response.Content.ReadFromMessagePackAsync<SimpleModel>(_serializerOptions);
            Assert.NotNull(model);
            model.Verify();
        }
    }
}