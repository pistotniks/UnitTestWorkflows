using System.Runtime.Serialization;

namespace EmployeeTodosApp
{
  [DataContract]
  public class ManagerResponse
  {
    [DataMember] public bool Approved { get; set; }

    [DataMember] public string ManagerName { get; set; }
  }
}