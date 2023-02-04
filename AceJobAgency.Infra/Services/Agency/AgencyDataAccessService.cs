using AceJobAgency.Core.Entities.Ecosystem;
using AceJobAgency.Core.Interfaces.Service;
using AceJobAgency.Infra.Contexts;

namespace AceJobAgency.Infra.Services.Agency
{
    public class AgencyDataAccessService : IAgencyRepository
    {
        private readonly AgencyContext _agencyContext;

        public AgencyDataAccessService(AgencyContext agencyContext)
        {
            _agencyContext = agencyContext;
        }

        public async Task<int> CreatePerson(Person person)
        {
            await _agencyContext.People.AddAsync(person);
            await SaveChangesAsync();
            return person.Id;
        }
        public async Task UpdatePerson(Person person)
        {
            _agencyContext.People.Update(person);
            await SaveChangesAsync();
        }

        public Person GetPersonById(int personId)
        {
            var person = _agencyContext.People.SingleOrDefault(t => t.Id == personId);
            //var docs = _agencyContext.Documents.Where(t => t.PersonId == person.Id).ToList();
            //person.Documents = docs;
            return person;
        }

        private async Task SaveChangesAsync()
        {
            await _agencyContext.SaveChangesAsync();
        }

        public async Task<Document> CreateDocument(Document doc)
        {
            _agencyContext.Documents.Add(doc);
            await SaveChangesAsync();
            return doc;
        }

        public async Task<Document> UpdateDocument(Document doc)
        {
            _agencyContext.Documents.Update(doc);
            await SaveChangesAsync();
            return doc;
        }

        public async Task<Document> GetDocumentByPersonId(int personId)
        {
            return _agencyContext.Documents.SingleOrDefault(t => t.PersonId == personId);
        }

    }
}
