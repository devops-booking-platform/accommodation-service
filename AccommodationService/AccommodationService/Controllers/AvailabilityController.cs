using AccommodationService.Domain.DTOs;
using AccommodationService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccommodationService.Controllers;

[Route("api/availability")]
[ApiController]
public class AvailabilityController(IAvailabilityService availabilityService) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = "Host")]
    public async Task<IActionResult> CreateOrUpdate([FromBody] AvailabilityRequest request)
    {
        await availabilityService.CreateOrUpdate(request);
        return StatusCode(StatusCodes.Status200OK);
    }
}