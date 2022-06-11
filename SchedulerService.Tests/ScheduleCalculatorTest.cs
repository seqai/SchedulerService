using System;
using System.Linq;
using FsCheck;
using FsCheck.Xunit;
using SchedulerService.Business.Calculation;
using Xunit;
using static SchedulerService.Business.Calculation.ScheduleCalculator;

namespace SchedulerService.Tests
{
    public class ScheduleCalculatorTest
    {
        private static readonly CalculationStrategy[] Strategies = Enum.GetValues(typeof(CalculationStrategy)).Cast<CalculationStrategy>().ToArray();

        [Property(MaxTest = 1000)]
        public Property AllSolvableParametersSolvedByAllStrategiesRealistic() => Prop.ForAll(
            Arbitraries.ParamsGenerator(10, 50, 1, 10, 4, 20, Arbitraries.Solvability.SolvableOnly),
            AssertSolvable
        );

        [Property(MaxTest = 1000)]
        public Property AllSolvableParametersSolvedByAllStrategiesMoreRandom() => Prop.ForAll(
            Arbitraries.ParamsGenerator(1, 5000, 1, 100, 1, 200, Arbitraries.Solvability.SolvableOnly),
            AssertSolvable
        );

        private void AssertSolvable(CalculationParameters parameters)
        {
            var (startDate, endDate, courseLengths) = parameters;
            foreach (var strategy in Strategies)
            {
                var result = CalculateWeeklyStudyHours(startDate, endDate, courseLengths, strategy);
                Assert.True(result.IsRight);
            }
        }

        [Property(MaxTest = 1000)]
        public Property AllUnsolvableParametersUnsolvedByAllStrategiesRealistic() => Prop.ForAll(
            Arbitraries.ParamsGenerator(10, 50, 1, 10, 4, 20, Arbitraries.Solvability.UnsolvableOnly),
            AssertUnsolvable
        );

        [Property(MaxTest = 1000)]
        public Property AllUnsolvableParametersUnsolvedByAllStrategiesMoreRandom() => Prop.ForAll(
            Arbitraries.ParamsGenerator(1, 5000, 1, 100, 1, 200, Arbitraries.Solvability.UnsolvableOnly),
            AssertUnsolvable
        );

        private void AssertUnsolvable(CalculationParameters parameters)
        {
            var (startDate, endDate, courseLengths) = parameters;
            foreach (var strategy in Strategies)
            {
                var result = CalculateWeeklyStudyHours(startDate, endDate, courseLengths, strategy);
                Assert.True(result.IsLeft);
            }
        }

        [Property(MaxTest = 1000)]
        public Property AllParametersSolvedOrUnsolvedByAllStrategiesTogetherRealistic() => Prop.ForAll(
            Arbitraries.ParamsGenerator(10, 50, 1, 10, 4, 20, Arbitraries.Solvability.Any),
            AssertSame
        );

        [Property(MaxTest = 1000)]
        public Property AllParametersSolvedOrUnsolvedByAllStrategiesTogetherMoreRandom() => Prop.ForAll(
            Arbitraries.ParamsGenerator(1, 5000, 1, 100, 1, 200, Arbitraries.Solvability.Any),
            AssertSame
        );


        private void AssertSame(CalculationParameters parameters)
        {
            var (startDate, endDate, courseLengths) = parameters;
            var solvable = CalculateWeeklyStudyHours(startDate, endDate, courseLengths, CalculationStrategy.AsFastAsPossible).IsRight;
            foreach (var strategy in Strategies.Skip(1))
            {
                var result = CalculateWeeklyStudyHours(startDate, endDate, courseLengths, strategy);
                Assert.True(result.IsRight == solvable);
            }
        }

        [Property(MaxTest = 1000)]
        public Property NaiveAndOptimizedSpreadYieldSameResultRealistic()
        {
            return Prop.ForAll(
                Arbitraries.ParamsGenerator(10, 50, 1, 10, 4, 20, Arbitraries.Solvability.SolvableOnly),
                AssertSpread
            );
        }

        [Property(MaxTest = 1000)]
        public Property NaiveAndOptimizedSpreadYieldSameResultMoreRandom()
        {
            return Prop.ForAll(
                Arbitraries.ParamsGenerator(1, 5000, 1, 100, 1, 200, Arbitraries.Solvability.SolvableOnly),
                AssertSpread
            );
        }

        private void AssertSpread(CalculationParameters parameters)
        {
            var (startDate, endDate, courseLengths) = parameters;
            var arrays = from naive in CalculateWeeklyStudyHours(startDate, endDate, courseLengths, CalculationStrategy.SpreadEvenlyNaive) from optimized in CalculateWeeklyStudyHours(startDate, endDate, courseLengths, CalculationStrategy.SpreadEvenly) select (naive, optimized);

            arrays.Match(xss => Assert.Equal(xss.naive, xss.optimized), _ => Assert.True(false, "Impossible"));
        }

    }
}