using Microsoft.Data.Sqlite;
using PostService.DataAccess;
using PostService.Dtos;
using PostService.Mappings;
using PostService.BusinessLogic;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<IPostingService, PostingService>();
// Сталий шлях незалежно від запуску через термінал або відлагоджувач.
var connectionString = builder.Configuration.GetConnectionString("Postings")
    ?? new SqliteConnectionStringBuilder
    {
        DataSource = Path.Combine(builder.Environment.ContentRootPath, "PostingDb.db")
    }.ToString();
builder.Services.AddScoped<IPostingRepository>(_ => new PostingRepository(connectionString));
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var repository = (PostingRepository)scope.ServiceProvider.GetRequiredService<IPostingRepository>();
    repository.CreateDb();
}

// Перетворення помилок вхідних даних сервісу на HTTP 400.
app.Use(async (context, next) =>
{
    try { await next(context); }
    catch (ArgumentException ex) when (!context.Response.HasStarted)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        await context.Response.WriteAsJsonAsync(new { error = ex.Message });
    }
});

app.MapGet("/", () => "Це API поштового клієнта.");

app.MapGet("/postings", (IPostingService service) =>
    Results.Ok(service.GetAll().Select(PostingMapper.ToPostingGetDto)));

app.MapGet("/postings/{id:int}", (int id, IPostingService service) =>
{
    var posting = service.Find(id);
    return posting is null ? Results.NotFound()
        : Results.Ok(PostingMapper.ToPostingGetDto(posting));
});

app.MapPost("/postings", (PostingPostDto dto, IPostingService service) =>
{
    var posting = service.Create(PostingMapper.ToPosting(dto));
    var result = PostingMapper.ToPostingGetDto(posting);
    return Results.Created($"/postings/{result.Id}", result);
});

app.MapPut("/postings/{id:int}", (int id, PostingPutDto dto, IPostingService service) =>
{
    if (id != dto.Id)
        return Results.BadRequest(new { error = "Id у маршруті та тілі запиту мають збігатися." });
    var posting = service.Update(PostingMapper.ToPosting(dto));
    return posting is null ? Results.NotFound()
        : Results.Ok(PostingMapper.ToPostingGetDto(posting));
});

app.MapDelete("/postings/{id:int}", (int id, IPostingService service) =>
    service.Delete(id) == 0 ? Results.NotFound() : Results.NoContent());

app.Run();
