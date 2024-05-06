using System.Data;
using Dapper;

namespace Krake.Core.Infrastructure.Data.TypeHandlers;

internal sealed class TimeOnlyTypeHandler : SqlMapper.TypeHandler<TimeOnly>
{
    public override TimeOnly Parse(object value) => value switch
    {
        DateTime dateTime => TimeOnly.FromDateTime(dateTime),
        TimeSpan timeSpan => TimeOnly.FromTimeSpan(timeSpan),
        _ => default
    };

    public override void SetValue(IDbDataParameter parameter, TimeOnly value)
    {
        parameter.DbType = DbType.Time;
        parameter.Value = value;
    }
}