using Asp.Versioning;
using Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services
    .AddApiVersioning(
        options =>
        {
            options.DefaultApiVersion = ApiVersion.Default;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }
    )
    .AddApiExplorer(
        options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        }
    );

services.AddSwagger();

services
    .AddMvcCore()
    .AddMessagePackFormatters()
    .AddFormatterMappings();
services.AddControllers();

var app = builder.Build();
if (app.Environment.IsProduction() == false)
    app.UseSwaggerWithApiVersionDescriptionProvider();

app.MapControllers();
app.UseRouting();
app.Run();