using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;

namespace TestRunner
{

    class Program
    {
        static void Main(string[] args)
        {
            WorkflowInvoker.Invoke(GetWorkflow());
            Console.ReadLine();
        }

        private static Activity GetWorkflow()
        {
            Variable<string> textValue =
                new Variable<string> {Default="Default value" };
            
            return new CustomActivities.MySequence
            {
                Variables = { textValue },
                Activities =
                {
                    new WriteLine{Text = textValue},
                    new Assign<string> {To = textValue, 
                        Value = "Assigned value"},
                    new WriteLine{Text = textValue}   
                }
            };
        }

        private static Activity GetParallelWorkflow()
        {
            return new Sequence
            {
                Activities =
                {
                   new CustomActivities.MyParallel{
                       Branches = {
                           new WriteLine{Text = "First"},
                           new Sequence {
                               Activities  = {
                                   new Delay {Duration = TimeSpan.FromSeconds(2)},
                                   new WriteLine{Text = "Second"}
                               }
                           }
                           
                       }
                   },
                    new WriteLine{Text="After parallel"}
                }
            };
        }
    }
}
