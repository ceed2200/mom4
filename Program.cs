using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Koppling databas
builder.Services.AddDbContext<SongDb>(options =>
    options.UseSqlite("Data Source=SongDb.db"));

var app = builder.Build();

// Start av applikationen
app.MapGet("/", () => "Här kan du se den aktuella låtlistan");

// Visa alla låtar i listan
app.MapGet("/songs", async(SongDb db) => 
    await db.Songs.ToListAsync());

// Visa en särskild låt utifrån dess Id
app.MapGet("/songs/{id}", async(int id, SongDb db) => 
{
    if (await db.Songs.FindAsync(id) is Song song)
    {
        return Results.Ok(song);
    }

    return Results.NotFound();
});

// Lagra en till låt till listan
app.MapPost("/songs", async(Song newsong, SongDb db) => {
    db.Songs.Add(newsong);
    await db.SaveChangesAsync();
    return Results.Ok();
});

// Radera en särskild låt utifrån dess Id
app.MapDelete("/songs/{id}", async(int id, SongDb db) =>
{
    if (await db.Songs.FindAsync(id) is Song song)
    {
        db.Songs.Remove(song);
        await db.SaveChangesAsync();
        return Results.Ok();
    }

    return Results.NotFound();
});

// Uppdatera en särskild låt utifrån dess Id
app.MapPut("/songs/{id}", async(Song updatedsong, int id, SongDb db) =>
{
    if (await db.Songs.FindAsync(id) is Song song)
    {
        song.Artist = updatedsong.Artist;
        song.Title = updatedsong.Title;
        song.Length = updatedsong.Length;
        song.Category = updatedsong.Category;

        await db.SaveChangesAsync();
        return Results.Ok();
    }

    return Results.NotFound();
});

app.Run();

// Klasser för properties och databaskoppling

class Song {
    public int Id { get; set; }
    public string? Artist { get; set; }
    public string? Title { get; set; }
    public int? Length { get; set; }
    public string? Category { get; set; }
}

class SongDb : DbContext {
    public SongDb(DbContextOptions<SongDb> options) : base(options) { }
    public DbSet<Song> Songs => Set<Song>();
}


