using System.ComponentModel;

namespace SchedulerService.Business.Calculation
{
    public enum CalculationStrategy
    {
        [Description("As fast as possible")]
        AsFastAsPossible,
        [Description("Spread out workload evenly")]
        SpreadOut
    }
}
