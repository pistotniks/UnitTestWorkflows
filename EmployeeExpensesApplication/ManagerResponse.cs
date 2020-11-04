using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExpenseReporting
{
    [DataContract]
    public class ManagerResponse
    {
        [DataMember]
        public bool Approved { get; set; }

        [DataMember]
        public string ManagerName { get; set; }
    }
}
