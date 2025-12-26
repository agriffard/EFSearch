using System.Linq.Expressions;
using System.Reflection;
using EFSearch.Mapping;
using EFSearch.Models;

namespace EFSearch.Internal;

/// <summary>
/// Builds Expression trees from search filters.
/// </summary>
internal static class ExpressionBuilder
{
    private static readonly MethodInfo StringContainsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
    private static readonly MethodInfo StringStartsWithMethod = typeof(string).GetMethod(nameof(string.StartsWith), [typeof(string)])!;
    private static readonly MethodInfo StringEndsWithMethod = typeof(string).GetMethod(nameof(string.EndsWith), [typeof(string)])!;

    /// <summary>
    /// Builds a predicate expression from a list of filters.
    /// </summary>
    public static Expression<Func<T, bool>> BuildPredicate<T>(
        IEnumerable<SearchFilter> filters,
        SearchMap<T> map)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var filter in filters)
        {
            if (!map.TryGetProperty(filter.Field, out var propertyInfo) || propertyInfo == null)
            {
                throw new InvalidOperationException($"Field '{filter.Field}' is not mapped.");
            }

            if (!map.IsOperatorAllowed(filter.Operator))
            {
                throw new InvalidOperationException($"Operator '{filter.Operator}' is not allowed.");
            }

            var filterExpression = BuildFilterExpression(parameter, propertyInfo, filter);

            combinedExpression = combinedExpression == null
                ? filterExpression
                : Expression.AndAlso(combinedExpression, filterExpression);
        }

        combinedExpression ??= Expression.Constant(true);
        return Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
    }

    private static Expression BuildFilterExpression(
        ParameterExpression parameter,
        PropertyInfo propertyInfo,
        SearchFilter filter)
    {
        var property = Expression.Property(parameter, propertyInfo);
        var value = ConvertValue(filter.Value, propertyInfo.PropertyType);
        var constant = Expression.Constant(value, propertyInfo.PropertyType);

        return filter.Operator switch
        {
            FilterOperator.Equals => Expression.Equal(property, constant),
            FilterOperator.NotEquals => Expression.NotEqual(property, constant),
            FilterOperator.GreaterThan => Expression.GreaterThan(property, constant),
            FilterOperator.GreaterThanOrEqual => Expression.GreaterThanOrEqual(property, constant),
            FilterOperator.LessThan => Expression.LessThan(property, constant),
            FilterOperator.LessThanOrEqual => Expression.LessThanOrEqual(property, constant),
            FilterOperator.Contains => BuildStringMethodCall(property, constant, StringContainsMethod),
            FilterOperator.StartsWith => BuildStringMethodCall(property, constant, StringStartsWithMethod),
            FilterOperator.EndsWith => BuildStringMethodCall(property, constant, StringEndsWithMethod),
            _ => throw new NotSupportedException($"Operator '{filter.Operator}' is not supported.")
        };
    }

    private static Expression BuildStringMethodCall(
        MemberExpression property,
        ConstantExpression constant,
        MethodInfo method)
    {
        if (property.Type != typeof(string))
        {
            throw new InvalidOperationException(
                $"String operators can only be applied to string properties. Property '{property.Member.Name}' is of type '{property.Type.Name}'.");
        }

        return Expression.Call(property, method, constant);
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
        {
            if (!IsNullable(targetType))
            {
                throw new InvalidOperationException(
                    $"Cannot assign null to non-nullable type '{targetType.Name}'.");
            }
            return null;
        }

        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value.GetType() == underlyingType)
        {
            return value;
        }

        try
        {
            return Convert.ChangeType(value, underlyingType);
        }
        catch (Exception ex) when (ex is InvalidCastException or FormatException or OverflowException)
        {
            throw new InvalidOperationException(
                $"Cannot convert value '{value}' to type '{underlyingType.Name}'.", ex);
        }
    }

    private static bool IsNullable(Type type)
    {
        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }
}
