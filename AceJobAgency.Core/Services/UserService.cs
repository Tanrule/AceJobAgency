using AceJobAgency.Core.Entities.Ecosystem;
using AceJobAgency.Core.Interfaces.Service;
using AceJobAgency.Core.Models.User;

namespace AceJobAgency.Core.Services
{
    public class UserService
    {
        private readonly IUserRepository _userService;
        private readonly IAgencyRepository _agencyService;

        public UserService(IUserRepository userService, IAgencyRepository agencyService)
        {
            _userService = userService;
            _agencyService = agencyService;
        }

        public async Task<Person> CreatePerson(int userId,
            string firstName, string lastName, DateTime dateOfBirth, Gender gender, string nric)
        {
            try
            {
                var person = new Person()
                {
                    FirstName = firstName,
                    LastName = lastName,
                    DateOfBirth = dateOfBirth,
                    Gender = gender,
                    NRIC = nric
                };
                var personId = await _agencyService.CreatePerson(person);
                await _userService.UpdateUserPersonId(userId, personId);
                return person;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Person GetPersonById(int personId)
        {
            try
            {
                return _agencyService.GetPersonById(personId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task UpdatePerson(Person person)
        {
            try
            {
                await _agencyService.UpdatePerson(person);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        

        public async Task InsertAudit(string actionName, string ipAdress, string loggedInAt, string loggedOutAt, string pageAccessed, string pageName, string sessionId,
            string urlReferrer, string actionType, int? userId, string description)
        {
            try
            {
                await _userService.InsertAudit(actionName, ipAdress, loggedInAt, loggedOutAt, pageAccessed, pageName, sessionId,
            urlReferrer, actionType, userId, description);
            }
            catch (Exception e)
            {
                
            }
        }

        public async Task CreateDocument(Document doc)
        {
            try
            {
                await _agencyService.CreateDocument(doc);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        public async Task<Document> UpdateDocument(Document doc)
        {
            return await _agencyService.UpdateDocument(doc);
        }

        public async Task<Document> GetDocumentByPersonId(int personId)
        {
            return await _agencyService.GetDocumentByPersonId(personId);
        }
    }
}
