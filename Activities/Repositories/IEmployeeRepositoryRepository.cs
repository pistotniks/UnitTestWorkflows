using Activities.Data;

namespace Activities.Repositories
{
  public interface IEmployeeRepositoryRepository
  {
    Employee Get(string personName);
  }
}