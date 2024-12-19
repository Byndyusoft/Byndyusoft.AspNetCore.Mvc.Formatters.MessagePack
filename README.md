# Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack
ASP.NET Core MVC formatters for MessagePack input and output.

[![(License)](https://img.shields.io/github/license/Byndyusoft/Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.svg)](LICENSE.txt)
[![Nuget](http://img.shields.io/nuget/v/Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.svg?maxAge=10800)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack/) [![NuGet downloads](https://img.shields.io/nuget/dt/Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack.svg)](https://www.nuget.org/packages/Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack/) 

[MessagePack](https://www.nuget.org/packages/MessagePack/) is an efficient binary serialization format. It lets you exchange data among multiple languages like JSON. But it's faster and smaller. 
Small integers are encoded into a single byte, and typical short strings require only one extra byte in addition to the strings themselves.

## Content negotiation
Content negotiation occurs when the client specifies an `Accept` header. The default format used by ASP.NET Core is JSON. But client can specify `MessagePack` format:
```csharp
using (var httpClient = new HttpClient())
{
	client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/msgpack"));
	var response = await client.GetAsync("/api/products/5");
	var product = response.Content.ReadFromMessagePackAsync<Product>();
}
```
In this example [Byndyusoft.Net.Http.MessagePack](https://www.nuget.org/packages/Byndyusoft.Net.Http.MessagePack/) package is used.


## Configure formatters
Asp.NET Core Apps that need to support MessagePack format can add the `Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack` NuGet packages and configure support. 
There are separate formatters for input and output. Input formatters are used by [Model Binding](https://docs.microsoft.com/en-us/aspnet/core/mvc/models/model-binding). Output formatters are used to format responses. 

## Installing

```shell
dotnet add package Byndyusoft.AspNetCore.Mvc.Formatters.MessagePack
```

## Add MessagePack format support to Asp.Net
MessagePack formatters implemented using `MessagePackSerializer` are configured by calling `AddMessagePackFormatters`:

```csharp
public void ConfigureServices(IServiceCollection services)
{
     services.AddMvcCore()
        .AddMessagePackFormatters();
}
```
The preceding code serializes results using `MessagePackSerializer`.

## Configure MessagePack formatters

Features for the MessagePack formatters can be configured using `Microsoft.AspNetCore.Mvc.MvcMessagePackOptions.SerializerOptions`.

```csharp
services.AddMvcCore()
	.AddMessagePackFormatters(
		options =>
		{
			options.SerializerOptions
				.WithResolver(TypelessContractlessStandardResolver.Instance)
				.WithCompression(MessagePackCompression.Lz4Block)
				.WithSecurity(MessagePackSecurity.TrustedData);
		});
```


## Response format URL mappings
Clients can request a particular format as part of the URL, for example:
* In the query string or part of the path.
* By using a format-specific file extension such as .json or .msgpack.

The mapping from request path should be specified in the route the API is using. For example:
```csharp
[Route("api/[controller]")]
[ApiController]
[FormatFilter]
public class ProductsController : ControllerBase
{
    [HttpGet("{id}.{format?}")]
    public Product Get(int id)
    {
```
The preceding route allows the requested format to be specified as an optional file extension. 
The `[FormatFilter]` attribute checks for the existence of the format value in the RouteData and maps the response format to the appropriate formatter when the response is created.

| Route| Formatter |
| ---- | --------- |
| /api/products/5          |	The default output formatter |
| /api/products/5.json     |	The JSON formatter (if configured) |
| /api/products/5.msgpack  |	The MessagePack formatter (if configured) |


# Contributing

To contribute, you will need to setup your local environment, see [prerequisites](#prerequisites). For the contribution and workflow guide, see [package development lifecycle](#package-development-lifecycle).

A detailed overview on how to contribute can be found in the [contributing guide](CONTRIBUTING.md).

## Prerequisites

Make sure you have installed all of the following prerequisites on your development machine:

- Git - [Download & Install Git](https://git-scm.com/downloads). OSX and Linux machines typically have this already installed.
- .NET (version 8.0 or higher) - [Download & Install .NET Core](https://dotnet.microsoft.com/download/dotnet/8.0).

## General folders layout

### src
- source code

### tests

- unit-tests

## Package development lifecycle

- Implement package logic in `src`
- Add or addapt unit-tests (prefer before and simultaneously with coding) in `tests`
- Add or change the documentation as needed
- Open pull request in the correct branch. Target the project's `master` branch

# Maintainers

[github.maintain@byndyusoft.com](mailto:github.maintain@byndyusoft.com)
