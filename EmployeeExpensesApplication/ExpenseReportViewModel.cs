using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmployeeExpensesApplication
{
    public class ExpenseReportViewModel
    {
        public ExpenseReporting.ExpenseReport Report { get; set; }
        public ExpenseReporting.ManagerResponse Response { get; set; }
    }
}
