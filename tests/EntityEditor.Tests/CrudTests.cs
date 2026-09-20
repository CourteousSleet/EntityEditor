using System.Net;
using System.Text.RegularExpressions;
using EntityEditor.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace EntityEditor.Tests;

// Each test owns a fresh SQL Server database, never the application's database.
public sealed class CrudTests : IDisposable
{
    private readonly string connectionString;
    private readonly WebApplicationFactory<Program> app;
    private readonly HttpClient browser;

    public CrudTests()
    {
        var connection = new SqlConnectionStringBuilder(
            Environment.GetEnvironmentVariable("ENTITYEDITOR_TEST_CONNECTION")
            ?? @"Server=(localdb)\mssqllocaldb;Integrated Security=true;TrustServerCertificate=true");
        connection.InitialCatalog = "EntityEditorTests_" + Guid.NewGuid().ToString("N");
        connectionString = connection.ConnectionString;
        using var context = OpenContext();
        context.Database.EnsureCreated();
        app = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Development");
            builder.ConfigureAppConfiguration((_, config) => config.AddInMemoryCollection(
                new Dictionary<string, string> { ["ConnectionStrings:EntityEditorContext"] = connectionString }));
        });
        browser = app.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    private EntityEditorContext OpenContext() => new(new DbContextOptionsBuilder<EntityEditorContext>()
        .UseSqlServer(connectionString).Options);

    [Fact]
    public void Seeding_is_repeatable_and_creates_related_founders()
    {
        using var context = OpenContext();
        DBInitializer.Initialize(context);
        var clients = context.Clients.Include(c => c.Founders).ToList();
        Assert.NotEmpty(clients);
        Assert.Contains(clients, c => c.Founders.Count > 0);
        var founderCount = context.Founders.Count();
        Assert.True(founderCount > 0);
        DBInitializer.Initialize(context);
        Assert.Equal(clients.Count, context.Clients.Count());
        Assert.Equal(founderCount, context.Founders.Count());
    }

    [Fact]
    public async Task Client_can_be_created_read_edited_and_deleted_without_overposting()
    {
        var before = DateTime.UtcNow.AddSeconds(-2);
        var create = Fields("Example client");
        create["Client.ID"] = "99999";
        create["Client.CreationDate"] = "2000-01-01";
        create["Client.UpdateDate"] = "2000-01-01";
        create["Client.Founders[0].Initials"] = "Injected founder";
        Assert.Equal(HttpStatusCode.Redirect, (await PostForm("/Clients/Create", create)).StatusCode);
        using var context = OpenContext();
        var client = await context.Clients.SingleAsync();
        Assert.NotEqual(99999, client.ID);
        Assert.InRange(client.CreationDate, before, DateTime.UtcNow.AddSeconds(2));
        Assert.Equal(client.CreationDate, client.UpdateDate);
        Assert.Empty(await context.Founders.ToListAsync());
        Assert.Contains("Example client", await browser.GetStringAsync("/Clients"));
        Assert.Contains("Example client", await browser.GetStringAsync($"/Clients/Details?id={client.ID}"));

        var edit = Fields("Updated client");
        edit["Client.ID"] = client.ID.ToString();
        edit["Client.CreationDate"] = "2001-01-01";
        edit["Client.UpdateDate"] = "2001-01-01";
        Assert.Equal(HttpStatusCode.Redirect, (await PostForm($"/Clients/Edit?id={client.ID}", edit)).StatusCode);
        var creationDate = client.CreationDate;
        await context.Entry(client).ReloadAsync();
        Assert.Equal("Updated client", client.Name);
        Assert.Equal(creationDate, client.CreationDate);
        Assert.True(client.UpdateDate >= creationDate);
        Assert.Equal(HttpStatusCode.Redirect, (await PostForm($"/Clients/Delete?id={client.ID}",
            new() { ["Client.ID"] = client.ID.ToString() })).StatusCode);
        Assert.False(await context.Clients.AnyAsync());
        Assert.Equal(HttpStatusCode.NotFound, (await browser.GetAsync($"/Clients/Details?id={client.ID}")).StatusCode);
    }

    [Theory]
    [InlineData("Client.Name", "")]
    [InlineData("Client.IndividualTaxNumber", "invalid")]
    [InlineData("Client.OrganizationType", "invalid")]
    public async Task Invalid_input_is_rejected_without_writing_to_database(string field, string value)
    {
        var fields = Fields("Example client");
        fields[field] = value;
        var response = await PostForm("/Clients/Create", fields);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("field-validation-error", await response.Content.ReadAsStringAsync());
        using var context = OpenContext();
        Assert.False(await context.Clients.AnyAsync());
    }

    [Fact]
    public async Task Missing_client_returns_not_found()
    {
        foreach (var page in new[] { "Details", "Edit", "Delete" })
            Assert.Equal(HttpStatusCode.NotFound, (await browser.GetAsync($"/Clients/{page}?id=2147483647")).StatusCode);
        var fields = Fields("Missing client");
        fields["Client.ID"] = "2147483647";
        var tokenPage = await browser.GetStringAsync("/Clients/Create");
        fields["__RequestVerificationToken"] = Token(tokenPage);
        Assert.Equal(HttpStatusCode.NotFound, (await browser.PostAsync("/Clients/Edit?id=2147483647",
            new FormUrlEncodedContent(fields))).StatusCode);
    }

    [Fact]
    public async Task Deleting_a_company_also_removes_its_founders()
    {
        using var context = OpenContext();
        DBInitializer.Initialize(context);
        var company = await context.Clients.SingleAsync(c => c.OrganizationType == "EN");
        var response = await PostForm($"/Clients/Delete?id={company.ID}",
            new() { ["Client.ID"] = company.ID.ToString() });
        Assert.True(response.StatusCode == HttpStatusCode.Redirect, await response.Content.ReadAsStringAsync());
        Assert.False(await context.Clients.AnyAsync(c => c.ID == company.ID));
        Assert.False(await context.Founders.AnyAsync());
        Assert.True(await context.Clients.AnyAsync(c => c.OrganizationType == "IE"));
    }

    [Fact]
    public async Task Post_without_antiforgery_token_is_rejected()
    {
        Assert.Equal(HttpStatusCode.BadRequest,
            (await browser.PostAsync("/Clients/Create", new FormUrlEncodedContent(Fields("Forged")))).StatusCode);
        using var context = OpenContext();
        Assert.False(await context.Clients.AnyAsync());
    }

    private static Dictionary<string, string> Fields(string name) => new()
    {
        ["Client.Name"] = name,
        ["Client.IndividualTaxNumber"] = "123456789012",
        ["Client.OrganizationType"] = "IE"
    };

    private async Task<HttpResponseMessage> PostForm(string url, Dictionary<string, string> fields)
    {
        fields["__RequestVerificationToken"] = Token(await browser.GetStringAsync(url));
        return await browser.PostAsync(url, new FormUrlEncodedContent(fields));
    }

    private static string Token(string html)
    {
        var match = Regex.Match(html, "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"");
        Assert.True(match.Success, "The form must contain an antiforgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    public void Dispose()
    {
        browser.Dispose();
        app.Dispose();
        using var context = OpenContext();
        context.Database.EnsureDeleted();
    }
}
