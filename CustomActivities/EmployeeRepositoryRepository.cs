namespace CustomActivities
{
  public class EmployeeRepositoryRepository : IEmployeeRepositoryRepository
  {
    public Employee Get(string personName)
    {
      // Search by personName
      var employee = new Employee {StillWorks = false};
      return employee;
    }
  }
}