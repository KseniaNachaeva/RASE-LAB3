using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using RaceSimulator.Models;

namespace RaceSimulator.Services;

public static class ReflectionService
{
    public static Dictionary<string, object?> GetCarProperties(RaceCar car)
    {
        var result = new Dictionary<string, object?>();
        var properties = typeof(RaceCar).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            if (prop.CanRead)
                result[prop.Name] = prop.GetValue(car);
        }

        return result;
    }

    public static string GetCarInfo(RaceCar car)
    {
        var props = GetCarProperties(car);
        var sb = new StringBuilder();

        sb.AppendLine($"=== {car.Name} ===");
        foreach (var (key, value) in props)
        {
            sb.AppendLine($"  {key}: {value}");
        }

        return sb.ToString();
    }

    public static List<string> GetPropertyNames<T>()
    {
        var names = new List<string>();
        foreach (var prop in typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            names.Add(prop.Name);
        }
        return names;
    }

    public static void SetProperty(object target, string propertyName, object value)
    {
        var prop = target.GetType().GetProperty(propertyName,
            BindingFlags.Public | BindingFlags.Instance);

        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found on {target.GetType().Name}");

        prop.SetValue(target, Convert.ChangeType(value, prop.PropertyType));
    }
}
