using System.Runtime.Serialization;

namespace EmployeeTodosApp.ViewModel
{
  [DataContract]
  public class Person
  {
    [DataMember] public string Email { get; set; }
    [DataMember] public string Name { get; set; }
  }
}