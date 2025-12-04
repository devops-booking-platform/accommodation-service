using AccommodationService.Domain.DTOs;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationService.Controllers;

[Route("api/accommodations")]
[ApiController]
public class AccommodationController(IAccommodationService accommodationService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> Create([FromBody] AccommodationRequest request)
    {
        await accommodationService.Create(request);
        return StatusCode(StatusCodes.Status201Created);
    }

    [HttpGet("{id:guid}/reservation-info")]
    public async Task<ActionResult<AccommodationReservationInfoResponseDTO>> GetReservationInfo(
        [FromRoute] Guid id,
        [FromQuery] DateTimeOffset start,
        [FromQuery] DateTimeOffset end,
        [FromQuery] int guests,
        CancellationToken ct)
    {
        var dto = await accommodationService.GetReservationInfoAsync(id, start, end, guests, ct);
        return Ok(dto);
    }

	[HttpGet("my")]
	[Authorize(Roles = "Host")]
	public async Task<IActionResult> GetMy(CancellationToken ct)
	{
		var list = await accommodationService.GetMyAsync(ct);
		return Ok(list);
	}
}