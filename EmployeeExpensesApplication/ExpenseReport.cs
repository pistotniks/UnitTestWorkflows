﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ExpenseReporting
{
    
    [DataContract]
    public class ExpenseReport
    {
        [DataMember]
        public Person Employee { get; set; }
        [DataMember]
        public double Amount { get; set; }
        [DataMember]
        public string Client { get; set; }
        [DataMember]
        public DateTime StartDate { get; set; }
        [DataMember]
        public DateTime EndDate { get; set; }
        [DataMember]
        public string City { get; set; }

    }
}
