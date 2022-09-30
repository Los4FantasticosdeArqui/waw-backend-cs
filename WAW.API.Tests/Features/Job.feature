Feature: Jobs API
As a developer
I want to manage Jobs through an API
In order to make it available for client applications.

  Scenario: Get all Companies
    Given I am a user client
    And the Jobs repository has data
      | Id | Title                                     | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Software Engineer                  | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
      | 2  | Remote Senior Embedded Android Specialist | https://miro.medium.com/max/875/1*8si943UlaDsBo8i7xm-bGg.png                                                  | Hinge Health is building a digital MSK clinic and is looking for experienced embedded developers       | $145k-$157k | true
    When a GET request is sent to Jobs
    Then a JobsResource response with status 200 is received
    And a list of JobResources is included in the body
      | Id | Title                                     | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Software Engineer                  | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
      | 2  | Remote Senior Embedded Android Specialist | https://miro.medium.com/max/875/1*8si943UlaDsBo8i7xm-bGg.png                                                  | Hinge Health is building a digital MSK clinic and is looking for experienced embedded developers       | $145k-$157k | true

  Scenario: Add Job with data
    Given I am a user client
    And the Jobs repository has data
      | Id | Title                                     | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Software Engineer                  | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
      | 2  | Remote Senior Embedded Android Specialist | https://miro.medium.com/max/875/1*8si943UlaDsBo8i7xm-bGg.png                                                  | Hinge Health is building a digital MSK clinic and is looking for experienced embedded developers       | $145k-$157k | true
    When a POST request is sent to Companies
      | Id | Title                            | Image                                                                                                           | Description                                                                   | SalaryRange  | Status
      | 3  | Remote Lead UX Researcher Motion | https://images.pexels.com/photos/3471423/pexels-photo-3471423.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | We're looking for a mission-driven individual to join us in creating engaging | $$145k-$160k | false
    Then a JobsResource response with status 200 is received
    And a JobsResource is included in the body
      | Id | Title                            | Image                                                                                                           | Description                                                                   | SalaryRange  | Status
      | 3  | Remote Lead UX Researcher Motion | https://images.pexels.com/photos/3471423/pexels-photo-3471423.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | We're looking for a mission-driven individual to join us in creating engaging | $$145k-$160k | false

  Scenario: Add invalid Job
    Given I am a user client
    When a POST request is sent to Jobs
      | Id | Title | Image                                                                                                           | Description                                                                   | SalaryRange  | Status
      | 3  |       | https://images.pexels.com/photos/3471423/pexels-photo-3471423.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | We're looking for a mission-driven individual to join us in creating engaging | $$145k-$160k | false
    Then a JobsResource response with status 400 is received
    And a JobsResource Error Message is included in the body
      | Message                      |
      | The Title field is required. |

  Scenario: Update existing Job
    Given I am a user client
    And the Jobs repository has data
      | Id | Title                    | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Software Engineer | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
    When a PUT request is sent to Jobs with Id 1
      | Title                            | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | Remote Lead UX Researcher Motion | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
    Then a JobsResource response with status 200 is received
    And a JobsResource is included in the body
      | Id | Title                            | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Lead UX Researcher Motion | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true

  Scenario: Delete existing Job
    Given I am a user client
    And the Jobs repository has data
      | Id | Title                    | Image                                                                                                         | Description                                                                                            | SalaryRange | Status
      | 1  | Remote Software Engineer | https://images.pexels.com/photos/574071/pexels-photo-574071.jpeg?auto=compress&cs=tinysrgb&w=1260&h=750&dpr=1 | This is a unique role that offers broad exposure to the systems and data that span our entire business | $115k-$117k | true
    When a DELETE request is sent to Jobs with Id 1
    Then a JobsResource response with status 204 is received
