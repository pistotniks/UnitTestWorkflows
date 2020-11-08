using System.Runtime.Serialization;

namespace EmployeeTodosApp
{
  [DataContract]
  public class ProductOwnerResponse
  {
    [DataMember] public bool Approved { get; set; }

    [DataMember] public string ProductOwnerName { get; set; }
  }
}