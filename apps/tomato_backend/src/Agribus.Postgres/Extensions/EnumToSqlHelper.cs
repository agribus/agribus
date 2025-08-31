namespace Agribus.Postgres.Extensions;

public static class EnumToSqlHelper
{
    public static string GetEnumValuesSql<TEnum>() where TEnum : struct, Enum
    {
        var values = Enum.GetValues(typeof(TEnum))
            .Cast<TEnum>()
            .Select(e => e.ToString().ToLowerInvariant());

        return $"'{string.Join("', '", values)}'";
    }
}