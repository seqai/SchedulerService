using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FsCheck;

namespace SchedulerService.Tests
{
    internal class Arbitraries
    {
        public enum Solvability
        {
            Any,
            SolvableOnly,
            UnsolvableOnly
        }

        public static Arbitrary<CalculationParameters> ParamsGenerator(int minDays, int maxDays, int minCourses, int maxCourses, int minCourseLength, int maxCourseLength, Solvability solvability)
        {
            var date = new DateTime(2020, 1, 1);
            
            var gen = from startOffset in Gen.Choose(1, 3000)
                      from endOffset in Gen.Choose(minDays, maxDays)
                      from coursesN in Gen.Choose(minCourses, maxCourses)
                      from courses in Gen.ListOf(coursesN, Gen.Choose(minCourseLength, maxCourseLength))
                      where     solvability == Solvability.Any 
                      || (solvability == Solvability.SolvableOnly && courses.Sum() < endOffset * 0.7 * 8) 
                      || (solvability == Solvability.UnsolvableOnly && courses.Sum() > endOffset * 8)
                      select new CalculationParameters(date.AddDays(startOffset), date.AddDays(startOffset + endOffset), courses.ToList());
            
            return gen.ToArbitrary();
        }
    }
}
