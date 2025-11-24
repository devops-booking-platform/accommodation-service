using AccommodationService.Domain.DTOs;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationService.Controllers;

[Route("api/accommodation")]
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
}