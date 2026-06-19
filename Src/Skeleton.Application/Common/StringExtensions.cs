namespace Skeleton.Application.Common;

public static class StringExtensions
{
    public static string OrDefault(this string? value, string defaultValue)
        => string.IsNullOrWhiteSpace(value) ? defaultValue : value;
}
