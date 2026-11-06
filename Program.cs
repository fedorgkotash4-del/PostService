using PostService.Models;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var postings = new List<Posting>
{
    new Posting
    {
        Id = 1, From = "Alice", To = "Bob", Content = "Books",
        DeliveryType = DeliveryType.Courier,
        Weight = 2.5f, Width = 30, Height = 20, Depth = 10,
        Value = 50f, Price = 10f, CreatedAt = DateTime.UtcNow
    },
    new Posting
    {
        Id = 2, From = "Olena", To = "Andrii", Content = "Documents",
        DeliveryType = DeliveryType.Department,
        Weight = 0.5f, Width = 25, Height = 2, Depth = 18,
        Value = 100f, Price = 5f, CreatedAt = DateTime.UtcNow
    }
};
// Лічильник не залежить від наявності записів у списку.
var nextId = postings.Max(p => p.Id) + 1;
var sync = new object();

app.MapGet("/", () => "Це API поштового клієнта.");

app.MapGet("/postings", () =>
{
    lock (sync) { return Results.Json(postings); }
});

app.MapGet("/postings/{id:int}", (int id) =>
{
    lock (sync)
    {
        var posting = postings.FirstOrDefault(p => p.Id == id);
        return posting is null ? Results.NotFound() : Results.Json(posting);
    }
});

app.MapPost("/postings", (Posting posting) =>
{
    lock (sync)
    {
        posting.Id = nextId++;
        posting.CreatedAt = DateTime.UtcNow;
        postings.Add(posting);
        return Results.Created($"/postings/{posting.Id}", posting);
    }
});

app.MapPut("/postings/{id:int}", (int id, Posting updated) =>
{
    lock (sync)
    {
        var posting = postings.FirstOrDefault(p => p.Id == id);
        if (posting is null) return Results.NotFound();

        posting.From = updated.From;
        posting.To = updated.To;
        posting.Content = updated.Content;
        posting.DeliveryType = updated.DeliveryType;
        posting.Weight = updated.Weight;
        posting.Width = updated.Width;
        posting.Height = updated.Height;
        posting.Depth = updated.Depth;
        posting.Value = updated.Value;
        posting.Price = updated.Price;
        return Results.Ok(posting);
    }
});

app.MapDelete("/postings/{id:int}", (int id) =>
{
    lock (sync)
    {
        var posting = postings.FirstOrDefault(p => p.Id == id);
        if (posting is null) return Results.NotFound();
        postings.Remove(posting);
        return Results.NoContent();
    }
});

app.Run();
