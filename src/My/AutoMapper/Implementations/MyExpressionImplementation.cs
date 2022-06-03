using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace AutoMapper.Implementations;

internal class MyExpressionImplementation : IMapper
{
    private readonly Dictionary<(Type, Type), Delegate> _cache = new();

    public TTo Map<TFrom, TTo>(TFrom from) where TTo : new()
    {
        Type fromType = typeof(TFrom);
        Type toType = typeof(TTo);

        (Type, Type) key = (fromType, toType);
        if (!_cache.ContainsKey(key))
        {
            _cache.Add(key, CreateDelegate(fromType, toType));
        }

        return (TTo)_cache[key].DynamicInvoke(from);

    }

    private Delegate CreateDelegate(Type fromType, Type toType)
    {
        ParameterExpression param = Expression.Parameter(fromType);
        NewExpression newExpression = Expression.New(constructor: toType.GetConstructor(Type.EmptyTypes));

        List<MemberBinding> bindings = new();
        foreach (var fromProperty in fromType.GetProperties())
        {
            PropertyInfo toPropertyInfo = toType.GetProperty(fromProperty.Name);
            if (toPropertyInfo is null)
            {
                continue;
            }

            MemberExpression fromTypePropertyAccessExpression = Expression.MakeMemberAccess(param, fromProperty);

            MemberAssignment toTypePropertyInitializationBinding = Expression.Bind(toPropertyInfo, fromTypePropertyAccessExpression);
            bindings.Add(toTypePropertyInitializationBinding);
        }

        MemberInitExpression toTypeInitializationExpression = Expression.MemberInit(newExpression, bindings);

        return Expression.Lambda(toTypeInitializationExpression, false, param).Compile();
    }
}
