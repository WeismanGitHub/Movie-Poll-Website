using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers;

[ApiController]
[Route("API/Polls")]
public class GetController : ControllerBase
{
    private class Movie
    {
        public required string Id { get; set; }
        public string? PosterPath { get; set; }
        public string? ReleaseDate { get; set; }
        public string? Title { get; set; }
    }

    private class GetPollResponse
    {
        public required string Question { get; set; }
        public required IEnumerable<string> Votes { get; set; }
        public required ICollection<Movie> Movies { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required bool ServerRestricted { get; set; }
        public DateTime? Expiration { get; set; }
    }

    private class TMDbMovieResponse
    {
        // There are more details, but I don't need them.
        public required int id { get; set; }
        public string? title { get; set; }
        public string? poster_path { get; set; }
        public string? release_date { get; set; }
    }

    [HttpGet("{id}", Name = "GetPoll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetPoll(
        [FromRoute] Guid id,
        Settings settings,
        HttpClient client,
        MoviePollsContext db
    )
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            settings.TmdbKey
        );

        var poll = await db
            .Polls.Where(p => p.Id == id)
            .Select(p => new
            {
                p.Question,
                p.Expiration,
                p.CreatedAt,
                p.GuildId,
                p.Id,
                p.MovieIds,
                Votes = p.Votes.Select(vote => vote.MovieId).ToList(),
            })
            .FirstOrDefaultAsync();

        if (poll == null)
        {
            return NotFound("Poll not found");
        }

        Movie[] movies = await Task.WhenAll(
            poll.MovieIds.Select(
                async (id) =>
                {
                    HttpResponseMessage res = await client.GetAsync(
                        $"https://api.themoviedb.org/3/movie/{id}"
                    );

                    if (!res.IsSuccessStatusCode)
                    {
                        return new Movie
                        {
                            Id = id,
                            PosterPath = null,
                            Title = null,
                            ReleaseDate = null,
                        };
                    }

                    TMDbMovieResponse? movie = JsonSerializer.Deserialize<TMDbMovieResponse>(
                        await res.Content.ReadAsStringAsync()
                    );

                    return new Movie
                    {
                        Id = id,
                        PosterPath = movie?.poster_path,
                        Title = movie?.title,
                        ReleaseDate = movie?.release_date
                    };
                }
            )
        );

        return Ok(
            new GetPollResponse()
            {
                Question = poll.Question,
                Votes = poll.Votes,
                Expiration = poll.Expiration,
                Movies = movies,
                ServerRestricted = poll.GuildId != null,
                CreatedAt = poll.CreatedAt,
            }
        );
    }
}
