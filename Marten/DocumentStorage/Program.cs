using Marten;
using Weasel.Core;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten(o =>
{
    o.Connection(builder.Configuration.GetConnectionString("Default"));

    o.AutoCreateSchemaObjects = AutoCreate.None;

    o.Schema.For<User>();
    o.Schema.For<Post>().Index(x =>x.By);
})
    .ApplyAllDatabaseChangesOnStartup()
    .AssertDatabaseMatchesConfigurationOnStartup();

var app = builder.Build();

app.MapGet("/{guid}", (Guid guid, IQuerySession session) =>
{
    return session.LoadAsync<User>(guid);
});
app.MapGet("/list", (IQuerySession session) =>
{
    return session.Query<User>()
        .Where(x => x.Age > 21)
        .Select(x => new { x.Age })
        .ToListAsync();
});

app.MapGet("/related", async (IQuerySession session) =>
{
    var users = new List<User>();
    var posts = await session.Query<Post>()
        .Include(x => x.By, users)
        .ToListAsync();

    return new
    {
        posts,
        users
    };
});

app.MapGet("/batch", async (IQuerySession session) =>
{
    var query = session.CreateBatchQuery();

    var postQuery = query.Load<Post>(new Guid("018158db-a6e1-45a1-a8ce-213323059348"));
    var userQuery = query.Load<User>(new Guid("018158cd-658a-4b52-9594-75ee80ccc4ba"));

    await query.Execute();

    return new
    {
        post = await postQuery,
        user = await userQuery
    };
});

app.MapGet("/create", async (IDocumentSession session) =>
{
    var existing = new User("Bar", 20) { Id = new Guid("018158cd-658a-4b52-9594-75ee80ccc4ba") };
    session.Store(existing, new User("Baz", 25));
    await session.SaveChangesAsync();
    return "created!";
});

app.MapGet("/{userId:guid}/create-content", async (Guid userId, IDocumentSession session) =>
{
    session.Store(
        new Post("One body", "One Title z ", userId),
        new Post("Two body", "Two Title z ", userId)
    );
    await session.SaveChangesAsync();
    return "created!";
});

app.Run();

public record User(string Name, int Age)
{
    public Guid Id { get; set; }
}

public record Post(string Body, string Title, Guid By)
{
    public Guid Id { get; set; }
}