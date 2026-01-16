using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// Resolve the actual wwwroot folder next to the EXE
var root = Path.Combine(AppContext.BaseDirectory, "wwwroot");
Console.WriteLine($"Serving static files from: {root}");

var provider = new FileExtensionContentTypeProvider();

// Ensure WASM loads correctly
provider.Mappings[".wasm"] = "application/wasm";

// ICU data files
provider.Mappings[".dat"] = "application/octet-stream";

// Precompressed assets
provider.Mappings[".br"] = "application/octet-stream";
provider.Mappings[".gz"] = "application/octet-stream";

// Allow unknown file types (Blazor uses many)
var staticOptions = new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(root),
    ContentTypeProvider = provider,
    ServeUnknownFileTypes = true
};

// Enable index.html as default
app.UseDefaultFiles(new DefaultFilesOptions
{
    FileProvider = new PhysicalFileProvider(root)
});

// Serve static files
app.UseStaticFiles(staticOptions);

// Blazor fallback routing: serve index.html for all non-file requests
app.MapFallback(async context =>
{
    var indexFile = Path.Combine(root, "index.html");
    context.Response.ContentType = "text/html";
    await context.Response.SendFileAsync(indexFile);
});
app.MapGet("v1/healthz", () => Results.Ok("healthy"));

app.Run();