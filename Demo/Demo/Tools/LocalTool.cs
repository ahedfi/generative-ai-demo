using System.ComponentModel;

namespace Demo.Tools;

public static class LocalTool
{
    [Description("Gets the current time")]
    public static DateTime GetCurrentTime() =>
        DateTime.Now;

    [Description("Gets the weather")]
    public static string GetWeather(string city) =>
        Random.Shared.NextDouble() > 0.5 ? $"It's sunny in {city}" : $"It's raining in {city}";
}
