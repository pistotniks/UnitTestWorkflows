using System.Runtime.Serialization;

namespace ExpenseReporting
{
  [DataContract]
  public class Person
  {
    [DataMember] public string Email { get; set; }
    [DataMember] public string Name { get; set; }
  }
}