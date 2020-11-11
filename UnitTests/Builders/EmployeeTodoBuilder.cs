using System;
using EmployeeTodosApp.ViewModel;

namespace UnitTests.Builders
{
  public class EmployeeTodoBuilder
  {
    private EmployeeTodo mDefaultData;

    public EmployeeTodoBuilder DefaultData()
    {
      mDefaultData = new EmployeeTodo
      {
        Employee = new Person()
        {
          Name = "FooPersonName"
        },
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