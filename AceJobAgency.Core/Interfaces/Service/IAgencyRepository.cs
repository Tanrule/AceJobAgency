using AceJobAgency.Core.Entities.Ecosystem;

namespace AceJobAgency.Core.Interfaces.Service
{
    public interface IAgencyRepository
    {
        Task<int> CreatePerson(Person person);
        Person GetPersonById(int id);
        Task UpdatePerson(Person person);
        Task<Document> CreateDocument(Document doc);
        Task<Document> UpdateDocument(Document doc);
        Task<Document> GetDocumentByPersonId(int personId);
    }
}
