// ReSharper disable CheckNamespace

using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    ///     Extension methods for adding MessagePack formatters to MVC.
    /// </summary>
    public static class MessagePackMvcBuilderExtensions
    {
        /// <summary>
        ///     Adds the MessagePack formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" />.</param>
        /// <returns>The <see cref="IMvcBuilder" />.</returns>
        public static IMvcBuilder AddMessagePackFormatters(this IMvcBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            AddMessagePackFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        ///     Adds the MessagePack formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcBuilder" />.</param>
        /// <param name="setupAction">The <see cref="MvcMessagePackOptions" /> which need to be configured.</param>
        /// <returns>The <see cref="IMvcBuilder" />.</returns>
        public static IMvcBuilder AddMessagePackFormatters(this IMvcBuilder builder,
            Action<MvcMessagePackOptions> setupAction)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);

            return AddMessagePackFormatters(builder);
        }

        /// <summary>
        ///     Adds the MessagePack formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder" />.</param>
        /// <returns>The <see cref="IMvcCoreBuilder" />.</returns>
        public static IMvcCoreBuilder AddMessagePackFormatters(this IMvcCoreBuilder builder)
        {
            Guard.NotNull(builder, nameof(builder));

            AddMessagePackFormatterServices(builder.Services);
            return builder;
        }

        /// <summary>
        ///     Adds the MessagePack formatters to MVC.
        /// </summary>
        /// <param name="builder">The <see cref="IMvcCoreBuilder" />.</param>
        /// <param name="setupAction">The <see cref="MvcMessagePackOptions" /> which need to be configured.</param>
        /// <returns>The <see cref="IMvcCoreBuilder" />.</returns>
        public static IMvcCoreBuilder AddMessagePackFormatters(this IMvcCoreBuilder builder,
            Action<MvcMessagePackOptions> setupAction)
        {
            Guard.NotNull(builder, nameof(builder));
            Guard.NotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);

            return AddMessagePackFormatters(builder);
        }

        private static void AddMessagePackFormatterServices(IServiceCollection services)
        {
            services.TryAddEnumerable(
                ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, MvcMessagePackOptionsSetup>());
        }
    }
}