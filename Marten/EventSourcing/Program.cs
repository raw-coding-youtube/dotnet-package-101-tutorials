using Marten;
using Marten.Events.Aggregation;
using Marten.Events.Projections;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMarten(o =>
{
    o.Connection(builder.Configuration.GetConnectionString("Default"));
    o.Projections.Add<Projector>();
    o.Projections.SelfAggregate<P3>(ProjectionLifecycle.Async);
    o.Projections.Add<UserSummaryProjector>(ProjectionLifecycle.Inline);
});
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/p1", (IQuerySession session) =>
{
    return session.Events.AggregateStreamAsync<P1>(new Guid("01815d5b-659a-47c5-81c3-88ce9a234e50"));
});

app.MapGet("/p2", (IQuerySession session) =>
{
    return session.Events.AggregateStreamAsync<P2>(new Guid("01815d5b-659a-47c5-81c3-88ce9a234e50"));
});

app.MapGet("/p3", (IQuerySession session) =>
{
    return session.LoadAsync<P3>(new Guid("01815d5b-659a-47c5-81c3-88ce9a234e50"));
});
app.MapGet("/sum/{guid}", (Guid guid, IQuerySession session) =>
{
    return session.LoadAsync<UserSummary>(guid);
});

app.MapGet("/rebuild", async (IDocumentStore store) =>
{
    var daemon = await store.BuildProjectionDaemonAsync();
    await daemon.RebuildProjection<P3>(CancellationToken.None);
    return "rebuilt!";
});

app.MapGet("/cart", async (IDocumentSession session) =>
{
    var result = session.Events.StartStream(
        new AddedToCart("Pants", 2),
        new UpdatedShippingInformation("42 Street", "123465")
    );

    await session.SaveChangesAsync();

    return "created!";
});

app.MapGet("/append", async (IDocumentSession session) =>
{
    session.Events.Append(
        new Guid("01815d5b-659a-47c5-81c3-88ce9a234e50"),
        new AddedToCart("T-Shirt", 3)
    );

    await session.SaveChangesAsync();

    return "created!";
});

app.MapGet("/post", async (IDocumentSession session) =>
{
    var useId = Guid.NewGuid();
    session.Events.StartStream(new CreatedPost("one", useId));
    session.Events.StartStream(new CreatedPost("two", useId));
    session.Events.StartStream(new Commented("yes!", useId));

    await session.SaveChangesAsync();

    return useId;
});

app.Run();


public interface IUserEvent
{
    public Guid UserId { get; init; }
}

public record CreatedPost(string Title, Guid UserId) : IUserEvent;

public record Commented(string Content, Guid UserId) : IUserEvent;

public class UserSummary
{
    public Guid Id { get; set; }
    public List<string> Posts { get; set; } = new();
    public List<string> Comments { get; set; } = new();
}

public class UserSummaryProjector : MultiStreamAggregation<UserSummary, Guid>
{
    public UserSummaryProjector()
    {
        Identity<IUserEvent>(x => x.UserId);
    }

    public void Apply(UserSummary snapshot, CreatedPost e)
    {
        snapshot.Posts.Add(e.Title);
    }

    public void Apply(UserSummary snapshot, Commented e)
    {
        snapshot.Comments.Add(e.Content);
    }
}

public class P1
{
    public Guid Id { get; set; }
    public List<string> Products { get; set; } = new();

    public void Apply(AddedToCart e)
    {
        Products.Add(e.Name);
    }
}

public class P3
{
    public Guid Id { get; set; }
    public List<string> Products { get; set; } = new();
    public string PhoneNumber { get; set; }

    public void Apply(AddedToCart e)
    {
        Products.Add(e.Name);
    }

    public void Apply(UpdatedShippingInformation e)
    {
        PhoneNumber = e.PhoneNumber;
    }
}

public class P2
{
    public Guid Id { get; set; }

    public int TotalQty { get; set; }
}

public class Projector : SingleStreamAggregation<P2>
{
    public void Apply(P2 snapshot, AddedToCart e)
    {
        snapshot.TotalQty += e.Qty;
    }
}


public record AddedToCart(string Name, int Qty);

public record UpdatedShippingInformation(string Address, string PhoneNumber);