using Newtonsoft.Json;
using NUnit.Framework;
using RestSharp;
using System.Collections.Generic;
using System.Net;
using TechTalk.SpecFlow;

namespace PetstoreApiTests
{
    [Binding]
    public class PetStoreSteps : IDisposable
    {
        private RestClient _client;
        private bool disposed = false;
        private RestResponse _response;
        private dynamic _retrievedPet;

        public PetStoreSteps()
        {
            _client = new RestClient("https://petstore.swagger.io/v2");
        }

        [Given(@"a pet with ID (\d+) exists")]
        public void PetWithIDExists(int petId)
        {

            var checkRequest = new RestRequest($"pet/{petId}", Method.Get);
            var checkResponse = _client.Execute(checkRequest);

            if (checkResponse.StatusCode != HttpStatusCode.OK)
            {
                var createRequest = new RestRequest("pet", Method.Post);
                var newPet = new
                {
                    id = petId,
                    name = "TestPet",
                    photoUrls = new List<string> { "http://example.com/photo" },
                    status = "available"
                };
                createRequest.AddJsonBody(newPet);

                var createResponse = _client.Execute(createRequest);
                Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            }
        }

        [When(@"I update the pet with ID (\d+) status to ""(.*)"" and name to ""(.*)""")]
        public void UpdatePetStatusAndName(int petId, string status, string name)
        {
            var updateRequest = new RestRequest("pet", Method.Post);
            var updatedPet = new
            {
                id = petId,
                name = name,
                status = status
            };
            updateRequest.AddJsonBody(updatedPet);
            _response = _client.Execute(updateRequest);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the pet with ID (\d+) status is '(.*)' and name is '(.*)'")]
        public void ThenThePetWithIDStatusIsAndNameIs(int petId, string expectedStatus, string expectedName)
        {
            var getRequest = new RestRequest($"pet/{petId}", Method.Get);
            var getResponse = _client.Execute(getRequest);
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            _retrievedPet = JsonConvert.DeserializeObject<dynamic>(getResponse.Content);
            Assert.That((int)_retrievedPet.id, Is.EqualTo(petId));
            Assert.That((string)_retrievedPet.name, Is.EqualTo(expectedName));
            Assert.That((string)_retrievedPet.status, Is.EqualTo(expectedStatus));
        }

         [When(@"I delete the pet with ID (\d+)")]
        public void DeletePet(int petId)
        {
            var deleteRequest = new RestRequest($"pet/{petId}", Method.Delete);
            _response = _client.Execute(deleteRequest);
            Assert.That(_response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Then(@"the pet with ID (\d+) does not exist")]
        public void PetDoesNotExist(int petId)
        {
            var verifyRequest = new RestRequest($"pet/{petId}", Method.Get);
            var verifyResponse = _client.Execute(verifyRequest);
            Assert.That(verifyResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [When(@"I attempt to create a pet with invalid data")]
        public void PetWithInvalidData()
        {
            var request = new RestRequest("pet", Method.Post);
            var invalidPet = new
            {
                id = "invalid_id"
            };
            request.AddJsonBody(invalidPet);

            _response = _client.Execute(request);
        }

        [Then(@"the response status code should be (.*)")]
        public void CheckResponsCode(int statusCode)
        {
            Assert.That(_response.StatusCode, Is.EqualTo((HttpStatusCode)statusCode));
        }

        [AfterScenario]
        public void Cleanup()
        {

            Dispose();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                _client?.Dispose();
                disposed = true;
            }
        }
    }
}