using System.ComponentModel;

namespace SchedulerService.Business.Calculation
{
    public enum CalculationStrategy
    {
        [Description("As fast as possible")]
        AsFastAsPossible,
        [Description("Spread the workload evenly (naive)")]
        SpreadEvenlyNaive,
        [Description("Spread the workload evenly")]
        SpreadEvenly
    }
}
