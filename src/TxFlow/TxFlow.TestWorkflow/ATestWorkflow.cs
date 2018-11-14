using System;
using TxFlow.CSharpDSL;

namespace TxFlow.TestWorkflow
{

    public class AbstractActivityToolbox
    {
        public virtual void WriteLine(string v)
        {
        }
    }

    public class ATestWorkflow : AbstractWorkflow<AbstractActivityToolbox>
    {
        public void Execute(int value) {
            Activities.WriteLine(value.ToString());

            int result = fibonacci(value);

            Activities.WriteLine(result.ToString());

            if(result > 2) {
                Activities.WriteLine("Result higher than 2!");
            }
        }

        private int fibonacci(int n)
        {
            int a = 0;
            int b = 1;
            // In N steps compute Fibonacci sequence iteratively.
            for (int i = 0; i < n; i++)
            {
                int temp = a;
                a = b;
                b = temp + b;
            }
            return a;
        }

        // void Execute(bool decisionVar)
        // {
        //     int value = yeah(3,2);

        //     string myStr = "PDC Rocks" + value;
        //     if(decisionVar)
        //     {
        //         for(int i = 0; 
        //             i < 3; 
        //             i++)
        //         {
        //             Activities.WriteLine("In True Branch" + i.ToString());
        //         }
        //         myStr = "Marius";
        //     }else
        //     {
        //         Activities.WriteLine("In False Branch" + myStr);
        //     }

        //     Activities.WriteLine("Done");
        // }

        // private int yeah(int a, int b)
        // {
        //     return a+b;
        // }

        // private void other(ref int za, int def, out int x)
        // {
        //     Activities.WriteLine(za.ToString() + "," + def);
        //     x = 66;
        // }

        //void Execute(int argument1, int argument2)
        //{
        //    //Activities.Parallel(testmethod);

        //    try
        //    {
        //        Activities.WriteLine("try before");
        //        throw new ArgumentException("test exception thrown!");
        //        Activities.WriteLine("try");
        //    }
        //    catch (ArgumentException ex)
        //    {
        //        Activities.WriteLine("Arg" + ex.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        Activities.WriteLine("Exc" + ex.Message);
        //    }
        //    finally
        //    {
        //        Activities.WriteLine("yeah");
        //    }

        //    System.Collections.Generic.List<int> lst = new System.Collections.Generic.List<int>() { 2 };
        //    bool tmp = Activities.ExistsInCollection(lst, 1);
        //    Activities.WriteLine("ist drinnen:" + tmp);

        //    for (int i = 0, j = 2; i < 3; i++, j++)
        //    {
        //        Activities.WriteLine(i.ToString());
        //    }


        //    Activities.AddToCollection(lst, 1);
        //    tmp = Activities.ExistsInCollection(lst, 2);
        //    Activities.WriteLine("ist drinnen:" + tmp);

        //    Activities.ExistsInCollection(lst, 2);


        //    int sum = argument1 + argument2;
        //    Activities.WriteLine("Sum: " + argument1 + " + " + argument2 + " = " + sum);
        //    if (sum > 10)
        //    {
        //        Activities.WriteLine("Sum is higher than 10: " + sum);
        //    }
        //    else
        //    {
        //        Activities.WriteLine("Sum is lower or equal than 10. ");
        //        return;
        //    }

        //    Activities.WriteLine("afterwards");
        //    Activities.WriteLine("Max number from [0,9] = " + System.Linq.Enumerable.Max(System.Linq.Enumerable.Range(0, 10)));
        //    //bool defaultMatched = false;
        //    //switch (sum)
        //    //{
        //    //    case 12: WriteLine("Matched! Alles Cool2!"); defaultMatched = false; break;
        //    //    case 11: WriteLine("case 11"); break;
        //    //    default: WriteLine("No-Match"); defaultMatched = true; break;
        //    //}

        //    //WriteLine("default matched: " + defaultMatched);
        //    testmethod();
        //    Activities.WriteLine("Yeah nach testmethod");
        //    testmethod();

        //    //Activities.Parallel(() => Activities.WriteLine("yeah"), () => Activities.WriteLine("yeah"));
        //}

        //private void testmethod()
        //{
        //    Activities.WriteLine("Yeah in testmethod");
        //    bool val = true;
        //    if (val == true)
        //    {
        //        Activities.WriteLine("yeah in if in testemethod");
        //    }
        //    Activities.WriteLine("Yeah in testmethod2");
        //}
    }
}
