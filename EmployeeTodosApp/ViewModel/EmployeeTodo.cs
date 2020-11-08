using System;
using System.Runtime.Serialization;

namespace EmployeeTodosApp.ViewModel
{
  [DataContract]
  public class EmployeeTodo
  {
    [DataMember] public Person Employee { get; set; }
    [DataMember] public string Todo { get; set; }
    [DataMember] public DateTime StartDate { get; set; }
  }
}