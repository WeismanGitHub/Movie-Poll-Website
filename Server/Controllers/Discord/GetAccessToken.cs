using System.ComponentModel.DataAnnotations;

namespace Server.Controllers;

[ApiController]
[Route("API/Discord")]
public class GetAccessTokenController : ControllerBase
{
    [HttpGet("Token", Name = "GetAccessToken")]
    [Produces<string>]
    [Tags("Discord")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAccessToken(
        [FromQuery(Name = "code"), Required, MaxLength(50)] string authCode,
        DiscordOauth2 discord
    )
    {
        var accessToken = await discord.GetAccessToken(authCode);

        return Ok(accessToken);
    }
}
