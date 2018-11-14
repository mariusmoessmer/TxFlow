using System.Activities;
using System.Threading;

namespace TxFlow.WFBuilder.Layout
{
    internal static class FlowChartLayoutingUtil
    {
        internal static string ArrangeLayout(ActivityBuilder builder)
        {
            FlowChartLayouter layouter = null;
            Thread thread = new System.Threading.Thread(() =>
            {
                layouter = new FlowChartLayouter(builder);
                System.Windows.Threading.Dispatcher.Run();

            })
            {
                IsBackground = true,
                Name = "ArrangeLayoutThread",
            };

            thread.SetApartmentState(System.Threading.ApartmentState.STA);
            thread.Start();

            thread.Join();

            return layouter.WorkflowXaml;
        }
    }
}
