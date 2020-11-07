using System.Activities.Hosting;
using System.Collections.Generic;

namespace CustomActivities
{
  public sealed class EmployeeRepositoryExtension : ICanGetEmployeeFacts
  {
    private readonly IEmployeeRepositoryRepository mRepository;

    public EmployeeRepositoryExtension(IEmployeeRepositoryRepository repository)
    {
      mRepository = repository;
    }

    public bool IsEmployeeStillEmployed(string personName)
    {
      Employee e = mRepository.Get(personName);
      return e.StillWorks;
    }

    protected WorkflowInstanceProxy Instance { get; private set; }

    public IEnumerable<object> GetAdditionalExtensions()
    {
      return null;
    }

    public void SetInstance(WorkflowInstanceProxy instance)
    {
      Instance = instance;
    }
  }
}