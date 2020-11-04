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
using ExpenseReporting;
using System.Activities;
using System.Activities.Statements;

namespace EmployeeExpensesApplication
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    ExpenseReportViewModel model;
    WorkflowApplication wfApp;

    public MainWindow()
    {
      InitializeComponent();
      model = new ExpenseReportViewModel
      {
        Report = new ExpenseReport
        {
          Employee = new Person(),
          StartDate = DateTime.Now,
          EndDate = DateTime.Now
        },
        Response = new ManagerResponse()
      };


      DataContext = model;

      wfApp = new WorkflowApplication(
        new ReportProcessing(),
        new Dictionary<string, object>
        {
          {"report", model.Report}
        });

      wfApp.Completed += (wce) =>
      {
        var response = (ManagerResponse) wce.Outputs["managerResponse"];
        MessageBox.Show("Workflow completed - " + response.Approved.ToString());
      };
      wfApp.Idle += (wie) => { MessageBox.Show("Workflow idle"); };
    }


    private void Button_Click(object sender, RoutedEventArgs e)
    {
      wfApp.Run();
    }

    private void Approval_Click(object sender, RoutedEventArgs e)
    {
      wfApp.ResumeBookmark(
        "SubmitResponse", model.Response);
    }
  }
}