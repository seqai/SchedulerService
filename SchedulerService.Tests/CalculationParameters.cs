using System;
using System.Collections.Generic;

namespace SchedulerService.Tests
{
    internal record CalculationParameters(DateTime startDate, DateTime endDate, IReadOnlyList<int> courseLengths);
}
