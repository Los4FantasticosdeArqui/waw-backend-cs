using System.Net.Mime;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WAW.API.Auth.Authorization.Attributes;
using WAW.API.Employers.Domain.Services;
using WAW.API.Employers.Resources;
using WAW.API.Shared.Extensions;

namespace WAW.API.Employers.Controllers;

using Domain.Models;

//[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Create, read, update and delete Companies")]
public class CompaniesController : ControllerBase {
  private readonly ICompanyService service;
  private readonly IMapper mapper;

  public CompaniesController(ICompanyService service, IMapper mapper) {
    this.service = service;
    this.mapper = mapper;
  }

  [HttpGet]
  [ProducesResponseType(typeof(IEnumerable<CompanyResource>), 200)]
  [SwaggerResponse(200, "All the stored companies were retrieved successfully.", typeof(IEnumerable<CompanyResource>))]
  public async Task<IEnumerable<CompanyResource>> GetAll() {
    var companies = await service.ListAll();
    return mapper.Map<IEnumerable<Company>, IEnumerable<CompanyResource>>(companies);
  }

  [HttpPost]
  [ProducesResponseType(typeof(CompanyResource), 201)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(201, "The company was created successfully", typeof(CompanyResource))]
  [SwaggerResponse(400, "The company data is invalid")]
  public async Task<IActionResult> Post(
    [FromBody] [SwaggerRequestBody("The company object about to create", Required = true)] CompanyRequest companyRequest
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

    var company = mapper.Map<CompanyRequest, Company>(companyRequest);
    var result = await service.Create(company);
    return result.ToResponse<CompanyResource>(this, mapper);
  }

  [HttpPut("{id:int}")]
  [ProducesResponseType(typeof(CompanyResource), 200)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(200, "The company was updated successfully", typeof(CompanyResource))]
  [SwaggerResponse(400, "The company data is invalid")]
  public async Task<IActionResult> Put(
    [FromRoute] [SwaggerParameter("Company identifier", Required = true)] int id,
    [FromBody] [SwaggerRequestBody("The company object about to update and its changes", Required = true)]
    CompanyRequest companyRequest
  ) {
    if (!ModelState.IsValid) return BadRequest(ModelState.GetErrorMessages());

    var company = mapper.Map<CompanyRequest, Company>(companyRequest);
    var result = await service.Update(id, company);
    return result.ToResponse<CompanyResource>(this, mapper);
  }

  [HttpDelete("{id:int}")]
  [ProducesResponseType(typeof(NoContentResult), 204)]
  [ProducesResponseType(typeof(List<string>), 400)]
  [ProducesResponseType(500)]
  [SwaggerResponse(204, "The company was deleted successfully", typeof(NoContentResult))]
  [SwaggerResponse(400, "The selected company to delete does not exist")]
  public async Task<IActionResult> DeleteAsync(
    [FromRoute] [SwaggerParameter("Company identifier", Required = true)] int id
  ) {
    await service.Delete(id);
    return NoContent();
  }
}
