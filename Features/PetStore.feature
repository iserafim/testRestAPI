Feature: PetStore API Tests
  Scenario: Update Pet Status
    Given a pet with ID 222 exists
    When I update the pet with ID 222 status to "sold" and name to "UpdateName"
    Then the pet with ID 222 status is 'sold' and name is 'UpdateName'

  Scenario: Delete Pet
    Given a pet with ID 222 exists
    When I delete the pet with ID 222
    Then the pet with ID 222 does not exist

  Scenario: Create Pet Bad Request
    When I attempt to create a pet with invalid data
    Then the response status code should be 500