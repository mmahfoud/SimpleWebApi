using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// inject dependencies

// 1. Logging
builder.Services.AddLogging(b => b.AddConsole());

// 2. Sessions
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(b =>
{
    b.Cookie.Name = "secret-session-id";
    b.IOTimeout = TimeSpan.FromSeconds(15);
    b.IdleTimeout = TimeSpan.FromSeconds(10);
});

// Build the app
var app = builder.Build();

// use dependencies

// 1. Use Sessions
app.UseSession();

// 2. Map endpoints
app.MapMethods("/hello/{*name}", ["GET", "POST", "PUT"], async (string? name, HttpContext cx, [FromServices] ILogger<Program> logger, CancellationToken ct) =>
{
    if (cx.Session.Get("once") == null) cx.Session.Set("once", Encoding.UTF8.GetBytes(true.ToString()));

    logger.LogInformation("Headers\n\t=======\n\t{headers}", cx.Request.Headers.Aggregate("", (t, h) => t + h.Key + ": '" + h.Value + "'\n\t"));

    var query = cx.Request.Query;
    logger.LogInformation("Query\n\t=======\n\t{queries}", query.Aggregate("", (t, h) => t + h.Key + ": '" + h.Value + "'\n\t"));
    if (string.IsNullOrEmpty(name)) name = query.FirstOrDefault(x => x.Key == "name").Value;

    if (cx.Request.Headers["Content-Type"] == "application/x-www-form-urlencoded")
    {
        var form = await cx.Request.ReadFormAsync(ct);
        logger.LogInformation("Form\n\t=======\n\t{fields}", form.Aggregate("", (t, h) => t + h.Key + ": '" + h.Value + "'\n\t"));
        if (string.IsNullOrEmpty(name)) name = form.FirstOrDefault(x => x.Key == "name").Value;
    }

    if (!string.IsNullOrEmpty(name)) name = " " + name;
    return string.Format("Hello{0}!", name);
}).WithDisplayName("Hello").WithDescription("Say a greeting for a name.").WithName("greeting");

app.MapGet("/{op:alpha}/{left:int}/{right:int?}", (string op, int left, int? right, ILogger<Program> logger) =>
{
    logger.LogInformation("\t{operation}/{left}/{right}", left, op, right);
    return op.ToLower() switch
    {
        "add" => string.Format("{0} {1} {2} = {3}", left, "+", right ?? 0, left + (right ?? 0)),
        "multiply" => string.Format("{0} {1} {2} = {3}", left, "*", right ?? 1, left * (right ?? 1)),
        "mod" => string.Format("{0} {1} {2} = {3}", left, "%", right ?? 1, left % (right ?? 1)),
        "div" => string.Format("{0} {1} {2} = {3}", left, "/", right ?? 1, left / (right ?? 1)),
        _ => "Invalid operation! available operations are: add(+), multiply(*), mod(%), div(/)"
    };
}).WithDisplayName("Calculator").WithDescription("Do basic math.").WithName("math"); ;

app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
    endpointSources
    .SelectMany(es => es.Endpoints)
    .Where(e => e.Metadata.OfType<EndpointNameMetadata>().Any(x => !string.IsNullOrEmpty(x.EndpointName)))
    .Select(e => new
    {
        e.DisplayName,
        e.Metadata.OfType<IEndpointDescriptionMetadata>().FirstOrDefault()?.Description,
        Method = e.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods.Aggregate("", (a, b) => string.IsNullOrEmpty(a) ? b : a + ", " + b),
        Pattern = (e as RouteEndpoint)?.RoutePattern.RawText,
        Name = e.Metadata.OfType<EndpointNameMetadata>().FirstOrDefault()?.EndpointName,
    })
).WithDisplayName("The Help: this").WithDescription("Displays all endpoints available.").WithName("help");

app.MapGet("/{*allOthers}", (string allOthers) =>
{
    return Results.NotFound("Invalid url");
}).WithDisplayName("Invalid").WithDescription("Other paths").WithName("error");

// run the app
app.Run();