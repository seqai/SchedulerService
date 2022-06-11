using System.ComponentModel;

namespace SchedulerService.Infrastructure
{
    internal static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {            
            var field = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute?.Description ?? value.ToString();
        }
    }
}
