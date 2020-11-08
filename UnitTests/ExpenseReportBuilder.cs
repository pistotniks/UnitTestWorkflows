using System;
using EmployeeTodosApp;

namespace UnitTests
{
  public class ExpenseReportBuilder
  {
    private ExpenseReport mDefaultData;

    public ExpenseReportBuilder DefaultData()
    {
      mDefaultData = new ExpenseReport
      {
        Employee = new Person(),
        StartDate = DateTime.Now,
        EndDate = DateTime.Now
      };
      return this;
    }

    public ExpenseReport Build()
    {
      return mDefaultData;
    }
  }
}