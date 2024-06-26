﻿namespace Chirper.Posts.Endpoints;

public class UnlikePost : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id}/unlike", Handle)
        .WithSummary("Unlikes a post")
        .WithRequestValidation<Request>()
        .WithEnsureEntityExists<Post, Request>(x => x.Id);

    public record Request(int Id);
    public class RequestValidator : AbstractValidator<Request>
    {
        public RequestValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0);
        }
    }

    private static async Task<Results<Ok, NotFound>> Handle([AsParameters] Request request, AppDbContext db, ClaimsPrincipal claimsPrincipal, CancellationToken ct)
    {
        var userId = claimsPrincipal.GetUserId();

        var rowsDeleted = await db.Likes
            .Where(x => x.PostId == request.Id && x.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return rowsDeleted == 0
            ? TypedResults.NotFound()
            : TypedResults.Ok();
    }
}