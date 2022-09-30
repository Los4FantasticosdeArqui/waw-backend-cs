using BoDi;
using Microsoft.AspNetCore.Mvc.Testing;
using WAW.API.Employers.Domain.Repositories;
using WAW.API.Job.Domain.Repositories;
using WAW.API.Shared.Domain.Repositories;
using WAW.API.Tests.Helpers;

namespace WAW.API.Tests.Hooks;

[Binding]
public class CompanyHooks {
  private readonly IObjectContainer objectContainer;
  private WebApplicationFactory<Program>? factory;

  public CompanyHooks(IObjectContainer objectContainer) {
    this.objectContainer = objectContainer;
  }

  [BeforeScenario]
  public async Task RegisterServices() {
    factory ??= AppFactory.GetWebApplicationFactory();
    await ClearData(factory);
    objectContainer.RegisterInstanceAs(factory);
    var companiesRepository = factory.Services.GetService(typeof(ICompanyRepository)) as ICompanyRepository;
    objectContainer.RegisterInstanceAs(companiesRepository);
    var jobRepository = factory.Services.GetService(typeof(IOfferRepository)) as IOfferRepository;
    objectContainer.RegisterInstanceAs(jobRepository);
    var unitOfWork = factory.Services.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
    objectContainer.RegisterInstanceAs(unitOfWork);
  }

  private static async Task ClearData(WebApplicationFactory<Program> factory) {
    if ((factory.Services.GetService(typeof(ICompanyRepository)) is not ICompanyRepository companyRepository)
      || factory.Services.GetService(typeof(IOfferRepository)) is not IOfferRepository offerRepository) {
      return;
    }

    var companyEntities = await companyRepository.ListAll();
    foreach (var entity in companyEntities) {
      companyRepository.Remove(entity);
    }

    var offerEntities = await offerRepository.ListAll();
    foreach (var entity in offerEntities) {
      offerRepository.Remove(entity);
    }
  }
}
