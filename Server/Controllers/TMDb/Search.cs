using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http.Headers;

namespace Server.Controllers;

[ApiController]
[Route("API/TMDb")]
public class SearchController : ControllerBase
{
    [HttpGet("search", Name = "SearchMovie")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SearchMovie(
        [FromQuery(Name = "query"), MaxLength(500), MinLength(1), Required] string query,
        [FromQuery(Name = "page"), MaxLength(500), MinLength(1), Required] string page,
        Settings settings
    )
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
            "Bearer",
            settings.TmdbKey
        );

        HttpResponseMessage res = await client.GetAsync(
            $"https://api.themoviedb.org/3/search/movie?query={query}&include_adult=true&page={page}'"
        );

        if (res.StatusCode == HttpStatusCode.BadRequest)
        {
            return BadRequest("Invalid search request.");
        }
        else if (!res.IsSuccessStatusCode)
        {
            throw new Exception("Search failed.");
        }

        var content = await res.Content.ReadAsStringAsync();

        return Ok(content);
    }
}
