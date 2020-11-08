using Activities.Data;

namespace Activities.Repositories
{
  public class EmployeeRepositoryRepository : IEmployeeRepositoryRepository
  {
    public Employee Get(string personName)
    {
      // Search by personName; Change it to simulate complex database change
      var employee = new Employee {StillWorks = true};
      return employee;
    }
  }
}