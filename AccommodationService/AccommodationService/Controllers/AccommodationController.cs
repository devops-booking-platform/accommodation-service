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
    
    [HttpPut]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> Update([FromBody] AccommodationRequest request)
    {
	    await accommodationService.Update(request);
	    return StatusCode(StatusCodes.Status204NoContent);
    }
    
    [HttpGet("{id:guid}/reservation-info")]
    public async Task<ActionResult<AccommodationReservationInfoResponseDto>> GetReservationInfo(
        [FromRoute] Guid id,
        [FromQuery] DateOnly start,
        [FromQuery] DateOnly end,
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
	
	[HttpGet("{id:guid}")]
	public async Task<ActionResult<GetAccommodationResponse>> GetAccommodationInfo(Guid id, CancellationToken ct)
	{
		var dto = await accommodationService.Get(id, ct);
		return Ok(dto);
	}
	
	[HttpGet("amenities")]
	public async Task<ActionResult<AmenityResponseDto>> GetAmenities(CancellationToken ct)
	{
		var dto = await accommodationService.GetAmenities(ct);
		return Ok(dto);
	}
}