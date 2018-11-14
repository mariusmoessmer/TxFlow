using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.Debug.Adapter
{
    class Program
    {
        static void Main(string[] args)
        {
            // Standard mode - run with the adapter connected to the process's stdin and stdout
            TxFlowDebugAdapter adapter = new TxFlowDebugAdapter(Console.OpenStandardInput(), Console.OpenStandardOutput());
            adapter.Protocol.LogMessage += (sender, e) => 
                System.Diagnostics.Debug.WriteLine(e.Message);
            adapter.Protocol.Run();
        }
    }
}
