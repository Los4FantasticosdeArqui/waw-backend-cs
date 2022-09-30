namespace WAW.API.Tests;

[Binding]
public class JobStep {
  [Given(@"I am a user client")]
  public void GivenIAmAUserClient() {
    ScenarioContext.StepIsPending();
  }

  [Given(@"the Jobs repository has data")]
  public void GivenTheJobsRepositoryHasData(Table table) {
    ScenarioContext.StepIsPending();
  }

  [Then(@"a list of JobResources is included in the body")]
  public void ThenAListOfJobResourcesIsIncludedInTheBody(Table table) {
    ScenarioContext.StepIsPending();
  }

  [When(@"a GET request is sent to Jobs")]
  public void WhenAgetRequestIsSentToJobs() {
    ScenarioContext.StepIsPending();
  }

  [Then(@"a JobsResource response with status (.*) is received")]
  public void ThenAJobsResourceResponseWithStatusIsReceived(int p0) {
    ScenarioContext.StepIsPending();
  }

  [Then(@"a JobsResource is included in the body")]
  public void ThenAJobsResourceIsIncludedInTheBody(Table table) {
    ScenarioContext.StepIsPending();
  }

  [When(@"a POST request is sent to Jobs")]
  public void WhenApostRequestIsSentToJobs(Table table) {
    ScenarioContext.StepIsPending();
  }

  [Then(@"a JobsResource Error Message is included in the body")]
  public void ThenAJobsResourceErrorMessageIsIncludedInTheBody(Table table) {
    ScenarioContext.StepIsPending();
  }

  [When(@"a PUT request is sent to Jobs with Id (.*)")]
  public void WhenAputRequestIsSentToJobsWithId(int p0, Table table) {
    ScenarioContext.StepIsPending();
  }

  [When(@"a DELETE request is sent to Jobs with Id (.*)")]
  public void WhenAdeleteRequestIsSentToJobsWithId(int p0) {
    ScenarioContext.StepIsPending();
  }
}
