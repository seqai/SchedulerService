using System.Text.RegularExpressions;

namespace SchedulerService.Infrastructure
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        // Slugify value
        public string? TransformOutbound(object? value) => value == null ? null : Regex.Replace(value.ToString(), "([a-z])([A-Z])", "$1-$2").ToLower();
    }
}
