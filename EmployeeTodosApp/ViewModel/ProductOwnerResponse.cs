using System.Runtime.Serialization;

namespace EmployeeTodosApp.ViewModel
{
  [DataContract]
  public class ProductOwnerResponse
  {
    [DataMember] public bool Approved { get; set; }

    [DataMember] public string ProductOwnerName { get; set; }
  }
}