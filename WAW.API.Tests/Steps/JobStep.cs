using System.Net;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using TechTalk.SpecFlow.Assist;
using WAW.API.Job.Domain.Models;
using WAW.API.Job.Domain.Repositories;
using WAW.API.Job.Resources;
using WAW.API.Shared.Domain.Repositories;
using WAW.API.Tests.Helpers;
using Xunit;

namespace WAW.API.Tests.Steps;

[Binding]
public class JobStep {
  private const string endpoint = "/api/v1/offers";
  private readonly WebApplicationFactory<Program> factory;
  private readonly IOfferRepository repository;
  private readonly IUnitOfWork unitOfWork;
  private HttpClient client = null!;
  private HttpResponseMessage response = null!;
  private OfferResource? entity;
  private IEnumerable<OfferResource>? entities;
  public JobStep(
    WebApplicationFactory<Program> factory,
    IOfferRepository repository,
    IUnitOfWork unitOfWork
  ) {
    this.factory = factory;
    this.repository = repository;
    this.unitOfWork = unitOfWork;
  }

  [Given(@"I am a user client")]
  public void GivenIAmAUserClient() {
    client = factory.CreateDefaultClient();
  }

  [Given(@"the Jobs repository has data")]
  public async Task GivenTheJobsRepositoryHasData(Table table) {
    var entries = table.CreateSet<Offer>();
    foreach (var entry in entries) {
      await repository.Add(entry);
      await unitOfWork.Complete();
    }
  }

  [Then(@"a list of JobResources is included in the body")]
  public async Task ThenAListOfJobResourcesIsIncludedInTheBody(Table table) {
    entities = await response.Content.ReadFromJsonAsync<List<OfferResource>>();
    table.CompareToSet(entities);
  }

  [When(@"a GET request is sent to Jobs")]
  public async Task WhenAGetRequestIsSentToJobs() {
    response = await client.GetAsync(endpoint);
  }

  [Then(@"a JobsResource response with status (.*) is received")]
  public void ThenAJobsResourceResponseWithStatusIsReceived(int status) {
    var expected = (HttpStatusCode) status;
    Assert.Equal(expected, response.StatusCode);
  }

  [Then(@"a JobsResource is included in the body")]
  public async Task ThenAJobsResourceIsIncludedInTheBody(Table table) {
    entity = await response.Content.ReadFromJsonAsync<OfferResource>();
    table.CompareToInstance(entity);
  }

  [When(@"a POST request is sent to Jobs")]
  public async Task WhenAPostRequestIsSentToJobs(Table table) {
    var data = table.CreateInstance<OfferRequest>();
    var json = JsonConvert.SerializeObject(data);
    var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    response = await client.PostAsync(endpoint, content);
  }

  [Then(@"a JobsResource Error Message is included in the body")]
  public async Task ThenAJobsResourceErrorMessageIsIncludedInTheBody(Table table) {
    var text = await response.Content.ReadAsStringAsync();
    var error = table.CreateInstance<TextError>();
    Assert.Contains(error.Message, text);
  }

  [When(@"a PUT request is sent to Jobs with Id (.*)")]
  public async Task WhenAPutRequestIsSentToJobsWithId(int id, Table table) {
    var data = table.CreateInstance<OfferRequest>();
    var json = JsonConvert.SerializeObject(data);
    var content = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
    response = await client.PutAsync($"{endpoint}/{id}", content);
  }

  [When(@"a DELETE request is sent to Jobs with Id (.*)")]
  public async Task WhenADeleteRequestIsSentToJobsWithId(int id) {
    response = await client.DeleteAsync($"{endpoint}/{id}");
  }
}
