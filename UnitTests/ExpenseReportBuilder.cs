using System;
using EmployeeTodosApp;
using EmployeeTodosApp.ViewModel;

namespace UnitTests
{
  public class ExpenseReportBuilder
  {
    private EmployeeTodo mDefaultData;

    public ExpenseReportBuilder DefaultData()
    {
      mDefaultData = new EmployeeTodo
      {
        Employee = new Person(),
        StartDate = DateTime.Now,
      };
      return this;
    }

    public EmployeeTodo Build()
    {
      return mDefaultData;
    }
  }
}