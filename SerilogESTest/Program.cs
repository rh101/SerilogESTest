using Elastic.Channels;
using Elastic.Ingest.Elasticsearch;
using Elastic.Serilog.Sinks;
using Elastic.Transport;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

const string template = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}][{RequestId}] {Message:lj}{NewLine}{Exception}";
var elasticSearchUrl = builder.Configuration["ElasticSearch:Uri"] ?? "";
var elasticSearchUsername = builder.Configuration["ElasticSearch:Username"] ?? "";
var elasticSearchPassword = builder.Configuration["ElasticSearch:Password"] ?? "";

// Add services to the container.
builder.Services.AddSerilog(x => x.MinimumLevel.Verbose()
    .Enrich.FromLogContext()
    .WriteTo.Elasticsearch(new[] { new Uri(elasticSearchUrl) }, opts =>
    {
        opts.BootstrapMethod = BootstrapMethod.Failure;
    }, transport =>
    {
        transport.Authentication(new BasicAuthentication(elasticSearchUsername, elasticSearchPassword)); // Basic Auth
    })
    .WriteTo.Console(outputTemplate: template)
    .ReadFrom.Configuration(builder.Configuration)
);


builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
