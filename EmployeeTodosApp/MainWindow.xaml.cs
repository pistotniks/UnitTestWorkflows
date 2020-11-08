using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Activities;
using System.Activities.Statements;
using Activities;
using Activities.Repositories;
using Activities.WorkflowExtensions;
using EmployeeTodosApp.ViewModel;

namespace EmployeeTodosApp
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    TodoViewModel model;
    WorkflowApplication workflowApplication;

    public MainWindow()
    {
      InitializeComponent();
      model = new TodoViewModel
      {
        TodoTask = new EmployeeTodo
        {
          Employee = new Person(),
          StartDate = DateTime.Now,
        },
        Response = new ProductOwnerResponse()
      };


      DataContext = model;

      workflowApplication = new WorkflowApplication(
        new Workflows.ReportProcessing(),
        new Dictionary<string, object>
        {
          {"report", model.TodoTask}
        });

      workflowApplication.Extensions.Add(new EmployeeRepositoryExtension(new EmployeeRepositoryRepository()));
      
      workflowApplication.Completed += (wce) =>
      {
        var response = (ProductOwnerResponse) wce.Outputs["managerResponse"];
        if (response == null)
        {
          MessageBox.Show("Workflow completed. Employee is not employed anymore and no approval for the TODO is needed.");
          this.Dispatcher.Invoke(() =>
          {
            ApprovalSubmit.IsEnabled = false;
          });
          
        }
        else
        {
          MessageBox.Show($"Workflow completed - Product Owner {(response.Approved ? "Approved" : "did not Approved")} the Employee's TODO.");
        }
      };
      workflowApplication.Idle += (wie) => { MessageBox.Show("Workflow idle. Waiting for Product Owner's approval of a Employee's TODO Task."); };
    }


    private void Button_Click(object sender, RoutedEventArgs e)
    {
      workflowApplication.Run();
    }

    private void Approval_Click(object sender, RoutedEventArgs e)
    {
      workflowApplication.ResumeBookmark(
        "SubmitResponse", model.Response);
    }
  }
}