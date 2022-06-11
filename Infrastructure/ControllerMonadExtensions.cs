using System.Diagnostics.Contracts;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;

namespace SchedulerService.Infrastructure
{
    public static class ControllerMonadExtensions
    {
        [Pure]
        public static Task<ActionResult<R>> MatchActionResult<A, R>(
            this TryOptionAsync<A> self,
            Func<A, R> some,
            Func<ActionResult> none,
            Func<Exception, ActionResult> fail) => self.Match(x => (ActionResult<R>) some(x), () => none(), ex => fail(ex));

        [Pure]
        public static Task<ActionResult<R>> MatchActionResult<A, R>(
            this TryAsync<A> self,
            Func<A, R> some,
            Func<Exception, ActionResult> fail) => self.Match(x => (ActionResult<R>) some(x), ex => fail(ex));

        [Pure]
        public static ActionResult<Ret> MatchActionResult<L, R, Ret>(
            this Either<L, R> self,
            Func<R, Ret> right, Func<L, ActionResult> left) => self.Match(r => (ActionResult<Ret>) right(r), l => left(l));
    }
}
