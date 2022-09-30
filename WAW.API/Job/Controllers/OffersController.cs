using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WAW.API.Auth.Authorization.Attributes;
using WAW.API.Job.Domain.Models;
using WAW.API.Job.Domain.Services;
using WAW.API.Job.Resources;
using WAW.API.Shared.Extensions;

namespace WAW.API.Job.Controllers;

// [Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Create, read, update and delete Job Offers")]
public class OffersController : ControllerBase {
  private readonly IOfferService service;
  private readonly IMapper mapper;

  public OffersController(IOfferService service, IMapper mapper) {
    this.service = service;
    this.mapper = mapper;
  }

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<OfferResource>), 200)]
  [SwaggerResponse(200, "All the stored job offers were retrieved successfully.", typeof(IEnumerable<OfferResource>))]
  public async Task<IEnumerable<OfferResource>> GetAll() {
    var offers = await service.ListAll();
    return mapper.Map<IEnumerable<Offer>, IEnumerable<OfferResource>>(offers);
  }

  [HttpPost]
  [ProducesResponseType(typeof(OfferResource), 200)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(200, "The job offer was created successfully", typeof(OfferResource))]
  [SwaggerResponse(400, "The job offer data is invalid")]
  public async Task<IActionResult> Post([FromBody] OfferRequest resource) {
    if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

    var offer = mapper.Map<OfferRequest, Offer>(resource);
    var result = await service.Create(offer);
    return result.ToResponse<OfferResource>(this, mapper);
  }

  [HttpPut("{id:int}")]
  [ProducesResponseType(typeof(OfferResource), 200)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(200, "The job offer was updated successfully", typeof(OfferResource))]
  [SwaggerResponse(400, "The job offer data is invalid")]
  public async Task<IActionResult> Put(
    [FromRoute] [SwaggerParameter("Job offer identifier", Required = true)] int id,
    [FromBody] OfferRequest resource
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

    var offer = mapper.Map<OfferRequest, Offer>(resource);
    var result = await service.Update(id, offer);
    return result.ToResponse<OfferResource>(this, mapper);
  }

  [HttpDelete("{id:int}")]
  [ProducesResponseType(typeof(NoContentResult), 204)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(204, "The job offer was deleted successfully", typeof(NoContentResult))]
  [SwaggerResponse(400, "The selected job offer to delete does not exist")]
  public async Task<IActionResult> DeleteAsync(
    [FromRoute] [SwaggerParameter("Job offer identifier", Required = true)] int id
  ) {
    await service.Delete(id);
    return NoContent();
  }
}
