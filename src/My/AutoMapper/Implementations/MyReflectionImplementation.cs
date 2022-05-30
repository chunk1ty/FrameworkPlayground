using System;
using System.Collections.Generic;
using System.Reflection;

namespace AutoMapper.Implementations;

internal class MyReflectionImplementation : IMapper
{
    private Dictionary<(Type from, Type to), List<(PropertyInfo FromPropertyGetter, PropertyInfo ToPropertySetter)>> _cache = new();

    // from MyRequest
    // to MyRequestViewModel
    public TTo Map<TFrom, TTo>(TFrom from) where TTo : new()
    {
        (Type from, Type to) key = (from: typeof(TFrom), to: typeof(TTo));

        if (!_cache.ContainsKey(key))
        {
            PopulateCache(key);
        }

        List<(PropertyInfo FromPropertyGetter, PropertyInfo ToPropertySetter)> propertyMappings = _cache[key];

        var result = new TTo();
        foreach ((PropertyInfo FromPropertyGetter, PropertyInfo ToPropertySetter) propertyPair in propertyMappings)
        {
            var value = propertyPair.FromPropertyGetter.GetValue(from);
            propertyPair.ToPropertySetter.SetValue(result, value);
        }

        return result;
    }

    public void PopulateCache((Type From, Type To) key)
    {
        var fromProperties = key.From.GetProperties();
        var toProperties = key.To.GetProperties();

        var properties = new List<(PropertyInfo Get, PropertyInfo Set)>();
        foreach (var fromTypeProperty in fromProperties)
        {
            foreach (var toTyppeProperty in toProperties)
            {
                if (fromTypeProperty.Name == toTyppeProperty.Name)
                {
                    properties.Add((fromTypeProperty, toTyppeProperty));
                    break;
                }
            }
        }

        _cache.Add(key, properties);
    }
}
