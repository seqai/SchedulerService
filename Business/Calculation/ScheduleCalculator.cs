using LanguageExt;

namespace SchedulerService.Business.Calculation
{
    public static class ScheduleCalculator
    {
        const int StudyHoursPerDay = 8;

        public static Either<string, IReadOnlyList<int>> CalculateWeeklyStudyHours(DateTime startDate, DateTime endDate, IReadOnlyCollection<int> courseLengths, CalculationStrategy strategy)
        {
            int DayIndex(DateTime date) => ((int) date.DayOfWeek + 6) % 7;

            var goalHours = courseLengths.Sum();
            var totalAvailableHours = 0;
            var studyWeeks = Convert.ToInt32((endDate.Date.AddDays(6 - DayIndex(endDate)) - startDate.Date.AddDays(-DayIndex(startDate))).TotalDays / 7);
            var studyHoursPerDayOfWeek = new int[studyWeeks, 7];
            var weekIndex = 0;
            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var studyHours = GetAvailableHoursOnDate(date);
                totalAvailableHours += studyHours;
                studyHoursPerDayOfWeek[weekIndex, DayIndex(date)] = studyHours;

                if (date.DayOfWeek == DayOfWeek.Sunday)
                {
                    weekIndex++;
                }
            }

            if (totalAvailableHours < goalHours)
            {
                return $"Not enough time to complete the desired courses. Total required time {goalHours} hours, available time is {totalAvailableHours} hours";
            }

            var result = strategy switch
            {
                CalculationStrategy.AsFastAsPossible => CalculateGreedy(goalHours, studyHoursPerDayOfWeek),
                CalculationStrategy.SpreadOut => CalculateSpreadOut(goalHours, studyHoursPerDayOfWeek),
                _ => throw new ArgumentOutOfRangeException(nameof(strategy), strategy, null)
            };

            return result;
        }

        private static int[] CalculateGreedy(int goalHours, int[,] availableHoursPerWeek)
        {
            var totalWeeks = availableHoursPerWeek.GetUpperBound(0) + 1;
            var resultWeeklyStudyHours = new int [totalWeeks];

            for (var weekIndex = 0; weekIndex < totalWeeks; ++weekIndex)
            {
                for (var dayIndex = 0; dayIndex < 6; ++dayIndex)
                {
                    if (goalHours == 0)
                    {
                        return resultWeeklyStudyHours;
                    }

                    var spentHours = Math.Min(availableHoursPerWeek[weekIndex, dayIndex], goalHours);
                    resultWeeklyStudyHours[weekIndex] += spentHours;
                    goalHours -= spentHours;
                }
            }

            return resultWeeklyStudyHours;
        }

        private static int[] CalculateSpreadOut(int goalHours, int[,] availableHoursPerWeek)
        {
            // in current scenario only last and first week can have less that 40 hours available,
            // but in reality schedule might be more complex, so let's assume available hours can have any arbitrary data
            // naive approach just to iterate substracting an hour while we still have goal to reach
            // as goal hours probably within very reasonable limits

            var totalWeeks = availableHoursPerWeek.GetUpperBound(0) + 1;
            var resultWeeklyStudyHours = new int[totalWeeks];

            while (goalHours > 0)
            {
                var hoursBeforeRun = goalHours;

                for (var weekIndex = 0; weekIndex < totalWeeks; ++weekIndex)
                {
                    for (var dayIndex = 0; dayIndex < 6; ++dayIndex)
                    {
                        if (goalHours == 0)
                        {
                            return resultWeeklyStudyHours;
                        }

                        var spentHours = Math.Min(availableHoursPerWeek[weekIndex, dayIndex], 1);
                        resultWeeklyStudyHours[weekIndex] += spentHours;
                        availableHoursPerWeek[weekIndex, dayIndex] -= spentHours;
                        goalHours -= spentHours;
                    }
                }

                if (hoursBeforeRun == goalHours)
                {
                    // Invariant violation
                    throw new InvalidOperationException("Spread Out Calculation infinite loop");
                }
            }

            return resultWeeklyStudyHours;
        }

        // Can be as complex as needed, e.g. use student personal calendar
        private static int GetAvailableHoursOnDate(DateTime day) =>
            (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday) ? StudyHoursPerDay : 0;
    }
}
