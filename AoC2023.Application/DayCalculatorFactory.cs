using AoC23.Domain;
using System.Reflection;

namespace AoC2023.Application;
public static class DayCalculatorFactory
{
    public static IDayCalculator? GetCalculator(int day)
    {
        var calculatorType = new AssemblyReference().Assembly
            .GetTypes()
            .Where(t => typeof(IDayCalculator).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .FirstOrDefault(t => t.GetCustomAttribute<DayAttribute>()?.Day == day);

        if (calculatorType is not null)
        {
            return Activator.CreateInstance(calculatorType) as IDayCalculator;
        }

        return null;
    }
}
